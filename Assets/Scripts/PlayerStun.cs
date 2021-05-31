using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStun : MonoBehaviour {

    float stunTimer;

    PlayerParkourMovement parkourController;
    Rigidbody rb;

    bool stunned = false; // used to only disable movement once

    public void Start() {
        parkourController = GetComponent<PlayerParkourMovement>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetStunTime(float time) {
        stunTimer = time;
        parkourController.enabled = false;
        rb.velocity = Vector3.zero;

        stunned = true;
    }

    void Update() {
        if (!stunned) return;

        stunTimer -= Time.deltaTime;
        
        if (stunTimer <= 0f) {
            stunned = false;

            parkourController.enabled = true;
        }
    }
}
