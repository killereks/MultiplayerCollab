using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour {

    [SerializeField] NetworkManager networkManager;

    private void Start() {
        if (!Application.isBatchMode) { // headless build
            Debug.Log("=== Client Starting ===");
            networkManager.StartClient();
        } else {
            Debug.Log("=== Server Starting ===");
        }
    }

    public void JoinLocal() {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

}
