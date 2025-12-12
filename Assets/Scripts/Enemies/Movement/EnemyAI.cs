using System.Collections.Generic;
using UnityEngine;
using Game.Navigation;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public PathfindingController pathCtrl;
    private EnemyMover mover;

    public float repathInterval = 0.5f;
    private float repathTimer;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        mover = GetComponent<EnemyMover>();
        if (pathCtrl == null)
            pathCtrl = FindFirstObjectByType<PathfindingController>();

        repathTimer = repathInterval;

        Debug.Log("pathCtrl = " + pathCtrl);
        Debug.Log("target = " + target);

        BuildNewPath();
    }

    void Update()
    {
        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            BuildNewPath();
        }
    }

    private void BuildNewPath()
    {
        if (target == null) return;
        if (pathCtrl == null) return;

        List<Vector3> path = pathCtrl.BuildPath(transform.position, target.position);
        if (path != null)
            mover.SetPath(path);
    }
}
