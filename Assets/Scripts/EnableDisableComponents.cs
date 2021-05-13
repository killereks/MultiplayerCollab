using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnableDisableComponents : NetworkBehaviour {

    public GameObject[] objectsToDisableIfMine;
    public GameObject[] objectsToEnableIfMine;

    private void Start() {
        if (isLocalPlayer) {
            foreach (GameObject obj in objectsToDisableIfMine) {
                obj.SetActive(false);
            }
            foreach (GameObject obj in objectsToEnableIfMine) {
                obj.SetActive(true);
            }
        }
    }

}
