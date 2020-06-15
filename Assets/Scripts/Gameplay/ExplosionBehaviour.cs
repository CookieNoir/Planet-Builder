using UnityEngine;

public class ExplosionBehaviour : MonoBehaviour
{
    private float _scale;

    void Start()
    {
        _scale = 0;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        _scale += Time.deltaTime;
        transform.localScale = new Vector3(_scale, _scale, 1);
    }
}
