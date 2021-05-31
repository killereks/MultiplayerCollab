using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerParkourMovement : NetworkBehaviour {
    public enum PlayerStates {
        Grounded,//on the ground
        InAir, //in the air
        Wallrunning, //running on the walls
        LedgeGrab, // holding and climbing a ledge
        Sliding // sliding down ramps
    }

    CapsuleCollider collider;
    Rigidbody rb;

    [Header("Physics")]
    public float gravity = -9.81f;
    float yVelocity;

    [Header("Camera")]
    float minFov;
    public float maxFov = 75f;
    public float maxSpeed = 9f;
    public Cinemachine.CinemachineVirtualCamera vCam;
    float cameraXRot;

    [Header("Player")]
    public PlayerStates currentState;
    public float walkingSpeed = 4f;
    public float runningSpeed = 9f;
    public float crouchMovementSpeed = 2f;
    public float jumpHeight = 1f;

    [Header("In Air")]
    public float inAirMovementMultiplier = 2f;

    [Header("Ledge Climbing")]

    public float ledgeCheckForwardOffset = 0.7f;
    public float ledgeCheckUpOffset = 0.5f;
    public float ledgeCheckDistance = 0.6f;

    float ledgeGrabTimer;
    public LayerMask ledgeGrabLayer;

    bool climbingLedge;
    Vector3 climbingDefaultPos;
    Vector3 climbingTargetPos;
    public AnimationCurve climbingCurveForward;
    public AnimationCurve climbingCurveY;
    float climbingLedgeTimer;
    bool canLedgeGrab;
    public float maxLedgeHoldTime = 1f;

    [Header("Wall running")]
    public float wallRunningCheckDistance = 1f;

    float cameraTiltTarget;
    public float wallrunningAngle = 15f;
    public float wallrunningAngleSmooth = 10f;
    float wallrunStickTimer;
    float wallrunDelay;

    public float wallrunUpJumpForce = 5f;
    public float wallrunSideJumpForce = 4f;
    public float wallrunForwardJumpForce = 10f;

    [Header("Crouching")]
    public float crouchHeight = 1f;
    public float crouchRayCheckLength = 0.5f;
    public float heightSmooth = 10f;
    public float slideDownForce = 5000f;
    public float slideCounterForce = 500f;

    public float crouchCameraHeight = 0.3f;

    float defaultHeight;
    float defaultCameraHeight;

    PlayerSprintSystem playerSprintSystem;
    Animator anim;
    NetworkAnimator networkAnim;

    public SkinnedMeshRenderer modelMesh;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        collider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        networkAnim = GetComponent<NetworkAnimator>();

        defaultHeight = collider.height;
        defaultCameraHeight = vCam.transform.localPosition.y;

        minFov = vCam.m_Lens.FieldOfView;

        playerSprintSystem = GetComponent<PlayerSprintSystem>();

        if (!isLocalPlayer) {
            vCam.Priority = 10;
            rb.isKinematic = true;
        } else {
            LeanTween.delayedCall(0.5f, () => {
                modelMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            });
        }
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        AnimateCameraFov();
        AnimateCrouch();

        CameraLook();

        wallrunStickTimer -= Time.deltaTime;
        wallrunDelay -= Time.deltaTime;

        switch (currentState) {
            case PlayerStates.Grounded:
                ApplyGravity();
                GroundedMovement(input);
                break;
            case PlayerStates.InAir:
                ApplyGravity();
                InAirMovement(input);
                break;
            case PlayerStates.Wallrunning:
                ApplyGravity();
                WallrunMovement(input);
                break;
            case PlayerStates.LedgeGrab:
                LedgeGrabMovement(input);
                break;
            default:
                break;
        }
    }

    private void FixedUpdate() {
        if (!isLocalPlayer) return;

        if (currentState == PlayerStates.Sliding) {
            SlidingMovement();
        }
    }

    void AnimateCrouch() {
        float targetHeight = defaultHeight;
        float targetCameraHeight = defaultCameraHeight;
        if (IsCrouching()) {
            targetHeight = crouchHeight;
            targetCameraHeight = crouchCameraHeight;
        }

        collider.height = Mathf.Lerp(collider.height, targetHeight, heightSmooth * Time.deltaTime);
        vCam.transform.localPosition = Vector3.Lerp(vCam.transform.localPosition, Vector3.up * targetCameraHeight, heightSmooth * Time.deltaTime);
    }

    void AnimateCameraFov() {
        float FOVSmooth = 10f;

        Vector3 velocityWithoutY = rb.velocity;
        velocityWithoutY.y = 0f;

        float targetFOV = Mathf.Lerp(minFov, maxFov, velocityWithoutY.magnitude/maxSpeed);

        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, targetFOV, FOVSmooth * Time.deltaTime);
    }

    void ApplyGravity() {
        yVelocity += gravity * Time.deltaTime;
        rb.velocity = new Vector3(rb.velocity.x, yVelocity, rb.velocity.z);
    }

    void CameraLook() {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.Rotate(0f, mouseInput.x, 0f);

        cameraXRot -= mouseInput.y;

        cameraXRot = Mathf.Clamp(cameraXRot, -89f, 89f);

        vCam.transform.localEulerAngles = new Vector3(cameraXRot, 0f, cameraTiltTarget);
    }

    void SlidingMovement() {
        rb.AddForce(Vector3.down * slideDownForce);

        rb.AddForce(-rb.velocity.normalized * slideCounterForce);

        if (!IsCrouching()) {
            currentState = PlayerStates.Grounded;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.magnitude > 0f) {
            Vector3 movementVector = GetMovementVector(input);
            rb.velocity = movementVector * crouchMovementSpeed;
        }
    }

    void GroundedMovement(Vector2 input) {
        if (IsGrounded()) {
            canLedgeGrab = true;
            wallrunStickTimer = 0f;

            if (yVelocity < 0f) yVelocity = 0f;

            if (Input.GetKeyDown(KeyCode.Space)) {
                yVelocity = GetJumpForce();
                currentState = PlayerStates.InAir;
            }
            
            if (IsCrouching()) {
                currentState = PlayerStates.Sliding;
            }

        } else {
            currentState = PlayerStates.InAir;
        }

        cameraTiltTarget = Mathf.Lerp(cameraTiltTarget, 0f, wallrunningAngleSmooth * Time.deltaTime);

        Vector3 moveDirection = GetMovementVector(input);

        float targetSpeed = walkingSpeed;
        if (IsRunning() && IsGrounded()) { 
            targetSpeed = runningSpeed;
            playerSprintSystem.Sprinting();
        }

        moveDirection *= targetSpeed;

        if (!IsCrouching()) {
            rb.velocity = moveDirection;
        }

        anim.SetFloat("velocityY", Mathf.Lerp(anim.GetFloat("velocityY"), input.y * targetSpeed / runningSpeed, 10f * Time.deltaTime));
        anim.SetFloat("velocityX", Mathf.Lerp(anim.GetFloat("velocityX"), input.x, 10f * Time.deltaTime));
    }

    Vector3 GetMovementVector(Vector2 input) {
        Vector3 moveDirection = (transform.forward * input.y) + (transform.right * input.x);
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;
        return moveDirection;
    }

    void InAirMovement(Vector2 input) {
        cameraTiltTarget = Mathf.Lerp(cameraTiltTarget, 0f, wallrunningAngleSmooth * Time.deltaTime);

        if (IsGrounded() && yVelocity <= 0f) {
            currentState = PlayerStates.Grounded;
        }

        if (CheckLedge(out RaycastHit hit) && canLedgeGrab) {
            climbingTargetPos = hit.point + Vector3.up;
            if (Vector3.Distance(hit.point, transform.position) <= 6f) {
                currentState = PlayerStates.LedgeGrab;
                networkAnim.SetTrigger("holdingLedge");
                climbingDefaultPos = transform.position;
                ledgeGrabTimer = maxLedgeHoldTime;
                yVelocity = 0f;
            }
        }

        if (wallrunDelay <= 0f && CheckForWalls()) {
            currentState = PlayerStates.Wallrunning;
        }
    }

    void LedgeGrabMovement(Vector2 input) {
        rb.velocity = Vector3.zero;

        if (!climbingLedge) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) {
                climbingLedge = true;
                climbingDefaultPos = transform.position;
                networkAnim.SetTrigger("climbing");

                if (CheckLedge(out RaycastHit hit)) {
                    climbingTargetPos = hit.point + Vector3.up;
                } else {
                    currentState = PlayerStates.InAir;
                }
            }
        }

        if (climbingLedge) {
            climbingLedgeTimer += Time.deltaTime;
            climbingLedgeTimer = Mathf.Clamp01(climbingLedgeTimer);

            float forwardPercentage = climbingCurveForward.Evaluate(climbingLedgeTimer);
            float yOffsetPercentage = climbingCurveY.Evaluate(climbingLedgeTimer);

            Vector3 forwardPos = Vector3.Lerp(climbingDefaultPos, climbingTargetPos, forwardPercentage);
            float yPos = Mathf.LerpUnclamped(climbingDefaultPos.y, climbingTargetPos.y, yOffsetPercentage);

            transform.position = new Vector3(forwardPos.x, yPos, forwardPos.z);

            if (climbingLedgeTimer >= 1f) {
                currentState = PlayerStates.InAir;
                climbingLedgeTimer = 0f;
                climbingLedge = false;
                networkAnim.SetTrigger("stopHoldingLedge");
            }
        } else {
            ledgeGrabTimer -= Time.deltaTime;
            if (ledgeGrabTimer <= 0f) {
                currentState = PlayerStates.InAir;
                canLedgeGrab = false;
                climbingDefaultPos = transform.position;
                networkAnim.SetTrigger("stopHoldingLedge");
            }
        }
    }

    void WallrunMovement(Vector2 input) {
        float stickForce = 100f;

        RaycastHit leftWall;
        RaycastHit rightWall;

        bool isWallrunning = false;

        Vector3 jumpSideDirection = Vector3.zero;

        if (Physics.Raycast(transform.position, transform.right, out rightWall, wallRunningCheckDistance)) {
            if (wallrunStickTimer <= 0f) {
                rb.AddForce(transform.right * stickForce);
            }
            isWallrunning = true;
            cameraTiltTarget = Mathf.Lerp(cameraTiltTarget, wallrunningAngle, wallrunningAngleSmooth * Time.deltaTime);
            jumpSideDirection = -transform.right;
        }

        if (Physics.Raycast(transform.position, -transform.right, out leftWall, wallRunningCheckDistance)) {
            if (wallrunStickTimer <= 0f) {
                rb.AddForce(-transform.right * stickForce);
            }
            isWallrunning = true;
            cameraTiltTarget = Mathf.Lerp(cameraTiltTarget, -wallrunningAngle, wallrunningAngleSmooth * Time.deltaTime);
            jumpSideDirection = transform.right;
        }

        if (!isWallrunning) {
            currentState = PlayerStates.InAir;
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 jumpForce = transform.forward * wallrunForwardJumpForce + jumpSideDirection * wallrunSideJumpForce + transform.up * wallrunUpJumpForce;
            rb.velocity = jumpForce;
            yVelocity = jumpForce.y;
            currentState = PlayerStates.InAir;
            wallrunStickTimer = 0.5f;
            wallrunDelay = 0.2f;
        }
        if (IsGrounded() && yVelocity < 0f) {
            currentState = PlayerStates.Grounded;
        }
    }

    bool CheckForWalls() {
        if (Physics.Raycast(transform.position, transform.right, wallRunningCheckDistance)) {
            return true;
        }

        if (Physics.Raycast(transform.position, -transform.right, wallRunningCheckDistance)) {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos() {
        Debug.DrawRay(GetLedgeCheckPosition(), Vector3.down * ledgeCheckDistance, Color.red);

        Gizmos.DrawWireSphere(GetLedgeCheckPosition(), 0.2f);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, Vector3.up * crouchRayCheckLength);
    }

    Vector3 GetLedgeCheckPosition() {
        return vCam.transform.position + vCam.transform.forward * ledgeCheckForwardOffset + Vector3.up * ledgeCheckUpOffset;
    }

    bool CheckLedge(out RaycastHit hit) {
        Vector3 ledgeCheckPos = GetLedgeCheckPosition();
        return Physics.Raycast(ledgeCheckPos, Vector3.down, out hit, ledgeCheckDistance, ledgeGrabLayer) && !Physics.CheckSphere(ledgeCheckPos, 0.2f);
    }

    bool IsRunning() {
        return Input.GetAxisRaw("Vertical") > 0f && Input.GetKey(KeyCode.LeftShift) && IsGrounded() && playerSprintSystem.CanSprint();
    }

    float GetJumpForce() {
        // using suvat equations
        return Mathf.Sqrt(-2f * jumpHeight * gravity);
    }

    public void SetYVelocity(float yVel) {
        yVelocity = yVel;
    }

    bool IsGrounded() {
        float distToGround = collider.bounds.extents.y;
        float margin = 0.1f;
        float thickness = 0.1f;

        Ray ray = new Ray(transform.position, -Vector3.up);

        return Physics.SphereCast(ray, thickness, distToGround + margin);
    }

    bool IsCrouching() {
        return Input.GetKey(KeyCode.LeftControl) || Physics.Raycast(transform.position, Vector3.up, crouchRayCheckLength);
    }
}