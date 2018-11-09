using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class CameraController : MonoBehaviour
{
    private float zoomFactor;

    private Vector3 lastMousePos;
    private Vector3 targetCameraPos;

    void Start()
    {
        zoomFactor = Camera.main.orthographicSize;

        targetCameraPos = Camera.main.transform.position;
    }

    void Update()
    {
        zoomFactor -= Input.GetAxis("Mouse ScrollWheel") * 100f;

        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomFactor, 0.1f);

        if (Input.GetMouseButton((int)MouseButton.MiddleMouse))
        {
            var pos = Input.mousePosition;

            var mouseDelta = (pos - lastMousePos) / 10f;

            if (mouseDelta.magnitude < 10)
            {
                targetCameraPos -= mouseDelta; 
            }

            lastMousePos = pos;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPos, 0.1f);
    }
}
