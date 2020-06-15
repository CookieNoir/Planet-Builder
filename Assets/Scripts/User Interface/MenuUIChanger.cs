using System.Collections;
using UnityEngine;

public class MenuUIChanger : MonoBehaviour
{
    public GameController gameController;
    public UiMovement mainMenu;
    public UiMovement customizationMenuLower;
    public ShipUIMover shipMover;
    public AlphaChanger shipAlphaChanger;
    public UiMovement toMenuButton;

    private bool _customizing;
    private bool _canChange;
    private bool _isPlaying;
    private bool _isPaused;
    private IEnumerator _moveui;
    private IEnumerator _alphaChanger;
    private IEnumerator _startGame;

    void Start()
    {
        _customizing = false;
        _canChange = true;
        _isPlaying = false;
        _isPaused = false;
        _moveui = moveui();
        _alphaChanger = shipAlphaChanger.ChangeAlpha();
        _startGame = startGame();
        mainMenu.Translate();
    }

    public void ChangeMenu()
    {
        if (_canChange)
        {
            _canChange = false;
            StopCoroutine(_moveui);
            _moveui = moveui();
            StartCoroutine(_moveui);
        }
    }

    public void StartGame()
    {
        if (!_customizing&&_canChange)
        {
            _isPlaying = true;
            _canChange = false;
            StopCoroutine(_startGame);
            _startGame = startGame();
            StartCoroutine(_startGame);
        }
    }

    private IEnumerator startGame()
    {
        float _t = 0;
        mainMenu.Translate();
        gameController.StartGame();
        while (_t < mainMenu.translationDuration)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        _t = 0;
        toMenuButton.Translate();
        while (_t < toMenuButton.translationDuration)
        {
            yield return null;
            _t += Time.deltaTime;
        }
    }

    private IEnumerator moveui()
    {
        float _t = 0;
        if (_customizing)
        {
            customizationMenuLower.Translate();
            shipMover.Translate();
            StopCoroutine(_alphaChanger);
            _alphaChanger = shipAlphaChanger.ChangeAlpha();
            StartCoroutine(_alphaChanger);
            while (_t < customizationMenuLower.translationDuration)
            {
                yield return null;
                _t += Time.deltaTime;
            }
            _t = 0;
            mainMenu.Translate();
            while (_t < mainMenu.translationDuration)
            {
                yield return null;
                _t += Time.deltaTime;
            }
        }
        else
        {
            mainMenu.Translate();
            while (_t < mainMenu.translationDuration)
            {
                yield return null;
                _t += Time.deltaTime;
            }
            _t = 0;
            customizationMenuLower.Translate();
            shipMover.Translate();
            StopCoroutine(_alphaChanger);
            _alphaChanger = shipAlphaChanger.ChangeAlpha();
            StartCoroutine(_alphaChanger);
            while (_t < customizationMenuLower.translationDuration)
            {
                yield return null;
                _t += Time.deltaTime;
            }
        }
        _customizing = !_customizing;
        _canChange = true;
    }
}
