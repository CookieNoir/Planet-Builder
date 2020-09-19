using UnityEngine;

public class Trees : GroundObject
{
    public override void Deploy(in Planet planet, in float angle)
    {
        segment = planet.GetSegmentByAngle(angle);
        Vector3 ls = transform.GetChild(0).localScale;
        ls *= Random.Range(0.7f, 1.3f);
        float value = Random.value;
        if (value > 0.5) ls = new Vector3(-ls.x, ls.y, ls.z);
        transform.GetChild(0).localScale = ls;
    }
}
