using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    private bool _enabled;
    public Image soundButton;
    public Sprite turnedOn;
    public Sprite turnedOff;

    private void Start()
    {
        int _val = PlayerPrefs.GetInt("Sound", 1);
        if (_val > 0)
        {
            _enabled = true;
            AudioListener.volume = 1f;
            soundButton.sprite = turnedOn;
        }
        else
        {
            _enabled = false;
            AudioListener.volume = 0f;
            soundButton.sprite = turnedOff;
        }
    }

    public void SwitchAudio()
    {
        _enabled = !_enabled;
        if (_enabled)
        {
            AudioListener.volume = 1f;
            soundButton.sprite = turnedOn;
            PlayerPrefs.SetInt("Sound", 1);
        }
        else
        {
            AudioListener.volume = 0f;
            soundButton.sprite = turnedOff;
            PlayerPrefs.SetInt("Sound", 0);
        }
    }

}
