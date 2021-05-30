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

    private void Awake() {
        nickname = "Player" + Random.Range(0, 9999);
        nicknameText.text = nickname;
    }

    private void Start() {
        cam = Camera.main.transform;
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

    public override void OnStartServer() {
        base.OnStartServer();

        CustomNetworkManager.instance.activePlayers.Add(this);
    }

    public override void OnStopServer() {
        base.OnStopServer();

        CustomNetworkManager.instance.activePlayers.Remove(this);
    }

    public string GetUsername() {
        return nickname;
    }
}
