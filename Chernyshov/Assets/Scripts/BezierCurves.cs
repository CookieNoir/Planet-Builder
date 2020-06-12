using UnityEngine;

public class BezierCurves
{
    public BezierCurves() { }

    static public Vector3 Lerp(Vector3 startPoint, Vector3 entPoint, float t)
    {
        return startPoint + (entPoint - startPoint) * t;
    }

    static public Vector3 QuadricLerp(Vector3 startPoint, Vector3 centerPoint, Vector3 endPoint, float t)
    {
        Vector3 p0 = Lerp(startPoint, centerPoint, t);
        Vector3 p1 = Lerp(centerPoint, endPoint, t);
        return Lerp(p0, p1, t);
    }
}
