using UnityEngine;

public class GameController : MonoBehaviour
{
    public ShipController shipController;
    public Transform cameraTransform;
    public int level = 1;
    public int staticLevelsCount;
    public GameObject[] planets;

    public Vector2 offset;

    public static GameController instance;

    private Planet _planet;
    private bool _levelCompleted;
    private int _completedLevelsCount; // Количество завершенных уровней в текущей игровой сессии
    private Vector3 _newPlanetPosition;

    void Start()
    {
        _completedLevelsCount = 0;
        _newPlanetPosition = Vector3.zero;
        instance = this as GameController;
        instance.setNextPlanet();
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
        setNextPlanet();
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
        cameraTransform.position = _planet.transform.position - new Vector3(0, 0, 10);

        shipController.SetPlanet(_planet);
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
