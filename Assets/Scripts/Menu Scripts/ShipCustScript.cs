using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCustScript : MonoBehaviour
{
    private SpriteRenderer shipRender;

    private void Start()
    {
        shipRender = gameObject.GetComponent<SpriteRenderer>();
        //setShipSpriteByID(PlayerPrefs.GetString("shipSprite"));
        setShipColor(PlayerPrefs.GetInt("shipColor"));
    }

    private void setShipSpriteByID(string id)
    {
        shipRender.sprite = Resources.Load<Sprite>(id);
    }

    private void setShipColor(int id)
    {
        switch (id)
        {
            case 1:
                {
                    shipRender.color = Color.white;
                    break;
                }
            case 2:
                {
                    shipRender.color = Color.green;
                    break;
                }
            case 3:
                {
                    shipRender.color = Color.red;
                    break;
                }
            case 4:
                {
                    shipRender.color = Color.blue;
                    break;
                }
        }
    }
}
