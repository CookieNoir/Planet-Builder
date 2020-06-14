using UnityEngine;

public class HomeController : MonoBehaviour
{
    public BlockInfo homeInfo;
    public SpriteRenderer homeIcon;
    public GameObject explosion;
    [Range(3f, 5f)] public float speed = 5f;
    public Vector3 shipPosition;
    public AudioSource audioSource;
    public AudioClip clipLanding;
    public AudioClip clipDestroy;

    private float _t = 0f;
    private bool _canPlace = false;

    public void HomeBuild()
    {
        _canPlace = homeInfo.canPlace;

        transform.rotation = homeInfo.rotation;
        transform.localScale = new Vector3(homeInfo.scale, homeInfo.scale, homeInfo.scale);
    }

    public void ChangeColor(Color color)
    {
        homeIcon.color = color;
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
            if (audioSource) audioSource.PlayOneShot(clipLanding);
            GameController.ChangeBlocksLandingAmount(-1);
            GameController.CheckLevel();
        }
        else
        {
            if (audioSource) audioSource.PlayOneShot(clipDestroy);
            Instantiate(explosion, transform);
            GameController.Restart();
        }
    }
}
