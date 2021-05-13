using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

public class MatchMaker : NetworkBehaviour {

    public static MatchMaker instance;

    public SyncListMatch matches = new SyncListMatch();
    public SyncListString matchIDs = new SyncListString();

    private void Start() {
        instance = this;
    }

    public bool HostGame(string _matchID, GameObject _player, out int playerIndex) {
        playerIndex = -1;
        if (!matchIDs.Contains(_matchID)) {
            matchIDs.Add(_matchID);
            matches.Add(new Match(_matchID, _player));
            Debug.Log("Match generated");
            playerIndex = 0;
            return true;
        }

        Debug.Log("Match ID already exists.");
        return false;
    }

    public bool JoinGame(string _matchID, GameObject _player, out int playerIndex) {
        playerIndex = -1;
        if (matchIDs.Contains(_matchID)) {

            for (int i = 0; i < matches.Count; i++) {
                if (matches[i].matchID == _matchID) {
                    matches[i].players.Add(_player);
                    playerIndex = matches[i].players.Count - 1;
                    break;
                }
            }

            Debug.Log("Match joined");
            return true;
        }

        Debug.Log("Match ID does not exist.");
        return false;
    }

    public void BeginGame() {

    }

    public static string GetRandomMatchID() {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++) {
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26) {
                _id += (char) ( random + 65 );
            } else {
                _id += ( random - 26 ).ToString();
            }
        }
        return _id;
    }
}

public static class MatchExtensions {
    public static Guid ToGuid(this string id) {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}

[System.Serializable]
public class Match {
    public string matchID;
    public SyncListGameObject players = new SyncListGameObject();

    public Match(string matchID, GameObject player) {
        this.matchID = matchID;
        players.Add(player);
    }

    // network needs to have a serialized class, to enable this we need to write an empty constructor
    public Match() { }
}

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> {

}

[System.Serializable]
public class SyncListMatch : SyncList<Match> {

}