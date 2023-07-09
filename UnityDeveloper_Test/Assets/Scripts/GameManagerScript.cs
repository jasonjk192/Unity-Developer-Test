using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    private float gameTimer = 120;
    private int collectibleCount = 0;

    public TMP_Text timerText;
    public TMP_Text gameEndText;
    public Button restartButton;
    private void Start()
    {
        timerText.gameObject.SetActive(true);
        gameEndText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        collectibleCount = 5;
        Physics.gravity = Vector3.down * 9.81f;
    }

    private void Update()
    {
        if (Input.GetAxis("Cancel") > 0)
            Application.Quit();

        if(gameTimer > 0)
            gameTimer -= Time.deltaTime;
        else
            GameEnd(2);
        
        // Display countdown
        int minutes = Mathf.FloorToInt(gameTimer / 60);
        int seconds = Mathf.FloorToInt(gameTimer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void GameEnd(int gameEndType)
    {
        switch(gameEndType)
        {
            case 0: gameEndText.text = "You won!";
                break;
            case 1: gameEndText.text = "You fell out of bounds!";
                break;
            case 2: gameEndText.text = "You ran out of time!";
                break;
            default: gameEndText.text = "You lost!";
                break;
        }

        gameEndText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void DeductCollectible(int count)
    {
        collectibleCount -= count;
        if (collectibleCount <= 0)
            GameEnd(0);
    }

    private void OnTriggerExit(Collider other)
    {
        // Game is located within a playable volume and ends when player leaves this volume
        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            GameEnd(1);
        }
            
    }
}
