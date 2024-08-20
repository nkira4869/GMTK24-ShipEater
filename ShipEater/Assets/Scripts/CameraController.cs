using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField] private Color boundsGizmoColor = Color.red;

    private Camera cam;

    [Header("Zoom Settings")]
    public float minZoom = 5f; // Closest zoom level
    public float maxZoom = 15f; // Farthest zoom level
    public float zoomSpeed = 2f; // Speed at which the camera zooms in/out

    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController script must be attached to a Camera.");
        }
    }

    void Update()
    {
        ClampCameraPosition();
    }

    public void UpdateCameraZoom(int currentLevel, int totalLevels)
    {
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, (float)(currentLevel - 1) / (totalLevels - 1));
        StartCoroutine(SmoothZoom(targetZoom));
    }

    IEnumerator SmoothZoom(float targetZoom)
    {
        while (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.1f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            yield return null;
        }

        cam.orthographicSize = targetZoom; // Ensure the final zoom level is set precisely
    }

    void ClampCameraPosition()
    {
        Vector3 pos = cam.transform.position;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + horzExtent, maxBounds.x - horzExtent);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + vertExtent, maxBounds.y - vertExtent);

        cam.transform.position = pos;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = boundsGizmoColor;
        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}