using System.Collections;
using UnityEngine;
using Mirror;

public class NetworkMatchManager : NetworkBehaviour
{
    private static NetworkMatchManager instance;
    [SerializeField] GameObject ball;
    NetworkPlayerManager playerManager;

    public delegate void CountDownStartedEventHandler(int time);
    public static event CountDownStartedEventHandler CountDownStarted;

    public delegate void CountDownEndedEventHandler();
    public static event CountDownEndedEventHandler CountDownEnded;

    public delegate void GameEndedEventHandler();
    public static event GameEndedEventHandler GameEnded;

    public delegate void ScoreUpdatedEventHandler(int team1, int team2);
    public static event ScoreUpdatedEventHandler ScoreUpdated;

    [SyncVar]
    int countdown;
    public int Countdown { get => countdown; private set => countdown = value; }

    int scoreTeam1 = 0;
    int scoreTeam2 = 0;
    bool isTimerPaused = true;

    [SyncVar]
    float timer = 300;
    public GameObject Ball { get => ball; }
    public float Timer { get=> timer; private set=> timer = value; }
    public static NetworkMatchManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NetworkMatchManager>();
            }
            return instance;
        }
        private set => instance = value;
    }

    public NetworkPlayerManager PlayerManager { get => playerManager; }

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        playerManager = GetComponent<NetworkPlayerManager>();
        Timer = GameParameters.Instance.GameTime;
    }

    private void Start()
    {
        if (!isServer) return;
        StartGame();
    }

    private void StartGame()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        playerManager.SetMovementBlock(true);
        countdown = 5;
        
        RpcStartCountdown(countdown);
        while (countdown > 0)
        {
            yield return new WaitForSeconds(1);
            countdown--;
        }
        isTimerPaused = false;
        RpcCountdownEnd();
        playerManager.SetMovementBlock(false);
    }

    [ClientRpc]
    private void RpcStartCountdown(int value)
    {
        CountDownStarted?.Invoke(value);
    }

    [ClientRpc]
    private void RpcCountdownEnd()
    {
        CountDownEnded?.Invoke();
    }

    [ServerCallback]
    private void Update()
    {
        if (!isTimerPaused) Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            Timer = 0;
            //TODO if ball is on the ground stop game
            playerManager.SetMovementBlock(true);

            Time.timeScale = 0;
            gameObject.SetActive(false);//??
        }
    }

    [ServerCallback]
    private void OnEnable()
    {
        GoalCheck.GoalScored += GoalScored;
    }

    [ServerCallback]
    private void OnDisable()
    {
        GoalCheck.GoalScored -= GoalScored;
    }

    private void GoalScored(int goal_id, int env_id)
    {
        isTimerPaused = true;

        //update score
        if (goal_id == 1)
            scoreTeam2++;
        else
            scoreTeam1++;
        RpcUpdateScore(scoreTeam1, scoreTeam2);

        //reset positions
        StartCoroutine(ResetGame());
    }

    [ClientRpc]
    private void RpcUpdateScore(int team1, int team2)
    {
        ScoreUpdated?.Invoke(team1, team2);
    }

    private IEnumerator ResetGame()
    {
        ball.SetActive(false);
        yield return new WaitForSecondsRealtime(4);
        ball.GetComponent<Ball>().ResetPosition();
        ball.SetActive(true);

        playerManager.ResetPositions();
        StartCoroutine(StartCountdown());
    }
}
