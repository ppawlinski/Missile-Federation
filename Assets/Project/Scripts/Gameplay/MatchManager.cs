using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private static MatchManager instance;

    [SerializeField] GameObject ball;

    PlayerManager playerManager;
    public delegate void OnCountDownStart(int time);
    public event OnCountDownStart CountDownStart;
    public delegate void OnCountDownEnd();
    public event OnCountDownEnd CountDownEnd;
    public delegate void OnGameEnd();
    public event OnGameEnd GameEnd;
    public delegate void OnGoalsUpdate(int team1, int team2);
    public event OnGoalsUpdate GoalsUpdate;

    public int Countdown { get; private set; }

    int scoreTeam1 = 0;
    int scoreTeam2 = 0;
    bool isTimerPaused = true;
    public GameObject Ball { get => ball; }
    public float Timer { get; private set; } = 300;
    public static MatchManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MatchManager>();
            }
            return instance;
        }
        private set => instance = value;
    }

    public PlayerManager PlayerManager { get => playerManager; }

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;

        playerManager = GetComponent<PlayerManager>();
        Timer = GameParameters.Instance.GameTime;
    }

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        playerManager.SetMovementBlock(true);
        Countdown = 5;
        CountDownStart?.Invoke(Countdown);
        Countdown = 5;
        while (Countdown > 0)
        {
            yield return new WaitForSeconds(1);
            Countdown--;
        }
        isTimerPaused = false;
        CountDownEnd?.Invoke();
        playerManager.SetMovementBlock(false);
    }

    private void Update()
    {
        if (!isTimerPaused) Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            Timer = 0;
            //TODO if ball is on the ground stop game
            playerManager.SetMovementBlock(true);

            //maybe set the timescale only on server
            Time.timeScale = 0;
            gameObject.SetActive(false);//??
        }
        //boostText.text = Convert.ToString(playerObject.GetComponent<CarBoost>().BoostPercentage);
    }

    private void OnEnable()
    {
        GoalCheck.GoalScored += GoalScored;
    }

    private void OnDisable()
    {
        GoalCheck.GoalScored -= GoalScored;
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
        GoalsUpdate?.Invoke(scoreTeam1, scoreTeam2);
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
