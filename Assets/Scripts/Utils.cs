using UnityEngine;

public class Utils
{
    public static float AngleDeg(Vector3 position, float radius, Vector3 center)
    {
        float x = position.x - center.x;
        float y = position.y - center.y;

        float angle = Mathf.Acos(x/radius);
        if (y < 0) angle *= -1;
        return angle*Mathf.Rad2Deg;
    }

    public static Vector3 StartingPointOfTheTangentEquation(Vector3 position, Vector3 center, float radiusInput, float radiusOutput, bool isClockwise, float additionalAngle=0f)
    {
        float angle = AngleDeg(position, radiusInput, center) + additionalAngle;
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
