using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class PlayerData : NetworkBehaviour {

    public Transform canvas;

    Transform cam;

    public TextMeshProUGUI nicknameText;

    [SyncVar(hook = nameof(UpdateNickname))]
    string nickname;

    [SyncVar]
    public int connection;

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

        if (Input.GetKeyDown(KeyCode.T) && isLocalPlayer)
        {
            Debug.Log(Lobby.Instance.activePlayers[connection].nickname);
        }
    }

    [Command]
    public void CmdSetName(string name) {
        nickname = name;

        Lobby.Instance.RefreshPlayersUI();
    }

    void UpdateNickname(string oldValue, string newValue) {
        nicknameText.text = newValue;
    }

    private void LateUpdate() {
        canvas.forward = cam.forward;
    }

    public override void OnStartServer() {
        base.OnStartServer();

        //CustomNetworkManager.Instance.activePlayers.Add(this);
    }

    public override void OnStopServer() {
        base.OnStopServer();

        //CustomNetworkManager.Instance.activePlayers.Remove(this);
    }

    public string GetUsername() {
        return nickname;
    }
}
