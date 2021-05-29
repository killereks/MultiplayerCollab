using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour {


    public MenuData[] menuData;

    public string startingMenu;

    private void Start() {
        OpenMenu(startingMenu);
    }

    [System.Serializable]
    public class MenuData {
        public GameObject targetObject;
        public string name;
    }

    public void OpenMenu(string menuName) {
        foreach (MenuData data in menuData) {
            data.targetObject.SetActive(data.name == menuName);
        }
    }
}
