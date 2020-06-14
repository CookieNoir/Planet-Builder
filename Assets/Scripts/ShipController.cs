using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour
{
    public SpriteRenderer ship;
    public SpriteRenderer laser;
    public GameObject homeBlock;
    [Range(5f, 20f)] public float maxSpeed = 10f;

    private float _speed = 60f;
    private Planet _planet;
    private IEnumerator _smoothFlight;
    private IEnumerator _laserModifier;
    private bool _onPlanet = false;
    private Color _alpha;

    private void Start()
    {
        _alpha = new Color(0,0,0,1);
        _smoothFlight = smoothFlightTo(Vector3.zero, Vector3.zero, 0f, 0f, 0f, false);
        _laserModifier = laserModifier(false);
        laser.enabled = false;
        changeLaserVisibility(false);
    }

    private IEnumerator smoothFlightTo(Vector3 startPoint, Vector3 endPoint, float pathLength, float speed, float speedStart, bool isLanding)
    {
        if (speedStart < speed) speedStart = speed;
        float _pathCompleted = 0;
        float _pathLengthReversed = 1f / pathLength;
        float _curSpeed = speedStart;
        Color _col = new Color(ship.color.r, ship.color.g, ship.color.b, 0);
        while (_pathCompleted < 1)
        {
            yield return null;
            _pathCompleted += _curSpeed * Time.deltaTime * _pathLengthReversed;
            transform.position = Vector3.Lerp(startPoint, endPoint, _pathCompleted);
            _curSpeed = Mathf.Lerp(speedStart, speed, _pathCompleted);
            ship.color = _col + _alpha * _pathCompleted;
        }
        transform.position = endPoint;
        ship.color = _col + _alpha;
        changeLaserVisibility(isLanding);
    }

    private IEnumerator smoothFlightFrom(Vector3 startPoint, Vector3 endPoint, float pathLength, float speed, float speedEnd, bool isLanding)
    {
        if (speedEnd < speed) speedEnd = speed;
        changeLaserVisibility(isLanding);
        float _pathCompleted = 0;
        float _pathLengthReversed = 1f / pathLength;
        float _curSpeed = speed;
        Color _col = new Color(ship.color.r, ship.color.g, ship.color.b, 0);
        while (_pathCompleted < 1)
        {
            yield return null;
            _pathCompleted += _curSpeed * Time.deltaTime * _pathLengthReversed;
            transform.position = Vector3.Lerp(startPoint, endPoint, _pathCompleted);
            _curSpeed = Mathf.Lerp(speed, speedEnd, _pathCompleted);
            ship.color = _col + _alpha * (1 - _pathCompleted);
        }
        transform.position = endPoint;
        ship.color = _col;
    }

    private IEnumerator laserModifier(bool isTurningOn)
    {
        float _t = 0f;
        Color _col = new Color(laser.color.r, laser.color.g, laser.color.b, 0);
        if (isTurningOn)
        {
            laser.enabled = true;
            laser.color = _col + Color.clear;
            laser.transform.localScale = Vector3.zero;
            while (_t < 1f)
            {
                yield return null;
                _t += Time.deltaTime;
                laser.color = _col + _alpha * _t;
                laser.transform.localScale = Vector3.one * _t;
            }
            laser.color = _col + _alpha;
            laser.transform.localScale = Vector3.one;
        }
        else
        {
            laser.color = _col + _alpha;
            laser.transform.localScale = Vector3.one;
            while (_t < 1f)
            {
                yield return null;
                _t += Time.deltaTime;
                laser.color = _col + _alpha * (1 - _t);
                laser.transform.localScale = Vector3.one * (1 - _t);
            }
            laser.color = _col;
            laser.transform.localScale = Vector3.zero;
            laser.enabled = false;
        }
    }

    public void ToPlanet(Planet planet, float pathLength)
    {
        Vector3 endPoint = SetPlanet(planet);
        Vector3 startPoint = Utils.StartingPointOfTheTangentEquation(planet.transform.position, endPoint, planet.planetAtmosphereRadius, pathLength, planet.isClockwise);
        float _speedLinear = (Mathf.PI * _speed * planet.planetAtmosphereRadius) / 180f;
        ship.flipX = planet.isClockwise;
        StopCoroutine(_smoothFlight);
        _smoothFlight = smoothFlightTo(startPoint, endPoint, pathLength, _speedLinear, maxSpeed, true);
        StartCoroutine(_smoothFlight);
    }

    public void FromPlanet(float pathLength)
    {
        Vector3 startPoint = transform.position;
        Vector3 endPoint = Utils.StartingPointOfTheTangentEquation(_planet.transform.position, startPoint, _planet.planetAtmosphereRadius, pathLength, !_planet.isClockwise);
        float _speedLinear = (Mathf.PI * _speed * _planet.planetAtmosphereRadius) / 180f;
        StopCoroutine(_smoothFlight);
        _smoothFlight = smoothFlightFrom(startPoint, endPoint, pathLength, _speedLinear, maxSpeed, false);
        StartCoroutine(_smoothFlight);
    }

    public Vector3 SetPlanet(Planet planet)
    {
        _planet = planet;
        _speed = planet.shipSpeed;
        transform.rotation = Quaternion.Euler(0, 0, _planet.startAngle);
        return PolarSystem.Position(_planet.startAngle, _planet.planetAtmosphereRadius, _planet.transform.position);
    }

    private void changeLaserVisibility(bool isLanding)
    {
        _onPlanet = isLanding;
        StopCoroutine(_laserModifier);
        _laserModifier = laserModifier(_onPlanet);
        StartCoroutine(_laserModifier);
    }

    void Update()
    {
        if (_onPlanet)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float angleZ = transform.rotation.eulerAngles.z;
                BlockInfo info = _planet.AddBlock(angleZ);

                GameObject childHome = Instantiate(homeBlock, gameObject.transform.position, Quaternion.identity);

                HomeController homeController = childHome.GetComponent<HomeController>();
                homeController.homeInfo = info;
                homeController.shipPosition = transform.position;
                homeController.HomeBuild();

                childHome.transform.SetParent(_planet.transform);
                _planet.addBlockToBlocksList(childHome);
            }

            if (_planet.isClockwise)
                transform.RotateAround(_planet.transform.position, new Vector3(0.0f, 0.0f, -1.0f), Time.deltaTime * _speed);
            else
                transform.RotateAround(_planet.transform.position, new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * _speed);
        }
    }
}
