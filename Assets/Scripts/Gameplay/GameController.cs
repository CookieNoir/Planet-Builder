﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public ShipController shipController;
    public Camera gameCamera;
    public AlphaChanger whiteScreen;
    public AlphaChanger toMenuButton;

    public Text levelText;
    public Text targetText;
    public Text heightText;

    public Hint hintSequence;

    public UiMovement levelBoard;
    public UiMovement targetBoard;
    public UiMovement heightBoard;

    public int level = 1;
    public int staticLevelsCount;
    public GameObject[] planets;

    public Vector2 offset;
    [Range(4f, 12f)] public float flightTime = 3f;
    [Range(1f, 3f)] public float zoomTime = 2f;
    public Vector2 cameraSize;
    public float cameraSizeAtStart = 7f;
    public float shipPathLength = 10f;

    public GameObject trail;

    public static GameController instance;

    private PlanetOld _planet;
    private GameObject _prevPlanet;
    private bool _levelCompleted;
    private int _completedLevelsCount; // Количество завершенных уровней в текущей игровой сессии
    private Vector3 _newPlanetPosition;
    private IEnumerator _moveToAnotherPlanet;
    private IEnumerator _zoomAtStart;
    private IEnumerator _restart;
    private IEnumerator _changeAlpha;
    private bool _restarting;
    private int _blocksLanding;

    void Start()
    {
        instance = this as GameController;
        level = PlayerPrefs.GetInt("Level", 1);
        _completedLevelsCount = 0;
        _newPlanetPosition = Vector3.zero;
        setNextPlanet();
        _newPlanetPosition = new Vector3(offset.x, offset.y, 0);
        _moveToAnotherPlanet = moveToAnotherPlanet();
        _zoomAtStart = zoomAtStart();
        _restart = restart();
        whiteScreen.SetAlpha(1f);
        _changeAlpha = whiteScreen.ChangeAlpha(SimpleFunctions.SmoothStep, true);
        StartCoroutine(_changeAlpha);
        _restarting = false;
        gameCamera.orthographicSize = cameraSizeAtStart;
    }

    public void StartGame()
    {
        StopCoroutine(_zoomAtStart);
        _zoomAtStart = zoomAtStart();
        StartCoroutine(_zoomAtStart);
    }

    private IEnumerator zoomAtStart()
    {
        float _t = 0;
        float _t2 = 0;
        while (_t < zoomTime)
        {
            yield return null;
            _t += Time.deltaTime;
            _t2 = _t / zoomTime;
            gameCamera.orthographicSize = Mathf.Lerp(cameraSizeAtStart, cameraSize.x, Mathf.Sqrt(_t2 * _t2 * (3 - 2 * _t2)));
        }
        gameCamera.orthographicSize = cameraSize.x;
        shipController.ToPlanet(_planet, shipPathLength);
        hintSequence.ShowHint();
        inGameUiTranslate(true);
    }

    private void inGameUiTranslate(bool isLanding = false)
    {
        levelBoard.Translate();
        targetBoard.Translate();
        heightBoard.Translate();
        if (isLanding)
        {
            levelText.text = "уровень " + level;
            targetText.text = "построено " + 0 + " из " + instance._planet.GetBuildingsToWin().ToString();
            heightText.text = "макс. высота - " + _planet.height.ToString();
        }
    }

    private void refreshTargetText()
    {
        targetText.text = "построено " + instance._planet.GetBuildingsCompleted().ToString() + " из " + instance._planet.GetBuildingsToWin().ToString();
    }

    public static void CheckLevel()
    {
        if (instance)
        {
            instance._levelCompleted = instance._planet.IsCompleted();
            instance.refreshTargetText();
            if (instance._levelCompleted && instance._blocksLanding == 0)
            {
                instance.changeLevel();
            }
        }
    }

    public static void ChangeBlocksLandingAmount(int value)
    {
        if (instance) instance._blocksLanding += value;
    }

    public static void Restart()
    {
        if (instance) instance.restartGC();
    }

    private void restartGC()
    {
        if (!_restarting && _blocksLanding > 0)
        {
            _restarting = true;
            StopCoroutine(_restart);
            _restart = restart();
            StartCoroutine(_restart);
        }
    }

    private IEnumerator restart()
    {
        float _t = 0;
        shipController.ChangeActivity(false);
        while (_t < 1f)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        whiteScreen.gameObject.SetActive(true);
        StopCoroutine(_changeAlpha);
        _changeAlpha = whiteScreen.ChangeAlpha();
        StartCoroutine(_changeAlpha);
        _t = 0;
        while (_t < whiteScreen.increasingDuration)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        _t = 0;
        while (_t < 0.25f)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        _planet.Refresh();
        refreshTargetText();
        shipController.SetStartPosition();
        _t = 0;
        StopCoroutine(_changeAlpha);
        _changeAlpha = whiteScreen.ChangeAlpha(SimpleFunctions.SmoothStep);
        StartCoroutine(_changeAlpha);
        shipController.ChangeActivity(true);
        _blocksLanding = 0;
        _restarting = false;
        while (_t < whiteScreen.decreasingDuration)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        whiteScreen.gameObject.SetActive(false);
    }

    private void changeLevel()
    {
        level++;
        _completedLevelsCount++;
        PlayerPrefs.SetInt("Level", level);
        setNextPlanet();
        StopCoroutine(_moveToAnotherPlanet);
        _moveToAnotherPlanet = moveToAnotherPlanet();
        StartCoroutine(_moveToAnotherPlanet);
    }

    private IEnumerator moveToAnotherPlanet()
    {
        shipController.FromPlanet(shipPathLength);
        float _t = 0;
        float _t2 = 0;
        float _tf = 0;
        float _f = 1f / flightTime;
        Vector3 _startPosition = gameCamera.transform.position;
        Vector3 _endPosition = gameCamera.transform.position + _newPlanetPosition;
        trail.SetActive(true);
        inGameUiTranslate();
        StopCoroutine(_zoomAtStart); // Исп-тся _zoomAtStart вместо _changeAlpha, т.к. в момент перехода она однозначно свободна
        _zoomAtStart = toMenuButton.ChangeAlpha();
        StartCoroutine(_zoomAtStart);
        while (_t < zoomTime)
        {
            yield return null;
            _t += Time.deltaTime;
            _t2 = _t / zoomTime;
            _tf += Time.deltaTime * _f;
            gameCamera.orthographicSize = Mathf.Lerp(cameraSize.x, cameraSize.y, Mathf.Sqrt(_t2 * _t2 * (3 - 2 * _t2)));
            gameCamera.transform.position = Vector3.Lerp(_startPosition, _endPosition, _tf * _tf * (3 - 2 * _tf));
        }
        gameCamera.orthographicSize = cameraSize.y;
        while (_t < flightTime - zoomTime)
        {
            yield return null;
            _t += Time.deltaTime;
            _tf += Time.deltaTime * _f;
            gameCamera.transform.position = Vector3.Lerp(_startPosition, _endPosition, _tf * _tf * (3 - 2 * _tf));
        }
        while (_t < flightTime)
        {
            yield return null;
            _t += Time.deltaTime;
            _t2 = (_t - flightTime) / zoomTime + 1;
            _tf += Time.deltaTime * _f;
            gameCamera.orthographicSize = Mathf.Lerp(cameraSize.y, cameraSize.x, Mathf.Sqrt(_t2 * _t2 * (3 - 2 * _t2)));
            gameCamera.transform.position = Vector3.Lerp(_startPosition, _endPosition, _tf * _tf * (3 - 2 * _tf));
        }
        gameCamera.orthographicSize = cameraSize.x;
        Destroy(_prevPlanet);
        _planet.transform.position -= _newPlanetPosition;
        gameCamera.transform.position = _endPosition - _newPlanetPosition;
        shipController.transform.position -= _newPlanetPosition;
        shipController.SetPlanet(_planet);
        _newPlanetPosition.x *= -1;
        shipController.ToPlanet(_planet, shipPathLength);
        inGameUiTranslate(true);
        StopCoroutine(_zoomAtStart);
        _zoomAtStart = toMenuButton.ChangeAlpha(); // Исп-тся _zoomAtStart вместо _changeAlpha, т.к. в момент перехода она однозначно свободна
        StartCoroutine(_zoomAtStart);
        trail.SetActive(false);
    }

    private void setNextPlanet()
    {
        if (_planet)
        {
            _prevPlanet = _planet.gameObject;
        }
        GameObject planetObject;
        if (level <= staticLevelsCount)
        {
            planetObject = Instantiate(planets[level - 1], _newPlanetPosition, Quaternion.identity);
            _planet = planetObject.GetComponent<PlanetOld>();
            _planet.Create();
        }
        else
        {
            int _type = Random.Range(0, planets.Length);
            planetObject = Instantiate(planets[_type], _newPlanetPosition, Quaternion.identity);
            _planet = planetObject.GetComponent<PlanetOld>();
            _planet.Create(true);
        }
        _levelCompleted = false;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
