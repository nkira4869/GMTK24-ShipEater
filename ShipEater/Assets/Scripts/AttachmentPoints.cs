using System.Collections.Generic;
using UnityEngine;

public class AttachmentPoints : MonoBehaviour
{
    public List<Transform> attachmentPoints = new List<Transform>(); // List of attachment points

    void OnDrawGizmos()
    {
        // Visualize the attachment points in the Unity editor
        Gizmos.color = Color.cyan;
        foreach (Transform point in attachmentPoints)
        {
            if (point != null)
            {
                Gizmos.DrawSphere(point.position, 0.1f); // Draw a small sphere at each attachment point
            }
        }
    }
}
