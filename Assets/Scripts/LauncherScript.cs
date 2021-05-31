using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LauncherScript : MonoBehaviour {
    [BoxGroup("Settings")]
    public float height;
    [BoxGroup("Settings")]
    public int pathResolution;
    [BoxGroup("Objects")]
    public Transform target;

    public AudioSource launchAudio;

    public LineRenderer line;

    public void Start() {
        float displacementY = target.position.y - transform.position.y;
        if (displacementY > height) {
            height = displacementY + 3f;
        }

        DrawPath();
    }

    void OnDrawGizmos() {
        DrawPath();
    }

    void DrawPath() {
        Vector3 initialVelocity = GetLaunchVelocity(transform);

        float gravity = Physics.gravity.y;

        line.positionCount = pathResolution+1;
        line.SetPosition(0, transform.position);

        for (int i = 1; i <= pathResolution; i++) {
            float simulationTime = i/(float)pathResolution * GetTotalTime(transform);
            Vector3 displacement = initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            line.SetPosition(i, drawPoint);
        }

    }

    private void OnTriggerStay(Collider other) {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        PlayerParkourMovement parkourMovement = other.GetComponent<PlayerParkourMovement>();

        if (rb != null && parkourMovement != null) {
            print(other.transform.name);
            Vector3 newVelocity = GetLaunchVelocity(other.transform);
            rb.velocity = newVelocity;
            parkourMovement.SetYVelocity(newVelocity.y);
            parkourMovement.currentState = PlayerParkourMovement.PlayerStates.InAir;
            //launchAudio.Play();
        }
    }

    Vector3 GetLaunchVelocity(Transform obj) {
        Vector3 displacement = target.position - obj.position;

        float gravity = Physics.gravity.y;
        float time = GetTotalTime(obj);
        
        float velocityY = Mathf.Sqrt(-2f * gravity * height);
        Vector3 velocityXZ = new Vector3(displacement.x/time, 0f, displacement.z/time);

        return velocityXZ + Vector3.up * velocityY;
    }

    float GetTotalTime(Transform obj) {
        Vector3 displacement = target.position - obj.position;
        float gravity = Physics.gravity.y;

        return Mathf.Sqrt(-2f * height / gravity) + Mathf.Sqrt(2f * ( displacement.y - height ) / gravity);
    }
}
