using UnityEngine;

public class Utils
{
    public static float AngleDeg(float xPosition, float yPosition)
    {
        return Mathf.Atan2(xPosition, yPosition) * Mathf.Rad2Deg;
    }

    public static Vector3 StartingPointOfTheTangentEquation(Vector3 center, float radiusInput, float radiusOutput)
    {
        float angle = AngleDeg(center.x, radiusInput) + 90f;
        return PolarSystem.Position(angle, radiusOutput, center);
    }

    public static Vector3 StartingPointOfTheTangentEquation(Vector3 center, float radiusInput, float radiusOutput, bool isClockwise)
    {
        float angle = AngleDeg(center.x, radiusInput);
        if (isClockwise)
        {
            angle -= 90f;
        }
        else
        {
            angle += 90f;
        }
        return PolarSystem.Position(angle, radiusOutput, center);
    }
}
