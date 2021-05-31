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

    public bool isReady;

    [SyncVar(hook = nameof(TaggedCallback))]
    public bool isTagged;

    public GameObject dynamiteGO;

    [SyncVar(hook = nameof(DeathCallback))]
    public bool isDead;

    float defaultRunSpeed;
    public float taggedRunSpeed = 12f;

    PlayerParkourMovement parkourController;

    private void Awake() {
        parkourController = GetComponent<PlayerParkourMovement>();

        nickname = "Player" + Random.Range(0, 9999);
        nicknameText.text = nickname;

        dynamiteGO.SetActive(false);
    }

    private void Start() {
        cam = Camera.main.transform;
    }

    private void Update() {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.G)) {
            CmdSetName("Player" + Random.Range(0, 9999));
        }

        if (Input.GetKeyDown(KeyCode.T) && isLocalPlayer)
        {
            Debug.Log(Lobby.Instance.activePlayers[connection].nickname);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            CmdToggleReady();
        }

        if (Input.GetMouseButtonDown(0) && isTagged && !isDead) {
            CmdAttemptTagAnotherPlayer();
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

    [Command]
    public void CmdToggleReady() {
        isReady = !isReady;

        Lobby.Instance.RefreshPlayersUI();
    }

    [Command]
    public void CmdAttemptTagAnotherPlayer() {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f)) {
            PlayerData playerData = hit.collider.GetComponent<PlayerData>();

            if (playerData != null) {
                playerData.isTagged = true;
                isTagged = false;
            }
        }
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

    public void TaggedCallback(bool oldValue, bool newValue) {
        dynamiteGO.SetActive(newValue);

        if (newValue) {
            parkourController.runningSpeed = taggedRunSpeed;
        } else {
            parkourController.runningSpeed = defaultRunSpeed;
        }
    }

    public void DeathCallback(bool oldValue, bool newValue) {
        if (newValue) {
            GetComponent<PlayerDeath>().StartAnimation();
        }
    }
}
