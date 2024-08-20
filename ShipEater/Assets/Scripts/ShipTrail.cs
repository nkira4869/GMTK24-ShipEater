using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShipTrail : MonoBehaviour
{

    public int length;
    public LineRenderer lr;

    public Vector3[] segmentPos;
    private Vector3[] segmentVelocity;

    public Transform targetDir;
    public float targetDist;

    public float smoothSpeed;
    public float trailSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;

    void Start()
    {

        lr.positionCount = length;
        segmentPos = new Vector3[length];
        segmentVelocity = new Vector3[length];
    }

    void Update()
    {

        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);

        segmentPos[0] = targetDir.position;

        for (int i = 1; i < segmentPos.Length; i++)
        {
            segmentPos[i] = Vector3.SmoothDamp(segmentPos[i], segmentPos[i - 1] + targetDir.right * targetDist, ref segmentVelocity[i], smoothSpeed + i / trailSpeed);
        }

        lr.SetPositions(segmentPos);
    }
}