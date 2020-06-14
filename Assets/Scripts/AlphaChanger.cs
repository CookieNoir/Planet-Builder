using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Game UI/Alpha Changer")]
[RequireComponent(typeof(MaskableGraphic))]
public class AlphaChanger : MonoBehaviour
{
    public float decreasingDuration = 1f;
    public float increasingDuration = 1f;
    public float lowerAlpha = 0f;
    public float higherAlpha = 1f;
    public bool increase = false; // Если Increase = false, то Альфа будет уменьшаться, иначе - увеличиваться

    private MaskableGraphic maskableGraphic;
    private float decreasingDurationInversed;
    private float increasingDurationInversed;
    private Color lowerColor;
    private Color higherColor;

    private void Awake()
    {
        maskableGraphic = GetComponent<MaskableGraphic>();
        decreasingDurationInversed = 1f / decreasingDuration;
        increasingDurationInversed = 1f / increasingDuration;
    }

    public IEnumerator ChangeAlpha(bool hideObject = false)
    {
        Color c = maskableGraphic.color;
        increase = !increase;
        if (increase)
        {
            lowerColor = new Color(maskableGraphic.color.r, maskableGraphic.color.g, maskableGraphic.color.b, lowerAlpha);
            for (float i = 0; i < decreasingDuration; i += Time.deltaTime)
            {
                maskableGraphic.color = Vector4.Lerp(c, lowerColor, Mathf.Sqrt(i * decreasingDurationInversed));
                yield return null;
            }
            maskableGraphic.color = lowerColor;
            if (hideObject) gameObject.SetActive(false);
        }
        else
        {
            higherColor = new Color(maskableGraphic.color.r, maskableGraphic.color.g, maskableGraphic.color.b, higherAlpha);
            for (float i = 0; i < increasingDuration; i += Time.deltaTime)
            {
                maskableGraphic.color = Vector4.Lerp(c, higherColor, Mathf.Sqrt(i * increasingDurationInversed));
                yield return null;
            }
            maskableGraphic.color = higherColor;
        }
    }
}
