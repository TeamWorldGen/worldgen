#if CLIENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    void Start() {
    }

    void Update () {
        transform.LookAt(GameController.Instance.Player.transform.position, Vector3.up);
	}

}
#endif