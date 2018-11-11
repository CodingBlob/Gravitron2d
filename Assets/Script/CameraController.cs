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
        UpdateZoom();

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.MiddleMouse))
        {
            var pos = Input.mousePosition;
            lastMousePos = pos;
        }

        if (Input.GetMouseButton((int)MouseButton.MiddleMouse))
        {
            var pos = Input.mousePosition;

            var mouseDelta = (pos - lastMousePos) / 10f;

            targetCameraPos -= mouseDelta;

            lastMousePos = pos;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPos, 0.2f);
    }

    private void UpdateZoom()
    {
        zoomFactor = Mathf.Clamp(zoomFactor - Input.GetAxis("Mouse ScrollWheel") * 100f, 10, 100);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomFactor, 0.2f);
    }
}
