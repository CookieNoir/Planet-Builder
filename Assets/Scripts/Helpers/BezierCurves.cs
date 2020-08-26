using UnityEngine;

public static class BezierCurves
{
    static public Vector3 Lerp(in Vector3 startPoint, in Vector3 entPoint, in float t)
    {
        return startPoint + (entPoint - startPoint) * t;
    }

    static public Vector3 QuadraticLerp(in Vector3 startPoint, in Vector3 centerPoint, in Vector3 endPoint, in float t)
    {
        return Lerp(Lerp(startPoint, centerPoint, t), Lerp(centerPoint, endPoint, t), t);
    }
}
