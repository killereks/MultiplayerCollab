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

    private void Awake()
    {
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

    //TODO actually check for authority
    [Command(ignoreAuthority = true)]
    public void AddNewPlayer()
    {
        //ClientScene.RegisterPrefab(playerUIPrefab);
        GameObject newPlayer = NetworkIdentity.Instantiate(playerUIPrefab, playerListParent);
        NetworkServer.Spawn(newPlayer, connectionToClient);
    }
}
