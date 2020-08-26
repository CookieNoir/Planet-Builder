using UnityEngine;

public class RotationTest : MonoBehaviour
{
    [Range(0f, 360f)] public float angle;
    public PlanetTest planet;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(planet.GetSegmentByAngle(angle));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 pos = PolarSystem.Position(angle, 2, transform.position);
        Gizmos.DrawLine(transform.position, pos);
        Gizmos.DrawSphere(pos, 0.2f);
    }
}
