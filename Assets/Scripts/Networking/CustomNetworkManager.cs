using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CustomNetworkManager : NetworkManager {

    public TextMeshProUGUI errorMessage;

    public Transform spawnPoint;

    public static CustomNetworkManager Instance;

    public NetworkConnection host;

    public override void Awake() {
        base.Awake();

        Instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        GameObject newPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, newPlayer);
        PlayerData thisPlayer = newPlayer.GetComponent<PlayerData>();
        
        if (!Lobby.Instance.activePlayers.ContainsKey(conn.connectionId)) 
        {
            //Debug.Log("oi " + (thisPlayer != null) + conn.connectionId + " ");
            thisPlayer.connection = conn.connectionId;
            Debug.Log(" server connection #" + thisPlayer.connection +  " name: "+ thisPlayer.nicknameText.text);

            Lobby.Instance.activePlayers.Add(conn.connectionId, thisPlayer);
        }


        Lobby.Instance.RefreshPlayersUI();

        if (host == null) {
            host = conn;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        Lobby.Instance.activePlayers.Remove(conn.connectionId);
        Lobby.Instance.RefreshPlayersUI();
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        // the game is full, auto disconnect.
        if (numPlayers > maxConnections) {
            conn.Disconnect();
        }
    }

    [System.Obsolete]
    public override void OnClientError(NetworkConnection conn, int errorCode) {
        string[] errorMessages = new string[]{
            "",
            "Host doesn't exist.",
            "Connection doesn't exist.",
            "Channel doesn't exist.",
            "No internal resources can accomplish this request.",
            "Obsolete.",
            "Timeout.",
            "Sending a message too long to fit internal buffers, or user doesn't present buffer with length enough to contain receiving message.",
            "Operation is not supported.",
            "Different version of protocol on ends of connection.",
            "Two ends of connection have different agreement about channels, channels qos and network parameters.",
            "DNS Failure: The address supplied to connect to was invalid or could not be resolved."
        };

        errorMessage.text = errorMessages[errorCode];
    }
}
