using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimScript : MonoBehaviour
{
    public CanvasGroup cG;
    private bool isPlay;
    private Vector3 startPos;
    private Vector3 endPos;
    private float lerpTime = 0.2f;
    private float currLerpTime = 0;
    private GameObject planet;
    //private GameObject ship;
    private string scene;

    private void Start()
    {
        isPlay = false;
        planet = GameObject.FindGameObjectWithTag("Planet");
        //ship = GameObject.FindGameObjectWithTag("Ship");
        startPos = planet.transform.position;
        endPos = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        if ((planet.transform.localScale.x >= 6f))
        {
            planet.transform.localScale = new Vector3(6f, 6f, planet.transform.position.z);
            SceneManager.LoadScene(scene);
        }
    }

    private void Update()
    {
        if (isPlay)
        {
            //ship.SetActive(false);
            currLerpTime += Time.deltaTime;
            if (currLerpTime >= lerpTime)
            {
                currLerpTime = lerpTime;
            }
            float Perc = currLerpTime / lerpTime;
            planet.transform.position = Vector3.Lerp(startPos, endPos, Perc);
            planet.transform.localScale = new Vector3(planet.transform.localScale.x + Time.deltaTime * 20f, planet.transform.localScale.y + Time.deltaTime * 20f, planet.transform.localScale.z);
        }
    }

    public void Play(string changeScene)
    {
        StartCoroutine(FadeCanvasGroup(cG, cG.alpha, 0));
        isPlay = true;
        scene = changeScene;
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.2f)
    {
        float timeStartingLerping = Time.time;
        float timeSinceStarting = Time.time - timeStartingLerping;
        float percentComplete = timeSinceStarting / lerpTime;
        while (true)
        {
            timeSinceStarting = Time.time - timeStartingLerping;
            percentComplete = timeSinceStarting / lerpTime;
            float currVal = Mathf.Lerp(start, end, percentComplete);
            cg.alpha = currVal;
            if (percentComplete >= 1) break;
            yield return new WaitForEndOfFrame();
        }
    }
}
