#if CLIENT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HostMenu : MonoBehaviour {

    public GameObject canvas;
    public GameObject mainCanvas;

    public InputField seedInput;
    public InputField sizeInput;
    public InputField nameInput;

    public Button hostButton;
    public Button cancelButton;

    void Awake() {
        hostButton.onClick.AddListener(OnHost);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnHost() {
        GlobalStorage.seed = int.Parse(seedInput.text);
        GlobalStorage.size = int.Parse(sizeInput.text);
        GlobalStorage.localPlayerName = (string.IsNullOrEmpty(nameInput.text)) ? "Player" : nameInput.text;
        GlobalStorage.host = true;
        SceneManager.LoadScene("game");
    }

    private void OnCancel() {
        canvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

}
#endif