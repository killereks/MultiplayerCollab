using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class Lobby : NetworkBehaviour {

    public static Lobby Instance;

    public TMP_InputField serverAddressInput;

    public TextMeshProUGUI errorMessage;

    public NetworkManager networkManager;

    public GameObject lobbyConnectionGO;

    public GameObject playerUIPrefab;
    public Transform playerListParent;

    public SyncDictionary<int, PlayerData> activePlayers = new SyncDictionary<int, PlayerData>();

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
        List<string> playerUsernames = new List<string>();

        foreach (KeyValuePair<int, PlayerData> keyValuePair in activePlayers) {
            playerUsernames.Add(keyValuePair.Value.GetUsername());
        }

        RpcRefreshPlayersUI(playerUsernames);
    }

    [ClientRpc]
    void RpcRefreshPlayersUI(List<string> playerUsernames) {
        foreach (Transform child in playerListParent) {
            Destroy(child.gameObject);
        }

        foreach (string username in playerUsernames) {
            NewPlayer(username);
        }
    }

    public void NewPlayer(string username) {
        //NetworkConnection host = CustomNetworkManager.Instance.host;

        GameObject newPlayer = Instantiate(playerUIPrefab, playerListParent);

        Tools.FindDeepChild<TextMeshProUGUI>(newPlayer, "UsernameText").text = username;
        // we are a host and we are not trying to kick ourselves
        /*Tools.FindDeepChild<Button>(newPlayer, "KickBtn").gameObject.SetActive(host.identity == netIdentity && conn != host);

        Tools.FindDeepChild<Button>(newPlayer, "KickBtn").onClick.AddListener(() => conn.Disconnect());*/
    }
}


