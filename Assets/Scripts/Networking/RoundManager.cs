using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundManager : NetworkBehaviour {

    public Transform spawnPoint;

    [Range(0,300)]
    public float baseExplodeTimer = 10f;
    float explodeTimer = 10f;

    bool gameRunning = false;

    public Transform lobbySpawnPoint;
    
    void Start() {
        explodeTimer = baseExplodeTimer;
    }

    [Server]
    public void StartGame() {
        TeleportEveryoneTo(spawnPoint.position);

        gameRunning = true;

        TagRandomPlayer();
    }

    [Server]
    public void TeleportEveryoneTo(Vector3 pos) {
        SyncDictionary<int, PlayerData> playersList = Lobby.Instance.activePlayers;

        foreach (KeyValuePair<int, PlayerData> player in playersList) {
            NetworkTransform nTransform = player.Value.gameObject.GetComponent<NetworkTransform>();
            nTransform.ServerTeleport(pos);
        }
    }

    [Server]
    public void TagRandomPlayer() {
        List<PlayerData> players = FindObjectsOfType<PlayerData>().ToList();

        // filter all alive non-tagged players
        players = players.FindAll(x => !x.isTagged && !x.isDead);

        // WE HAVE A WINNER!!!
        if (players.Count == 1) {
            gameRunning = false;

            LeanTween.delayedCall(10f, () => {
                CustomNetworkManager.Instance.ServerChangeScene("Lobby");
            });
        } else {
            PlayerData randomPlayer = Tools.PickRandom(players.ToArray());

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
            explodeTimer = baseExplodeTimer;
        }
    }

    [Server]
    void ExplodePlayers() {
        foreach (PlayerData player in Lobby.Instance.GetAllPlayers()) {
            if (player.isTagged && !player.isDead) {
                player.isTagged = false;
                player.isDead = true;
            }
        }
    }
}
