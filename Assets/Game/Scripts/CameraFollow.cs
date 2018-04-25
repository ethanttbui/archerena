using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform playerTransform = null;
    public int depth = -20;

    public void setTarget(Transform target) {
        playerTransform = target;
    }

    void Update() {
        if (playerTransform != null) {
            transform.position = playerTransform.position + new Vector3(0, 5, depth);
        }
    }
}
