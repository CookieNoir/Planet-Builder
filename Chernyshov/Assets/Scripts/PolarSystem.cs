using UnityEngine;

public class PolarSystem
{

    public static Vector3 Position(float angle, float radius, Vector3 center)
    {
        angle *= Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3();

        newPosition.x = (center.x + radius) * Mathf.Cos(angle);
        newPosition.y = (center.y + radius) * Mathf.Sin(angle);
        newPosition.z = 0;

        return newPosition;
    }
}
