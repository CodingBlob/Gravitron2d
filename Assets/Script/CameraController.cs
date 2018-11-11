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
        UpdatePositionFromKeys();
        UpdatePositionFromMouse();
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPos, 0.4f);
    }

    private void UpdatePositionFromKeys()
    {
        var keyOffset = 5f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetCameraPos.x -= keyOffset;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            targetCameraPos.x += keyOffset;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            targetCameraPos.y += keyOffset;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            targetCameraPos.y -= keyOffset;
        }
    }

    private void UpdatePositionFromMouse()
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
    }

    private void UpdateZoom()
    {
        zoomFactor = Mathf.Clamp(zoomFactor - Input.GetAxis("Mouse ScrollWheel") * 100f, 10, 100);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomFactor, 0.2f);
    }
}
