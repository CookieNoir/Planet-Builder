using UnityEngine;

public class HomeController : MonoBehaviour
{
    public BlockInfo homeInfo;
    [Range(3f, 5f)] public float speed = 5f;
    public Vector3 shipPosition;

    private float _t = 0f;
    private bool _canPlace = false;

    public void HomeBuild()
    {
        _canPlace = homeInfo.canPlace;

        transform.rotation = homeInfo.rotation;
        transform.localScale = new Vector3(homeInfo.scale, homeInfo.scale, homeInfo.scale);
    }

    private void Update()
    {
        if (_t >= 1f)
        {
            transform.position = homeInfo.position;
            Destroy(this);
        }
        else
        {
            _t += Time.deltaTime * speed;
            transform.position = BezierCurves.Lerp(shipPosition, homeInfo.position, _t);
        }
    }

    private void OnDestroy()
    {
        if (_canPlace)
        {
            GameController.CheckLevel();
        }
        else
        {
            GameController.Restart();
        }
    }
}
