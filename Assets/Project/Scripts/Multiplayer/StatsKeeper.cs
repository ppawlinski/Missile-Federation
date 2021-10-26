using UnityEngine;
using Mirror;

public class StatsKeeper : NetworkBehaviour
{
    public SyncList<PlayerStats> stats = new SyncList<PlayerStats>();

    private void Start()
    {
        stats.Callback += Test;
    }

    public void SetStats(SyncList<PlayerStats> stats) 
    {
        this.stats = stats;
        Debug.Log("Stats set");
    }

    private void Test(SyncList<PlayerStats>.Operation op, int index, PlayerStats oldstat, PlayerStats newstat)
    {
        Debug.Log("Callback");
    }
}
