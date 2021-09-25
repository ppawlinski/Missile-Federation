using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    [SerializeField] GameObject ball;

    //<??????????????????> why do i need this here
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject scoreBoard;
    [SerializeField] GameObject hud;
    [SerializeField] Text scoreText;
    [SerializeField] Text boostText;
    [SerializeField] Text timerText;
    [SerializeField] Text countdownText;
    //</??????????????????>

    PlayerManager playerManager;

    //public MatchStats stats;


    int scoreTeam1 = 0;
    int scoreTeam2 = 0;
    float timer = 300;
    bool isTimerPaused = true;
    public GameObject Ball { get => ball; }

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        timer = GameParameters.Instance.GameTime;

        //TODO separate match stats class
        //stats = new MatchStats(players);
        //SetPlayersActive(true);
    }

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        playerManager.SetMovementBlock(true);
        countdownText.enabled = true;
        int countDown = 5;
        while (countDown > 0)
        {
            countdownText.text = Convert.ToString(countDown);
            yield return new WaitForSeconds(1);
            countDown--;
        }
        isTimerPaused = false;
        countdownText.enabled = false;
        playerManager.SetMovementBlock(false);
    }

    private void Update()
    {
        if (!isTimerPaused) timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            //if ball is on the ground stop game
            playerManager.SetMovementBlock(true);
            Time.timeScale = 0;
            endScreen.SetActive(true);
            scoreBoard.SetActive(true);
            hud.SetActive(false);
            gameObject.SetActive(false); ;
        }
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        if (seconds > 9)
            timerText.text = minutes + ":" + seconds;
        else
            timerText.text = minutes + ":0" + seconds;

        //boostText.text = Convert.ToString(playerObject.GetComponent<CarBoost>().BoostPercentage);
    }

    private void OnEnable()
    {
        GoalCheck.OnGoal += GoalScored;
    }

    private void OnDisable()
    {
        GoalCheck.OnGoal -= GoalScored;
    }
    private void OnDestroy()
    {
        //TODO
        //stats.CleanUp();
    }

    private void GoalScored(int goal_id, int env_id)
    {
        isTimerPaused = true;
        //update score
        if (goal_id == 1)
        {
            scoreTeam2++;
        }
        else
            scoreTeam1++;
        scoreText.text = scoreTeam1 + " - " + scoreTeam2;
        //reset positions
        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame()
    {
        ball.SetActive(false);
        yield return new WaitForSecondsRealtime(4);
        ball.GetComponent<Ball>().ResetPosition();
        ball.SetActive(true);

        GetComponent<PlayerManager>().ResetPositions();
        StartCoroutine(StartCountdown());
    }
}
