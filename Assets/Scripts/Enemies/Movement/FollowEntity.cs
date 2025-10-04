using UnityEngine;
using UnityEngine.AI;

public class FollowEntity : MonoBehaviour
{
    [Header("Target to follow"), SerializeField]
    protected Transform goal;

    [Header("Distance to keep")]
    private float offset;

    [Header("Speed")]
    private float speed;

    [Header("Navigation Agent")]
    private NavMeshAgent agent;
    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        ChangeGoal(goal);
    }

    public bool ChangeGoal(Transform goal)
    {
        if (!goal)
            return false;
        NavMeshPath path = new NavMeshPath();
        Vector3 destination = goal.position;
        agent.CalculatePath(destination, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            return agent.SetDestination(goal.transform.position);
        }
        return false;
    }
}
