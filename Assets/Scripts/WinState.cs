using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinState : MonoBehaviour
{
    [SerializeField] private GameObject winStatePanel;
    [SerializeField] private Text winRestartText;
    [SerializeField] private Text winText;
    public bool isWon = false;


    // Start is called before the first frame update
    void Start()
    {
        //Disables panel if active
        winStatePanel.SetActive(false);
        winRestartText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);

    }
     
    // Update is called once per frame
    void Update()
    {

        //If game is over
        if (isWon)
        {
            StartCoroutine(WinSequence());
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
    private IEnumerator WinSequence()
    {
        FadeMe();
        winStatePanel.SetActive(true);
        winText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        winRestartText.gameObject.SetActive(true);
    }
}
