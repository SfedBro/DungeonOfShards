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
    private NavigationGrid grid;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        mover = GetComponent<EnemyMover>();
        if (pathCtrl == null)
            pathCtrl = FindFirstObjectByType<PathfindingController>();

        grid = FindFirstObjectByType<NavigationGrid>();
        repathTimer = repathInterval;

        //Debug.Log("pathCtrl = " + pathCtrl);
        //Debug.Log("target = " + target);

        BuildNewPath();
    }

    void Update()
    {
        Vector2Int g = grid.WorldToGrid(transform.position);
        //Debug.Log("Enemy pos = " + transform.position + " grid = " + g);
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

        Vector2Int s = grid.WorldToGrid(transform.position);
        Vector2Int t = grid.WorldToGrid(target.position);

        Debug.Log(
            "Start=" + s +
            " End=" + t +
            " StartWalkable=" + grid.IsWalkable(s.x, s.y) +
            " EndWalkable=" + grid.IsWalkable(t.x, t.y)
        );


        List<Vector3> path = pathCtrl.BuildPath(transform.position, target.position);
        if (path != null)
            mover.SetPath(path);
    }
}
