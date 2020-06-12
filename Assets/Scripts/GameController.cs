using UnityEngine;

public class GameController : MonoBehaviour
{
    public Planet planet;
    private bool levelCompleted;
    public static GameController instance;

    void Start()
    {
        planet.Create();
        levelCompleted = false;
        instance = this as GameController;
    }

    public static void CheckLevel()
    {
        instance.levelCompleted = instance.planet.IsCompleted();
    }

    public static void Restart()
    {
        instance.planet.Refresh();
    }
}
