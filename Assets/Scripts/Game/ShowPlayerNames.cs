#if CLIENT
using UnityEngine;

[DisallowMultipleComponent]
public class ShowPlayerNames : MonoBehaviour {

    public float maxDistanceToShow = 5f;
    public GameObject canvas;
    public GameObject cameraObject;

    // TODO: This can probably be made more efficient

    void Update() {
        if (canvas.activeInHierarchy)
            canvas.SetActive(false);
    }

    void LateUpdate () {
        if (cameraObject != null) {

            RaycastHit hit;
            if (Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit)) {
                if (hit.transform.tag == "Player" && hit.distance < maxDistanceToShow) {
                    ShowPlayerNames other = hit.transform.GetComponent<ShowPlayerNames>();
                    other.showPlayerNameToTarget(transform);
                }
            }

        }
	}

    public void showPlayerNameToTarget(Transform target) {
        canvas.SetActive(true);
        canvas.transform.LookAt(target);
        canvas.transform.Rotate(0, 180, 0);
    }

}
#endif