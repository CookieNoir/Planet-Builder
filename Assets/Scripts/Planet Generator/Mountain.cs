using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class Mountain : GroundObject
{
    public override void Deploy(in Planet planet, in float angle)
    {
        segment = planet.GetSegmentByAngle(angle);
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Color[] colors = mesh.colors;
        for (int i = 0; i < colors.Length; ++i)
        {
            if (colors[i].r > 0.6f) colors[i] = planet.reliefColorHigher * 1.3f;
            else if (colors[i].r < 0.4f) colors[i] = planet.reliefColorHigher * 0.7f;
            else colors[i] = planet.reliefColorHigher;
        }
        mesh.colors = colors;
    }

    public override void Interact()
    {
        Debug.Log("Interacted with mountain on segment " + segment);
    }
}
