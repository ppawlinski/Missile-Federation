using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    public delegate void GoalScoredEventHandler(int goal_id, int env_id);
    public static event GoalScoredEventHandler GoalScored;
    public delegate void GoalExplosion(Vector3 position);
    //public static event GoalExplosion OnGoalExplosion;    

    [SerializeField] private int goal_id = 0;
    [SerializeField] private GameObject environmentContainer;

    public int ID { get=> goal_id; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Ball")
            GoalScored?.Invoke(goal_id, environmentContainer.GetInstanceID());
    }
}
