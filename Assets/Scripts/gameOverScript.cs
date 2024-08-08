using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text restartText;
    [SerializeField] private Text gameOverText;
    public bool isGameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        //Disables panel if active
        gameOverPanel.SetActive(false);
        restartText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);

    }
     
    // Update is called once per frame
    void Update()
    {
        //Trigger game over manually and check with bool so it isn't called multiple times
        if (Input.GetKeyDown(KeyCode.G) && !isGameOver)
        {
            isGameOver = true;

            StartCoroutine(GameOverSequence());
        }

        //If game is over
        if (isGameOver)
        {
            StartCoroutine(GameOverSequence());
            //If R is hit, restart the current scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            //If Q is hit, quit the game
            if (Input.GetKeyDown(KeyCode.Q))
            {
                print("Application Quit");
                Application.Quit();
            }
        }


    }

    public void FadeMe()
    {
        StartCoroutine(DoFade());
    }

    IEnumerator DoFade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha < 1)
        {
            //Until the fade in is fully done, fade it in
            canvasGroup.alpha += Time.deltaTime / 100;
            yield return null;
        }
        //Just in case
        canvasGroup.interactable = false;
        yield return null;
    }
        

    //controls game over canvas and there's a brief delay between main Game Over text and option to restart/quit text
    private IEnumerator GameOverSequence()
    {
        FadeMe();
        gameOverPanel.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        restartText.gameObject.SetActive(true);
    }
}
