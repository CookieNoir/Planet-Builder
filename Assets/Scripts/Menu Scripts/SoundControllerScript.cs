using UnityEngine;
using UnityEngine.UI;

public class SoundControllerScript : MonoBehaviour
{
    private bool _enabled;
    public Image soundButton;
    public Color turnedOn;
    public Color turnedOff;

    private void Start()
    {
        _enabled = true;
        soundButton.color = turnedOn;
    }

    public void SwitchAudio()
    {
        _enabled = !_enabled;
        if (_enabled)
        {
            AudioListener.volume = 1f;
            soundButton.color = turnedOn;
        }
        else
        {
            AudioListener.volume = 0f;
            soundButton.color = turnedOff;
        }
    }

}
