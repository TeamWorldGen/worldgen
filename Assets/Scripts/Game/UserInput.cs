#if CLIENT
using UnityEngine;

[DisallowMultipleComponent]
public class UserInput : MonoBehaviour {

    private static UserInput instance;
    public static UserInput Instance {
        get { return instance; }
    }

    private Player player;
    public Player Player {
        get { return player; }
        set { player = value; }
    }

    private float x, z;
    private bool jump = false;
    private bool run = false;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Update() {

        // Do nothing if the game is loading
        if (LoadingScreen.IsGameLoading)
            return;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        run = Input.GetButton("Run");
        if (Input.GetButtonDown("Jump"))
            jump = true;

        if (Input.GetKeyDown(KeyCode.Escape))
            MouseLook.Instance.lockCursor = false;
        if(Input.GetKeyDown(KeyCode.E)) { // Only for testing
            MouseLook.Instance.enabled = !MouseLook.Instance.enabled;
            player.SetKinematic(!MouseLook.Instance.enabled);
        }

        if (Input.GetKeyDown(KeyCode.Return))
            CustomNetworkManager.Instance.SpawnPlayer();
        if (Input.GetKeyDown(KeyCode.Backspace))
            CustomNetworkManager.Instance.KillPlayer();


    }

    void FixedUpdate() {

        if (player == null)
            return;

        player.Move(x, z, jump, run);
        jump = false;
        run = false;
    }

}
#endif