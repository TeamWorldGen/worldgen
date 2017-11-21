#if CLIENT
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LoadingScreen : MonoBehaviour {

    private static LoadingScreen instance;

    public GameObject loadingPanel;
    public Text title;
    public Text subtitle;

    private bool isLoading = false;

    void Awake() {
        instance = this;
    }

    void Update() {
        if (isLoading) {
            Color color = new Color(1, 1, 1, Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f);
            title.color = color;
        }
    }

    public static void Show() {
        LoadingScreen loadingScreen = LoadingScreen.instance;
        loadingScreen.loadingPanel.SetActive(true);
        loadingScreen.isLoading = true;
    }

    public static void Hide() {
        LoadingScreen loadingScreen = LoadingScreen.instance;
        loadingScreen.isLoading = false;
        loadingScreen.loadingPanel.SetActive(false);
    }

    public static void SetSubtitle(string text) {
        LoadingScreen loadingScreen = LoadingScreen.instance;
        loadingScreen.subtitle.text = text;
    }

    public static bool IsGameLoading {
        get {
            return LoadingScreen.instance.isLoading;
        }
    }

}
#endif