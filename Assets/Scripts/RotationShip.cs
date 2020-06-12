using UnityEngine;

public class RotationShip : MonoBehaviour
{
    public Transform parent;
    public float speed = 60f;

    public GameObject Home;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject childHome = Instantiate(Home, gameObject.transform.position, Quaternion.identity);
            childHome.transform.SetParent(parent);
        }

        transform.RotateAround(parent.position, new Vector3(0.0f, 0.0f, -1.0f), Time.deltaTime * speed);
    }
}
