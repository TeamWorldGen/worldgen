#if CLIENT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectMenu : MonoBehaviour {

    public GameObject canvas;
    public GameObject mainCanvas;

    public InputField addressInput;
    public InputField portInput;
    public InputField nameInput;

    public Button connectButton;
    public Button cancelButton;

    void Awake() {
        connectButton.onClick.AddListener(OnConnect);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnConnect() {
        GlobalStorage.connectAddress = addressInput.text;
        GlobalStorage.connectPort = int.Parse(portInput.text);
        GlobalStorage.localPlayerName = (string.IsNullOrEmpty(nameInput.text)) ? "Player" : nameInput.text;
        SceneManager.LoadScene("game");
    }

    private void OnCancel() {
        canvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

}
#endif
