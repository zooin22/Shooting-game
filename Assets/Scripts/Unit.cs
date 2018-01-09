using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    const float pathUpdateMoveThreshold = .5f;
    const float minPathUpdateTime = .2f;

    Vector3 targetPosOld;
    float sqrMoveThreshold;
    public Transform target;
    public float speed = 1;
    Vector3[] path;
    int targetIndex;

    void Start()
    {
        //StartCoroutine(UpdatePath());
    }

    public void Init(Transform _target,float _speed)
    {
        target = _target;
        speed = _speed;
        sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        targetPosOld = target.position;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            if(this.gameObject.activeSelf)
                StartCoroutine("FollowPath");
        }
    }

    public void StopMove()
    {
        StopCoroutine("FollowPath");
    }

    public void UpdatedPath()
    {
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        targetPosOld = target.position;
    }
    
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}