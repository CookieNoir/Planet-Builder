using UnityEngine;

public static class PolarSystem
{

    public static Vector3 Position(float angle, in float radius, in Vector3 center)
    {
        angle *= Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3();

        newPosition.x = center.x + radius * Mathf.Cos(angle);
        newPosition.y = center.y + radius * Mathf.Sin(angle);
        newPosition.z = 0;

        return newPosition;
    }

    public static Vector2 Position(float angle, in float radius, in Vector2 center)
    {
        angle *= Mathf.Deg2Rad;
        Vector3 newPosition = new Vector2();

        newPosition.x = center.x + radius * Mathf.Cos(angle);
        newPosition.y = center.y + radius * Mathf.Sin(angle);

        return newPosition;
    }
}