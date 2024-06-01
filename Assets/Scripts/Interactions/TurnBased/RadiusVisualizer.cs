using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusVisualizer : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private float radius;
    [SerializeField] private int numPointsOnCircle;

    public void Start()
    {
        DrawRadius(transform.position);
    }

    public void DrawRadius(Vector3 center)
    {
        lr.positionCount = numPointsOnCircle + 1;
        float angle = 0f;
        for (int i = 0; i < numPointsOnCircle + 1; i++)
        {
            lr.SetPosition(i, new Vector3(center.x + Mathf.Sin(angle) * radius, center.y, center.z + Mathf.Cos(angle) * radius));

            angle += (Mathf.PI * 2) / numPointsOnCircle;
        }
    }
}
