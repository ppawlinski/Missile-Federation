using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    public delegate void Goal(int goal_id, int env_id);
    public static event Goal OnGoal;
    public delegate void GoalExplosion(Vector3 position);
    public static event GoalExplosion OnGoalExplosion;    

    [SerializeField] private int goal_id = 0;
    [SerializeField] private GameObject environmentContainer;

    public int ID { get=> goal_id; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Ball")
            OnGoal?.Invoke(goal_id, environmentContainer.GetInstanceID());
    }
}
