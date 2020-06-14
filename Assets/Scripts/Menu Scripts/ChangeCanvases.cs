using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCanvases : MonoBehaviour
{
    public GameObject menu;
    public GameObject customization;
    private int currCanvas;
    private void Start()
    {
        currCanvas = 0;
        customization.SetActive(false);
    }

    public void ChangeCanvas()
    {
        if (currCanvas == 0)
        {
            currCanvas = 1;
            menu.SetActive(false);
            customization.SetActive(true);
        }
        else if (currCanvas == 1)
        {
            currCanvas = 0;
            menu.SetActive(true);
            customization.SetActive(false);
        }
    }
}
