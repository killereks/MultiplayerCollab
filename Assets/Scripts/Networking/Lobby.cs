using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Lobby : NetworkBehaviour {

    public static Lobby Instance;

    public TMP_InputField serverAddressInput;

    public TextMeshProUGUI errorMessage;

    public NetworkManager networkManager;

    public GameObject lobbyConnectionGO;

    public GameObject playerUIPrefab;
    public Transform playerListParent;

    public RoundManager roundManager;

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

    [Server]
    public void RefreshPlayersUI() {
        List<string> playerUsernames = new List<string>();
        List<bool> readyList = new List<bool>();

        foreach (PlayerData player in GetAllPlayers()) {
            playerUsernames.Add(player.GetUsername());
            readyList.Add(player.isReady);
        }

        if (CanStartGame(readyList)) {
            // start game here
            roundManager.StartGame();
        }

        RpcRefreshPlayersUI(playerUsernames, readyList);
    }

    [Server]
    public bool CanStartGame(List<bool> readyList) {
        foreach (bool isReady in readyList) {
            if (!isReady) return false;
        }

        CustomNetworkManager NM = CustomNetworkManager.Instance;

        if (NM.numPlayers < 2) return false;
        if (NM.numPlayers > NM.maxConnections) return false;

        return true;
    }

    // two lists because serializing a class over network is more difficult
    [ClientRpc]
    void RpcRefreshPlayersUI(List<string> playerUsernames, List<bool> readyList) {
        foreach (Transform child in playerListParent) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerUsernames.Count; i++) {
            NewPlayer(playerUsernames[i], readyList[i]);
        }
    }

    public void NewPlayer(string username, bool isReady) {
        //NetworkConnection host = CustomNetworkManager.Instance.host;

        GameObject newPlayer = Instantiate(playerUIPrefab, playerListParent);

        Tools.FindDeepChild<TextMeshProUGUI>(newPlayer, "UsernameText").text = username;

        Tools.FindDeepChild<Transform>(newPlayer, "ReadyBorder").gameObject.SetActive(isReady);
        // we are a host and we are not trying to kick ourselves
        /*Tools.FindDeepChild<Button>(newPlayer, "KickBtn").gameObject.SetActive(host.identity == netIdentity && conn != host);

        Tools.FindDeepChild<Button>(newPlayer, "KickBtn").onClick.AddListener(() => conn.Disconnect());*/
    }

    public List<PlayerData> GetAllPlayers() {
        return activePlayers.Values.ToList();
    }
}


