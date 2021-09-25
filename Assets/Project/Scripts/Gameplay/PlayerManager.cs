using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject botPrefab;

    GameObject playerObject;
    int playersPerTeam = 1;
    int playersTeam1 = 0;
    int playersTeam2 = 0;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();

    readonly Vector3[] spawnPoints = {
        new Vector3(82.5f,14,6.5f),
        new Vector3(82.5f,14,-6.5f),
        new Vector3(97.5f,14,0),
        new Vector3(53,14,44),
        new Vector3(53,14,-44)
    };
    readonly Vector3[] respawnPoints =
    {
        new Vector3(95,12.9f, 56),
        new Vector3(95,12.9f, 48),
        new Vector3(95,12.9f, -56),
        new Vector3(95,12.9f, -48)
    };

    readonly Vector3 team2SpawnOffset = new Vector3(-1, 1, -1);
    readonly Vector3 team2RespawnOffset = new Vector3(-1, 1, 1);

    public GameObject PlayerObject { get => playerObject; }

    private void Awake()
    {
        playersPerTeam = GameParameters.Instance.PlayersPerTeam;
        InstantiatePlayersLocal();
    }
    private void OnEnable()
    {
        CarDemoliition.OnDemo += OnCarDemolished;
        InGameUIController.OnGamePause += PauseAudio;
    }

    private void OnDisable()
    {
        CarDemoliition.OnDemo -= OnCarDemolished;
        InGameUIController.OnGamePause -= PauseAudio;
    }

    private void Start()
    {
        ResetPositions();
    }
    private void InstantiatePlayersLocal()
    {
        Player player;
        GameObject gObject;
        playerObject = Instantiate(playerPrefab);
        playersTeam1++;
        player = new Player(playerObject, Player.PlayerType.Local, 1, "Player");
        players.Add(playerObject.GetInstanceID(), player);

        while (playersTeam1 < playersPerTeam)
        {
            gObject = Instantiate(botPrefab);
            player = new Player(gObject, Player.PlayerType.LocalAI, 1, "Bot " + playersTeam1);
            players.Add(gObject.GetInstanceID(), player);
            playersTeam1++;
        }

        while (playersTeam2 < playersPerTeam)
        {
            gObject = Instantiate(botPrefab);
            player = new Player(gObject, Player.PlayerType.LocalAI, 2, "Bot " + playersTeam2);
            players.Add(gObject.GetInstanceID(), player);
            playersTeam2++;
        }
    }
    private List<int> ChooseSpawnPoints()
    {
        List<int> chosenPoints = new List<int>();
        int _temp;
        while (chosenPoints.Count < playersPerTeam)
        {
            _temp = Random.Range(0, 5);
            if (!chosenPoints.Contains(_temp)) chosenPoints.Add(_temp);
        }
        return chosenPoints;
    }
    private void SetPlayerPositions()
    {
        List<int> points = ChooseSpawnPoints();
        int playersSetTeam1 = 0;
        int playersSetTeam2 = 0;
        GameObject temp;
        foreach (KeyValuePair<int, Player> p in players)
        {
            temp = p.Value.PlayerObject;
            if (p.Value.Team == 1)
            {
                temp.transform.position = spawnPoints[points[playersSetTeam1]];
                temp.transform.eulerAngles = new Vector3(0, -90, 0);
                playersSetTeam1++;
            }
            else
            {
                temp.transform.position = Vector3.Scale(spawnPoints[points[playersSetTeam2]], team2SpawnOffset);
                temp.transform.eulerAngles = new Vector3(0, 90, 0);
                playersSetTeam2++;
            }
        }
    }
    public void ResetPositions()
    {
        //TODO do this in ball manager
        /*//ball
        Rigidbody ball_rb = Ball.GetComponent<Rigidbody>();
        ball_rb.velocity = Vector3.zero;
        ball_rb.angularVelocity = Vector3.zero;
        Ball.transform.position = new Vector3(0, 14.7f, 0);
        Ball.SetActive(true);*/
        Rigidbody rb;
        foreach (KeyValuePair<int, Player> p in players)
        {
            rb = p.Value.PlayerObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            p.Value.PlayerObject.GetComponent<CarBoost>().SetBoostAmount(33);
        }
        SetPlayerPositions();
    }
    public void OnCarDemolished(GameObject sender)
    {
        StartCoroutine(RespawnPlayer(sender));
    }
    IEnumerator RespawnPlayer(GameObject carObject)
    {
        yield return new WaitForSeconds(3);
        int position = UnityEngine.Random.Range(0, 4);
        Player temp;
        players.TryGetValue(carObject.GetInstanceID(), out temp);
        carObject.transform.position = temp.Team == 1 ? respawnPoints[position] : Vector3.Scale(respawnPoints[position], team2RespawnOffset);
        carObject.transform.eulerAngles = temp.Team == 1 ? new Vector3(0, -90, 0) : new Vector3(0, 90, 0);
        carObject.SetActive(true);
    }
    public void SetMovementBlock(bool value)
    {
        foreach (KeyValuePair<int, Player> p in players)
        {
            //TODO change playerobject to be the subobject of prefab (it makes sense ot me at the time of writing this)
            p.Value.PlayerObject.GetComponentInChildren<CarController>().SetFreeze(value);
        }
    }
    public void PauseAudio(bool value)
    {
        foreach (KeyValuePair<int, Player> p in players)
        {
            p.Value.PauseAudio(value);
        }
    }
    public Player GetPlayerFromInstanceId(int id)
    {
        if (!players.TryGetValue(id, out Player p)) return null;
        return p;
    }
}
