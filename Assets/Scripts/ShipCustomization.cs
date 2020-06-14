using UnityEngine;
using UnityEngine.UI;

public class ShipCustomization : MonoBehaviour
{
    public SpriteRenderer shipGame;
    public Image shipMenu;
    public Sprite[] ships;
    public Texture[] shipsMasks;
    private int _shipid;

    private void Start()
    {
        _shipid = PlayerPrefs.GetInt("Ship Type", 0);
        setColor(loadColor());
        changeTextures();
    }

    public void ChangeColor(Image icon)
    {
        setColor(icon.color);
        saveColor(icon.color);
    }

    private void setColor(Color color)
    {
        shipGame.color = color;
        shipMenu.color = color;
    }

    private Color loadColor()
    {
        return new Color(
            PlayerPrefs.GetFloat("Ship Color R", 1f),
            PlayerPrefs.GetFloat("Ship Color G", 1f),
            PlayerPrefs.GetFloat("Ship Color B", 1f),
            1f);
    }

    private void saveColor(Color color)
    {
        PlayerPrefs.SetFloat("Ship Color R", color.r);
        PlayerPrefs.SetFloat("Ship Color G", color.g);
        PlayerPrefs.SetFloat("Ship Color B", color.b);
    }

    public void ChangeShip(int id)
    {
        _shipid = id;
        PlayerPrefs.SetInt("Ship Type", _shipid);
        changeTextures();
    }

    private void changeTextures()
    {
        shipGame.sprite = ships[_shipid];
        shipGame.material.SetTexture("_MaskTex", shipsMasks[_shipid]);
        shipMenu.sprite = ships[_shipid];
        shipMenu.material.SetTexture("_MaskTex", shipsMasks[_shipid]);
        shipMenu.rectTransform.sizeDelta = new Vector2(shipsMasks[_shipid].width*4, shipsMasks[_shipid].height*4);
    }
}
