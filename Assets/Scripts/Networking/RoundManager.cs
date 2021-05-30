using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundManager : NetworkBehaviour {

    public Transform spawnPoint;

    public SyncList<PlayerData> taggedPlayers = new SyncList<PlayerData>();

    float explodeTimer = 10f;

    bool gameRunning = false;
    
    [Server]
    public void StartGame() {
        SyncDictionary<int, PlayerData> playersList = Lobby.Instance.activePlayers;
        
        foreach (KeyValuePair<int, PlayerData> player in playersList) {
            NetworkTransform nTransform = player.Value.gameObject.GetComponent<NetworkTransform>();
            nTransform.ServerTeleport(spawnPoint.position);
        }

        gameRunning = true;

        TagRandomPlayer();
    }

    [Server]
    public void TagRandomPlayer() {
        List<PlayerData> players = FindObjectsOfType<PlayerData>().ToList();

        // filter all alive non-tagged players
        players = players.FindAll(x => !x.isTagged && !x.isDead);

        // WE HAVE A WINNER!!!
        if (players.Count == 1) {
            gameRunning = false;
        } else {
            PlayerData randomPlayer = Tools.PickRandom(players.ToArray());

            taggedPlayers.Add(randomPlayer);
            randomPlayer.isTagged = true;
        }
    }

    [Server]

    private void Update() {
        if (!gameRunning) return;

        explodeTimer -= Time.deltaTime;

        if (explodeTimer <= 0f) {
            ExplodePlayers();
            TagRandomPlayer();
            explodeTimer = 10f;
        }
    }

    [Server]
    void ExplodePlayers() {
        foreach (PlayerData playerData in taggedPlayers) {
            playerData.isTagged = false;
            playerData.isDead = true;
        }
    }
}
