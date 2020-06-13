using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public ShipController shipController;
    public Transform cameraTransform;
    public int level = 1;
    public int staticLevelsCount;
    public GameObject[] planets;

    public Vector2 offset;
    public float flightTime = 3f;

    public static GameController instance;

    private Planet _planet;
    private bool _levelCompleted;
    private int _completedLevelsCount; // Количество завершенных уровней в текущей игровой сессии
    private Vector3 _newPlanetPosition;
    private IEnumerator _moveToAnotherPlanet;
    private float _z;

    void Start()
    {
        instance = this as GameController;
        _completedLevelsCount = 0;
        _newPlanetPosition = Vector3.zero;
        setNextPlanet();
        _moveToAnotherPlanet = moveToAnotherPlanet();
        _z = cameraTransform.position.z;
        shipController.SetPlanet(_planet);
        // Прилет
        cameraTransform.position = _planet.transform.position + new Vector3(0, 0, _z);
    }

    public static void CheckLevel()
    {
        instance._levelCompleted = instance._planet.IsCompleted();
        if (instance._levelCompleted)
        {
            instance.changeLevel();
        }
    }

    public static void Restart()
    {
        instance._planet.Refresh();
    }

    private void changeLevel()
    {
        level++;
        _completedLevelsCount++;

        // Корабль движется к безопасному углу, вылетает за пределы экрана

        setNextPlanet();

        StopCoroutine(_moveToAnotherPlanet);
        _moveToAnotherPlanet = moveToAnotherPlanet();
        StartCoroutine(_moveToAnotherPlanet);
    }

    private IEnumerator moveToAnotherPlanet()
    {
        float _t = 0;
        Vector3 _startPosition = cameraTransform.position;
        Vector3 _endPosition = _planet.transform.position + new Vector3(0, 0, _z);
        while (_t < 1)
        {
            _t += Time.deltaTime / flightTime;
            cameraTransform.position = Vector3.Lerp(_startPosition,_endPosition,_t*_t*(3-2*_t));
            yield return null;
        }
        cameraTransform.position = _endPosition;
        shipController.SetPlanet(_planet);
        // Прилет
    }

    private void setNextPlanet()
    {
        GameObject planetObject;
        if (level <= staticLevelsCount)
        {
            planetObject = Instantiate(planets[level - 1], _newPlanetPosition, Quaternion.identity);
            _planet = planetObject.GetComponent<Planet>();
            _planet.Create();
        }
        else
        {
            int _type = Random.Range(0, planets.Length);
            planetObject = Instantiate(planets[_type], _newPlanetPosition, Quaternion.identity);
            _planet = planetObject.GetComponent<Planet>();
            _planet.Create(true);
        }
        _levelCompleted = false;

        if (_completedLevelsCount % 2 == 0)
        {
            _newPlanetPosition += new Vector3(offset.x, offset.y, 0);
        }
        else
        {
            _newPlanetPosition += new Vector3(-offset.x, offset.y, 0);
        }
    }
}
