using UnityEngine;

public class RotationShip : MonoBehaviour
{
    public Transform parent;
    public float speed = 1.5f;

    void Update()
    {
        transform.RotateAround(parent.position, new Vector3(0.0f, 0.0f, -1.0f), 100 * Time.deltaTime * speed);
    }
}
