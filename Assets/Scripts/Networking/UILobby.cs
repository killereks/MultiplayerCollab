using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILobby : MonoBehaviour {

    [Header("Host Join")]
    public TMP_InputField joinMatchInput;

    public Button joinButton;
    public Button hostButton;

    public Canvas lobbyCanvas;

    public static UILobby instance;

    [Header("Lobby")]
    public Transform UIPlayerParent;
    public GameObject UIPlayerPrefab;

    public TextMeshProUGUI matchIDText;
    public GameObject beginGameButton;

    private void Start() {
        instance = this;
    }

    public void Host() {
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.HostGame();
    }

    public void HostSuccess(bool success) {
        if (success) {
            lobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(Player.localPlayer);

            matchIDText.text = Player.localPlayer.matchID;
            beginGameButton.SetActive(true);
        } else {
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void Join() {
        joinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.JoinGame(joinMatchInput.text);
    }

    public void JoinSuccess(bool success) {
        if (success) {
            lobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = Player.localPlayer.matchID;
        } else {
            joinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void SpawnPlayerUIPrefab(Player player) {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);

        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
    }

    public void BeginGame() {
        Player.localPlayer.BeginGame();
    }
}
