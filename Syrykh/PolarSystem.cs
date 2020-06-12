using UnityEngine;

public class PolarSystem
{
    PolarSystem() { }

    Vector3 position(float angle, float radius, Vector3 center)
    {
        Vector3 newPosition = new Vector3();

        newPosition.x = (center.x + radius) * Mathf.Cos(angle);
        newPosition.y = (center.y + radius) * Mathf.Sin(angle);
        newPosition.z = 0;

        return newPosition;
    }
}
