using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Player : NetworkBehaviour {

    public static Player localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    NetworkMatchChecker networkMatchChecker;

    private void Start() {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();

        if (isLocalPlayer) {
            localPlayer = this;
        } else {
            UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }

    /*
     * HOST GAME
     */

    public void HostGame() {
        string matchID = MatchMaker.GetRandomMatchID();
        Debug.Log("Hosted game with match id: " + matchID);
        CmdHostGame(matchID);
    }

    [Command]
    void CmdHostGame(string _matchID) {
        matchID = _matchID;
        if (MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex)) {
            Debug.Log("Hosting game: success");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetHostGame(true, _matchID);
        } else {
            Debug.Log("Hosting game: failed");
            TargetHostGame(false, _matchID);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID) {
        UILobby.instance.HostSuccess(success);
    }

    /*
     * JOIN GAME
     */

    public void JoinGame(string _inputID) {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID) {
        matchID = _matchID;
        if (MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex)) {
            Debug.Log("Joining game: success");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID);
        } else {
            Debug.Log("Joining game: failed");
            TargetJoinGame(false, _matchID);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID) {
        UILobby.instance.JoinSuccess(success);
    }

    /*
     * BEGIN GAME
     */

    public void BeginGame() {
        CmdBeginGame();
    }

    [Command]
    void CmdBeginGame() {
        MatchMaker.instance.BeginGame();
        Debug.Log("Beginning game: failed");
        RpcBeginGame();
    }

    [ClientRpc]
    void RpcBeginGame() {
        Debug.Log("Match ID: " + matchID + " | Beginning");
    }
}
