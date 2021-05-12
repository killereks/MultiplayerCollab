using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour {

    [SerializeField] NetworkManager networkManager;

    public void JoinLocal() {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

}
