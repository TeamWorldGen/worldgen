#if CLIENT
using UnityEngine;

[DisallowMultipleComponent]
public class MouseLook : MonoBehaviour {

    public bool enabled = true;

    private static MouseLook instance;
    public static MouseLook Instance {
        get { return instance; }
    }

    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    private GameObject camObject;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Start() {
        // Find the camera assigned to this player object
        foreach (Transform child in transform) {
            if (child.GetComponent<Camera>() != null) {
                camObject = child.gameObject;
                break;
            }
        }
        if (camObject == null) {
            Debug.LogError("No camera was found for player object.");
            return;
        }
        // Set target direction to the camera's initial orientation.
        targetDirection = camObject.transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        targetCharacterDirection = transform.localRotation.eulerAngles;
    }

    void Update() {

        if (!enabled)
            return;

        if (camObject == null)
            return;

        // Ensure the cursor is always locked when set
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Allow the script to clamp based on a desired target value.
        Quaternion targetOrientation = Quaternion.Euler(targetDirection);
        Quaternion targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        // Rotate player object and camera
        camObject.transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;
        Quaternion yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
        transform.localRotation = yRotation * targetCharacterOrientation;
    }

}
#endif