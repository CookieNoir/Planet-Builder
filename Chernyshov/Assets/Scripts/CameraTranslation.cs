using UnityEngine;

public class CameraTranslation : MonoBehaviour
{
    public bool isTranslation = false;
    public float speed = 0.79f;
    public float offsetFromPlanetZ = 2f;
    public Transform start;
    public Transform end;

    public enum TranslationType
    {
        Backward,
        Up,
        Down
    };

    public TranslationType type;

    private BezierCurves m_curses;
    private Camera m_camera;
    private float m_fit = 0f;

    private Vector3 centroid;
    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        m_camera = GetComponent<Camera>();

        centroid = (start.position + end.position) * .5f;

        if (type == TranslationType.Backward)
            centroid.z -= 5;
        if (type == TranslationType.Up)
            centroid.y += 5;
        if (type == TranslationType.Down)
            centroid.y -= 5;

        startPosition = start.transform.position;
        endPosition = end.transform.position;

        startPosition.z -= offsetFromPlanetZ;
        endPosition.z -= offsetFromPlanetZ;
    }

    void Update()
    {
        if (isTranslation)
        {
            if (m_fit >= 1f)
            {
                isTranslation = false;
                return;
            }

            m_fit += Time.deltaTime * speed;
            m_camera.transform.position = BezierCurves.QuadricLerp(startPosition, centroid, endPosition, m_fit);
        }
    }
}
