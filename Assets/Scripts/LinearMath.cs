using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Math utility functions
public class LinearMath : MonoBehaviour
{
    // Method to calculate the 2D cross product between a point given a line, given by 2 points
    // Assumes that the y-coordinate of the points is not relevant, meaning that 3D points are flattened to the XZ plane
    //  Read more about this here: https://stackoverflow.com/questions/243945/calculating-a-2d-vectors-cross-product
    public static float Cross2D(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 p = new Vector3(point.x, point.z, 0);
        Vector3 s = new Vector3(lineStart.x, lineStart.z, 0);
        Vector3 e = new Vector3(lineEnd.x, lineEnd.z, 0);

        Vector3 cross = Vector3.Cross(p - s, e - s);

        return cross.z;
    }

    // Method to calculate the 2D distance between two points, flattened from 3D onto the XZ plane
    public static float Distance2D(Vector3 start, Vector3 end)
    {
        Vector3 leveledStart = new Vector3(start.x, 0f, start.z);
        Vector3 leveledEnd = new Vector3(end.x, 0f, end.z);

        return (leveledEnd - leveledStart).magnitude;
    }

    // Method to move a point on level with another point on the XZ plane.
    // Used for rotation purposes, effectively just adjusts the Y position of the second point to match the first points height.
    public static Vector3 GetSameHeightPoint(Vector3 desiredLocation, Vector3 targetLocation)
    {
        return new Vector3(targetLocation.x, desiredLocation.y, targetLocation.z);
    }
}
