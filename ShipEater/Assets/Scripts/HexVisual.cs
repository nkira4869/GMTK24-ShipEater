using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HexVisual : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float radius = 1f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 7; // 6 sides plus the closing line
        DrawHexagon();
    }

    void DrawHexagon()
    {
        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i;
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            Vector3 position = new Vector3(radius * Mathf.Cos(angle_rad), radius * Mathf.Sin(angle_rad), 0);
            lineRenderer.SetPosition(i, position);
        }
        lineRenderer.SetPosition(6, lineRenderer.GetPosition(0)); // Close the hexagon
    }
}