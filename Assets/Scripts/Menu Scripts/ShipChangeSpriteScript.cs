using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShipChangeSpriteScript : MonoBehaviour
{
    public CanvasGroup cG;
    private string shipSpriteID;
    private int shipColorID;
    //private int intShipSpriteID;
    private SpriteRenderer shipRender;
    //private const int maxSpriteID = 2;
    public const int maxColorID = 4;

    private void Start()
    {
        //if (PlayerPrefs.GetString("shipSprite") != "")
        //{
        //    shipSpriteID = PlayerPrefs.GetString("shipSprite");
        //    intShipSpriteID = Int32.Parse(PlayerPrefs.GetString("shipSprite"));
        //}
        //else
        //{
        //    PlayerPrefs.SetString("shipSprite", "1");
        //    intShipSpriteID = 1;
        //}
        if (PlayerPrefs.GetInt("shipColor") != 0)
        {
            shipColorID = PlayerPrefs.GetInt("shipColor");
        }
        else
        {
            PlayerPrefs.SetInt("shipColor", 1);
            shipColorID = 1;
        }
        shipRender = GameObject.FindGameObjectWithTag("Ship").GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        //shipRender.sprite = getSpriteForShip(intShipSpriteID);
        shipRender.color = getColorForShip(shipColorID);
        //Enable/Disable sprite buttons
        //if (intShipSpriteID >= maxSpriteID)
        //    cG.transform.Find("NextS").gameObject.SetActive(false);
        //else
        //    cG.transform.Find("NextS").gameObject.SetActive(true);
        //if (intShipSpriteID <= 1)
        //    cG.transform.Find("PrevS").gameObject.SetActive(false);
        //else
        //    cG.transform.Find("PrevS").gameObject.SetActive(true);
        //Enable/Disable color buttons
        //if (shipColorID >= maxColorID)
        //    cG.transform.Find("NextC").gameObject.SetActive(false);
        //else
        //{
        //    cG.transform.Find("NextC").gameObject.SetActive(true);
        //    cG.transform.Find("NextC").gameObject.GetComponent<Image>().color = getColorForShip(shipColorID + 1);
        //}
        //if (shipColorID <= 1)
        //    cG.transform.Find("PrevC").gameObject.SetActive(false);
        //else
        //{
        //    cG.transform.Find("PrevC").gameObject.SetActive(true);
        //    cG.transform.Find("PrevC").gameObject.GetComponent<Image>().color = getColorForShip(shipColorID - 1);
        //}
    }

    //public void ChangeScene(string scene)
    //{
    //    SaveChanges();
    //    SceneManager.LoadScene(scene);
    //}

    //public void NextSprite()
    //{
    //    if (intShipSpriteID >= maxSpriteID)
    //    {
    //        intShipSpriteID = maxSpriteID;
    //    }
    //    else
    //        intShipSpriteID++;
    //}

    //public void PrevSprite()
    //{
    //    if (intShipSpriteID <= 1)
    //    {
    //        intShipSpriteID = 1;
            
    //    }
    //    else
    //        intShipSpriteID--;
    //}

    public void NextColor()
    {
        if (shipColorID >= maxColorID)
            shipColorID = maxColorID;
        else
            shipColorID++;
    }

    public void PrevColor()
    {
        if (shipColorID <= 1)
            shipColorID = 1;
        else
            shipColorID--;
    }

    public void SetColor(int id)
    {
        shipColorID = id;
    }

    public void SaveChanges()
    {
        //PlayerPrefs.SetString("shipSprite", intShipSpriteID.ToString());
        PlayerPrefs.SetInt("shipColor", shipColorID);
    }

    public void ResetChanges()
    {
        PlayerPrefs.SetString("shipSprite", "1");
        PlayerPrefs.SetInt("shipColor", 1);
    }

    private Color getColorForShip(int id)
    {
        switch (id)
        {
            case 1:
                {
                    cG.transform.Find("CurrColor").transform.position = cG.transform.Find("White").transform.position;
                    cG.transform.Find("CurrColor").GetComponent<Image>().color = Color.white;
                    return Color.white;
                }
            case 2:
                {
                    cG.transform.Find("CurrColor").transform.position = cG.transform.Find("Green").transform.position;
                    cG.transform.Find("CurrColor").GetComponent<Image>().color = Color.green;
                    return Color.green;
                }
            case 3:
                {
                    cG.transform.Find("CurrColor").transform.position = cG.transform.Find("Red").transform.position;
                    cG.transform.Find("CurrColor").GetComponent<Image>().color = Color.red;
                    return Color.red;
                }
            case 4:
                {
                    cG.transform.Find("CurrColor").transform.position = cG.transform.Find("Blue").transform.position;
                    cG.transform.Find("CurrColor").GetComponent<Image>().color = Color.blue;
                    return Color.blue;
                }
            default: return Color.white;
        }
    }

    private Sprite getSpriteForShip(int id)
    {
        shipSpriteID = id.ToString();
        return Resources.Load<Sprite>(shipSpriteID);
    }
}
