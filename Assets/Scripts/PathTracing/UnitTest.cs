using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTest : MonoBehaviour
{
    public Transform player;
    public float speed = 1f;
    Pathfinding pathfinding;
    List<NodeP> path;

    private void Start()
    {
        player = GlobalData.GameManager.PlayerModel;
        pathfinding = Pathfinding.instance;
        StartCoroutine(UpdatePathRoutine());
    }
    private void Update()
    {
        if (path != null && path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            path = pathfinding.FindPath(transform.position, player.position);

            yield return new WaitForSeconds(0.5f);
        }
    }
    void MoveAlongPath()
    {
        Vector2 targetPos = path[0].worldPos;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            path.RemoveAt(0);
        }
    }
    private void OnDrawGizmos()
    {
        if(path != null)
        {
            Gizmos.color = Color.black;
            foreach (NodeP n in path)
            {
                Gizmos.DrawCube(n.worldPos, Vector2.one * 0.3f);
            }
        }
    }
}
