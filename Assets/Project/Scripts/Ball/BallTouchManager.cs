using UnityEngine;

public class BallTouchManager : MonoBehaviour
{
    [SerializeField] LayerMask ignoredLayers;
    [SerializeField] NetworkPlayerManager playerManager;
    Vector3 velocityLastFrame;
    Rigidbody rb;

    public delegate void BallTouchedEventHandler(GameObject player, bool save);
    public static event BallTouchedEventHandler BallTouched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        velocityLastFrame = rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GoingTowardsGoal(velocityLastFrame, out int goal))
            {
                int playerTeam = playerManager.GetPlayerFromInstanceId(collision.gameObject.GetInstanceID()).Team;

                if (playerTeam != goal) return;
                if(!GoingTowardsGoal(rb.velocity, out _))
                {
                    BallTouched?.Invoke(collision.gameObject, true);
                    return;
                }
            }
            BallTouched?.Invoke(collision.gameObject, false);
        }
    }

    //TODO change to trajectory prediction which could be reused in AI training
    private bool GoingTowardsGoal(Vector3 velocity, out int goal)
    {
        Ray ray = new Ray(transform.position, velocity.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, 40f, ~ignoredLayers))
        {
            if (hit.transform.CompareTag("Goal"))
            {
                goal = hit.transform.GetComponent<GoalCheck>().ID;
                return true;
            }
        }
            

        ray = new Ray(transform.position, velocity.Flat().normalized);
        if (Physics.Raycast(ray, out hit, 40f, ~ignoredLayers))
        {
            if (hit.transform.CompareTag("Goal"))
            {
                goal = hit.transform.GetComponent<GoalCheck>().ID;
                return true;
            }
        }
        goal = 0;
        return false;
    }

    //if touching while ball is going towards the goal
    //start coroutine
    //after some time check if its still going towards the goal
    //if not - give a save

    //potential problem - multiple touches between coroutine start and end
}
