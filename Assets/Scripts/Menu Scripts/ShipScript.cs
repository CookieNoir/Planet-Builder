using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    private float timeCounter = 0;
    private float shipSpeed = 2f;
    private float shipHeight = 2.19f;
    private float centerX = 0f;
    private float centerY = 0f;

    private void FixedUpdate()
    {
        timeCounter += Time.deltaTime * shipSpeed;
        float x = Mathf.Cos(timeCounter) * shipHeight + centerX;
        float y = Mathf.Sin(timeCounter) * shipHeight + centerY;
        float z = 0;
        transform.position = new Vector3(x, y, z);
    }
}
