using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Mirror;

public class PlayerMovement : NetworkBehaviour {

    [BoxGroup("Player Settings")]
    public float walkingSpeed;
    [BoxGroup("Player Settings")]
    public float runningSpeed;
    [BoxGroup("Player Settings")]
    public float gravityScale;
    [BoxGroup("Object Settings")]
    public Transform playerT;

    Rigidbody rb;

    [BoxGroup("Object Settings")]
    public Cinemachine.CinemachineVirtualCamera cam;

    float cameraXRotation;
    [BoxGroup("Camera Settings")]
    public Vector2 cameraClamp;
    [BoxGroup("Camera Settings")]
    public float mouseSens = 2f;

    bool canMove;

    Collider collider;

    Animator anim;

    private void Start() {
        rb = GetComponent<Rigidbody>();

        collider = GetComponent<Collider>();
        anim = GetComponentInChildren<Animator>();

        LockCursor(true);
        canMove = true;

        if (!isLocalPlayer) {
            cam.Priority = 0;
        }
    }

    public void LockCursor(bool newState) {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !newState;

        ToggleMovement(newState);
    }
    
    public void ToggleMovement(bool newState) {
        canMove = newState;

        if (!canMove) rb.velocity = Vector3.zero;
    }

    private void Update() {
        if (!isLocalPlayer) return;

        cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, IsRunning() ? 75f : 60f, 10f * Time.deltaTime);
        if (canMove) {
            cameraController();
        }
    }

    private void FixedUpdate() {
        if (!isLocalPlayer) return;

        if (canMove) movementController();
    }

    private void cameraController() {
        Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSens;
        
        playerT.Rotate(Vector3.up, mouse.x, Space.World);

        cameraXRotation -= mouse.y;
        cameraXRotation = Mathf.Clamp(cameraXRotation, cameraClamp.x, cameraClamp.y);

        cam.transform.eulerAngles = new Vector3(cameraXRotation, cam.transform.eulerAngles.y, 0f);
    }

    public void MoveTo(Transform pos) {
        transform.position = pos.position;
    }

    private void movementController() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        float gravity = rb.velocity.y + Physics.gravity.y * gravityScale * Time.fixedDeltaTime;

        if (IsGrounded() && rb.velocity.y < 0f) {
            gravity = 0f;
        }

        float speed = walkingSpeed;
        if (IsRunning()) {
            speed = runningSpeed;
        }

        anim.SetFloat("velocityY", Mathf.Lerp(anim.GetFloat("velocityY"), input.y, 10f * Time.deltaTime));

        Vector3 direction = cam.transform.forward * input.y + cam.transform.right * input.x;
        direction.y = 0f;
        direction = direction.normalized * speed;

        direction *= transform.localScale.magnitude;

        rb.velocity = direction + Vector3.up * gravity;
    }

    bool IsRunning() {
        return Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0f;
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + 0.1f, 1);
    }
}
