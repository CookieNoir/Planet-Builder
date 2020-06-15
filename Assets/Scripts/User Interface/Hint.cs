using System.Collections;
using UnityEngine;

public class Hint : MonoBehaviour
{
    public AlphaChanger textComponent;
    public float hintDelay = 0f;
    public float lifeTime = 2f;
    public Hint nextHint; // Если следующий совет установлен, он будет вызван после исчезновения текущего
    private IEnumerator _showHint;
    private IEnumerator _changeAlpha;

    private void Start()
    {
        textComponent.increase = true;
        _showHint = showHint();
        _changeAlpha = textComponent.ChangeAlpha();
        textComponent.gameObject.SetActive(false);
    }

    public void ShowHint()
    {
        StopCoroutine(_showHint);
        _showHint = showHint();
        StartCoroutine(_showHint);
    }

    private IEnumerator showHint()
    {
        float _t = 0;
        while (_t < hintDelay)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        _t = 0;
        textComponent.gameObject.SetActive(true);
        textComponent.SetAlpha(0f);
        StopCoroutine(_changeAlpha);
        _changeAlpha = textComponent.ChangeAlpha(SimpleFunctions.SmoothStep);
        StartCoroutine(_changeAlpha);
        while (_t < textComponent.increasingDuration)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        _t = 0;
        while (_t < lifeTime)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        _t = 0;
        StopCoroutine(_changeAlpha);
        _changeAlpha = textComponent.ChangeAlpha(SimpleFunctions.SmoothStep, true);
        StartCoroutine(_changeAlpha);
        while (_t < textComponent.decreasingDuration)
        {
            yield return null;
            _t += Time.deltaTime;
        }
        if (nextHint) nextHint.ShowHint();
    }
}
