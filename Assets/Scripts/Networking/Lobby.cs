using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Lobby : NetworkBehaviour {

    public static Lobby Instance;

    public TMP_InputField serverAddressInput;

    public TextMeshProUGUI errorMessage;

    public NetworkManager networkManager;

    public GameObject lobbyConnectionGO;

    public GameObject playerUIPrefab;
    public Transform playerListParent;

    private void Awake() {
        Instance = this;
    }


    public void JoinGame() {
        if (string.IsNullOrEmpty(serverAddressInput.text)) {
            errorMessage.text = "Type in an IP address to join a game";
            return;
        }

        networkManager.networkAddress = serverAddressInput.text;
        networkManager.StartClient();

        lobbyConnectionGO.SetActive(false);
    }

    public void HostGame() {
        networkManager.networkAddress = Tools.GetMyIP().ToString();
        networkManager.StartHost();

        lobbyConnectionGO.SetActive(false);
    }

    public void RefreshPlayersUI() {
        foreach (Transform child in playerListParent) {
            Destroy(child.gameObject);
        }

        foreach (PlayerData player in CustomNetworkManager.instance.activePlayers) {
            RpcRefreshPlayersUI(player.GetUsername());
        }
    }

    [ClientRpc]
    void RpcRefreshPlayersUI(string username) {
        NewPlayer(username);
    }

    public void NewPlayer(string username) {
        GameObject newPlayer = Instantiate(playerUIPrefab, playerListParent);

        Tools.FindDeepChild<TextMeshProUGUI>(newPlayer, "UsernameText").text = username;
    }
}
