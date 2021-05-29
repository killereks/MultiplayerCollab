using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class PlayerData : NetworkBehaviour {

    public Transform canvas;

    Transform cam;

    public TextMeshProUGUI nicknameText;

    [SyncVar(hook ="UpdateNickname")]
    string nickname;

    private void Start() {
        cam = Camera.main.transform;

        nickname = "Player" + Random.Range(0, 9999);

        nicknameText.text = nickname;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            CmdSetName("Player" + Random.Range(0, 9999));
        }
    }

    [Command]
    public void CmdSetName(string name) {
        nickname = name;
    }

    void UpdateNickname(string oldValue, string newValue) {
        nicknameText.text = newValue;
    }

    private void LateUpdate() {
        canvas.forward = cam.forward;
    }
}
