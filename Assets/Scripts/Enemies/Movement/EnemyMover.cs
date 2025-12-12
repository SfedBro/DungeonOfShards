using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float reachDistance = 0.15f;
    public LayerMask enemyMask;

    public List<Vector3> path;
    public int index;

    private float avoidRadius = 1f;
    private float avoidForce = 0f;

    public void SetPath(List<Vector3> newPath)
    {
        path = newPath;
        index = 0;
    }

    void Update()
    {
        if (path == null || index >= path.Count)
            return;

        Vector3 target = path[index];
        Vector3 dir = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0);

        if (dir.magnitude < reachDistance)
        {
            index++;
            if (index >= path.Count)
            {
                path = null;
                return;
            }
        }

        dir = dir.normalized;

        Vector3 avoidance = ComputeAvoidance();
        Vector3 finalDir = (dir + avoidance).normalized;

        transform.position += finalDir * moveSpeed * Time.deltaTime;
    }

    private Vector3 ComputeAvoidance()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, avoidRadius, enemyMask);

        Vector3 force = Vector3.zero;
        int count = 0;

        foreach (var h in hits)
        {
            if (h.transform == transform)
                continue;

            Vector3 diff = transform.position - h.transform.position;
            float dist = diff.magnitude;

            if (dist < 0.01f)
                continue;

            force += diff.normalized / dist;
            count++;
        }

        if (count > 0)
            force /= count;

        return force * avoidForce;
    }
}
