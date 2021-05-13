using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementInterpolation : NetworkBehaviour {
    public float smoothSpeed = 10f;

    [SyncVar]
    private Vector3 mostRecentPos;
    private Vector3 prevPos;

    Rigidbody rb;

    float syncInterval;

    float predictionTime;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        syncInterval -= Time.deltaTime;
        predictionTime += Time.deltaTime;

        if (isLocalPlayer) {
            if (prevPos != transform.position && syncInterval <= 0f) {
                CmdSendDataToServer(transform.position);
                prevPos = transform.position;
                syncInterval = 0.1f; // 10 times a sec
            }
        } else {
            Vector3 prediction = mostRecentPos + rb.velocity * predictionTime;
            transform.position = Vector3.Lerp(transform.position, prediction, smoothSpeed * Time.deltaTime);
        }
    }

    [Command]
    void CmdSendDataToServer(Vector3 pos) {
        mostRecentPos = pos;
        predictionTime = 0f;
        RPCSyncPosition(pos);
    }

    [ClientRpc]
    void RPCSyncPosition(Vector3 pos) {
        mostRecentPos = pos;
        predictionTime = 0f;
    }
}
