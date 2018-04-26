using UnityEngine;

public class Parallaxing : MonoBehaviour {
    public Transform[] backgrounds;
    private float[] parallaxScales;
    public float smoothing = 1f;

    private Transform mainCam;
    private Vector3 prevCamPos;

    void Awake() {
        mainCam = Camera.main.transform;
    }

    void Start () {
        prevCamPos = mainCam.position;
        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++) {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
	}
	
	void Update () {
        // Don't perform parallaxing when the camera jumps too much
        if (Mathf.Abs(prevCamPos.x - mainCam.position.x) > 5) {
            prevCamPos = mainCam.position;
            return;
        }
        
        // Perform paralaxing
        for (int i = 0; i < backgrounds.Length; i++) {
            float parallax = (prevCamPos.x - mainCam.position.x) * parallaxScales[i];
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }
        prevCamPos = mainCam.position;
    }
}
