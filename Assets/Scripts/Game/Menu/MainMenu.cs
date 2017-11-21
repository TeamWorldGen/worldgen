#if CLIENT
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject canvas;
    public GameObject connectCanvas;

    public Button startButton;
    public Button settingsButton;
    public Button exitButton;

    void Awake() {
        canvas.SetActive(true);
        connectCanvas.SetActive(false);

        startButton.onClick.AddListener(OnStart);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);
    }

    private void OnStart() {
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