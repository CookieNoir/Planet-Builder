using UnityEngine;
using UnityEngine.UI;

public class ShipCustomization : MonoBehaviour
{
    public SpriteRenderer shipGame;
    public Image shipMenu;

    public void ChangeColor(Image icon)
    {
        shipGame.color = icon.color;
        shipMenu.color = icon.color;
    }
}
