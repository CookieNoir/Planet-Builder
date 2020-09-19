using UnityEngine;

public class GroundObject : MonoBehaviour, IInteractable
{
    protected int segment;

    public virtual void Deploy(in Planet planet, in float angle)
    {
        segment = planet.GetSegmentByAngle(angle);
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted with object on segment " + segment);
    }
}
