using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SerializeField]
    private Text playerNameText;

    [SyncVar(hook = "OnNameChange")]
    private string playerName;
    public string PlayerName {
        get { return playerName; }
    }

    private void OnNameChange(string playerName) {
        playerNameText.text = playerName;
    }

#if CLIENT

    [Header("Player Settings")]
    public float walkingSpeed = 10.0f;
    public float runningSpeed = 15.0f;
    public float jumpForce = 5.0f;

    [Header("Other Settings")]
    public float maxDistanceToShowName = 5f;
    public GameObject cameraPrefab;

    private GameObject cameraObject;
    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        if (isLocalPlayer) {
            // Add the camera
            cameraObject = (GameObject)Instantiate(cameraPrefab, transform);
            cameraObject.transform.localPosition = new Vector3(0, 0.5f, 0);

            GameController.Instance.EnableMainCamera(false);
            GameController.Instance.Player = this;
            UserInput.Instance.Player = this;
            //CmdSetPlayerName(NetworkController.Instance.localPlayerName);
            CmdSetPlayerName(GlobalStorage.localPlayerName);

            // Add MouseLook component if this is the local player
            MouseLook ml = gameObject.AddComponent<MouseLook>();
            ml.lockCursor = true;

            WorldManager.Instance.player = transform;

        } else {
            Destroy(cameraObject); // We only need the camera on the local player
        }

        // Add ShowPlayerNames component
        ShowPlayerNames spn = gameObject.AddComponent<ShowPlayerNames>();
        spn.canvas = transform.Find("PlayerNameCanvas").gameObject;
        spn.maxDistanceToShow = maxDistanceToShowName;
        spn.cameraObject = cameraObject;

    }

    void Update () {

        if (!isLocalPlayer)
            return;

	}

    public void Move(float x, float z, bool jump, bool run) {
        float xSpeed = x * walkingSpeed * Time.fixedDeltaTime;
        float zSpeed = z * (run ? runningSpeed : walkingSpeed) * Time.fixedDeltaTime;
        transform.Translate(xSpeed, 0, 0);
        transform.Translate(0, 0, zSpeed);
        if (jump && IsGrounded()) {
            rb.AddForce(Vector3.up * jumpForce * rb.mass, ForceMode.Impulse);
        }
    }

    public bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, 1.1f);
    }

    void OnDestroy() {
        if (isLocalPlayer && GameController.Instance)
            GameController.Instance.EnableMainCamera(true);
    }
#endif

    [Command]
    private void CmdSetPlayerName(string playerName) {
        this.playerName = playerName;
    }

}
