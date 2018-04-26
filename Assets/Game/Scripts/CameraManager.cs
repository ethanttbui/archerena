using UnityEngine;

public class CameraManager : MonoBehaviour {
    public int depth = -30;
    public float aspectRatio = 16 / 9f;
    Transform playerTransform = null;
    Camera cam;

    public void SetTarget(Transform target) {
        playerTransform = target;
    }

    void Start() {
        // Set the pixel rectangle to make sure that all player has the 
        // exact same view of the world, regardless of the aspect ratio
        cam = GetComponent<Camera>();
        var variance = aspectRatio / cam.aspect;
        if (variance < 1f)
            cam.rect = new Rect((1f - variance) / 2f, 0, variance, 1f);
        else {
            variance = 1f / variance;
            cam.rect = new Rect(0, (1f - variance) / 2f, 1f, variance);
        }
    }

    void Update() {
        // Set the camera to follow the player
        if (playerTransform != null) {
            transform.position = playerTransform.position + new Vector3(0, 5, depth);
        }
    }
}
