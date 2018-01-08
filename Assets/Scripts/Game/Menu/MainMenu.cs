#if CLIENT
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject canvas;
    public GameObject hostCanvas;
    public GameObject connectCanvas;

    public Button hostButton;
    public Button connectButton;
    public Button settingsButton;
    public Button exitButton;

    void Awake() {
        canvas.SetActive(true);
        hostCanvas.SetActive(false);
        connectCanvas.SetActive(false);

        hostButton.onClick.AddListener(OnHost);
        connectButton.onClick.AddListener(OnConnect);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);
    }

    private void OnHost() {
        canvas.SetActive(false);
        hostCanvas.SetActive(true);
    }

    private void OnConnect() {
        canvas.SetActive(false);
        connectCanvas.SetActive(true);
    }

    private void OnSettings() {

    }

    private void OnExit() {
        Application.Quit();
    }

}
#endif