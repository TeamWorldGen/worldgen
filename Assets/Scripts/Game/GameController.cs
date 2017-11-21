#if CLIENT
using UnityEngine;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour {

    public GameObject mainCameraObject;

    private static GameController instance;
    public static GameController Instance {
        get { return instance; }
    }

    public delegate void GameEventDelegate(GameEvent e);
    public event GameEventDelegate OnGameEvent;

    private Player player;
    public Player Player {
        get { return player; }
        set { player = value; }
    }

    void Awake() {
        instance = this;
    }

    public void EnableMainCamera(bool enabled) {
        if (mainCameraObject != null)
            mainCameraObject.SetActive(enabled);
    }

    public void LaunchEvent(GameEvent e) {
        if (OnGameEvent != null)
            OnGameEvent(e);
    }

}

public enum GameEvent {
    WorldCreated
}
#endif