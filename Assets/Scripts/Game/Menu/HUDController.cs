using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour {

    public GameObject profilerPanel;
    private bool showProfiler = false;

    void Awake() {
        profilerPanel.SetActive(showProfiler);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.F1)) {
            showProfiler = !showProfiler;
            ShowProfiler(showProfiler);
        }

    }

    public void ShowProfiler(bool show) {
        profilerPanel.SetActive(show);
        showProfiler = show;
    }

}
