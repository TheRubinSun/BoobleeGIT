using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private float accuracyPathfining = 1f;
    [SerializeField] private int maxPathCost = 100;
    public static Pathfinding instance;
    GridNodes grid;
    private int currentPathID = 0;
    private HashSet<NodeP> penaltyColdown = new HashSet<NodeP>();
    private List<NodeP> nodesToRemovePenalty = new List<NodeP>();
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        grid = GetComponent<GridNodes>();
    }
    void ResertNodeIfObsolete(NodeP node)
    {
        if(node.pathID != currentPathID)
        {
            node.gCost = int.MaxValue;
            node.parent = null;
            node.pathID = currentPathID;
        }
    }
    public List<NodeP> FindPath(Vector2 startPos, Vector2 targerPos)
    {
        currentPathID++;
        grid.ClearChecked();
        NodeP startNode = grid.NodeFromWorldPoint(startPos);
        NodeP targetNode = grid.NodeFromWorldPoint(targerPos);

        targetNode = GetNearestWalkable(targetNode);
        if (!targetNode.walkable) return null;

        ResertNodeIfObsolete(startNode);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode); // Инициализируем hCost для старта

        int distanceToPlayer = GetDistance(startNode, targetNode);
        if(distanceToPlayer > 50)
        {
            targetNode = GetClosesPoint(startNode, targetNode, 6);
        }

        List<NodeP> openSet = new List<NodeP>();
        HashSet<NodeP> closedSet = new HashSet<NodeP>();
        openSet.Add(startNode);

        NodeP bestNodeSoFar = startNode;

        while(openSet.Count > 0)
        {
            NodeP currentNode = openSet[0];

            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    currentNode = openSet[i];
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode.hCost + currentNode.penalty < bestNodeSoFar.hCost + bestNodeSoFar.penalty)
            {
                bestNodeSoFar = currentNode;
            }

            currentNode.isCheked = true;
            if (currentNode == targetNode)
            {
                return ReplacePath(startNode, targetNode);
            }
            foreach(NodeP neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;
                ResertNodeIfObsolete(neighbor);

                int distanceCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                int totalCostWithPenalty = distanceCost + neighbor.penalty;
                if (distanceCost > maxPathCost) continue;

                if(totalCostWithPenalty < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = totalCostWithPenalty;
                    neighbor.hCost = (int)(GetDistance(neighbor, targetNode) * accuracyPathfining);
                    neighbor.parent = currentNode;

                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        if (bestNodeSoFar != startNode)
            return ReplacePath(startNode, bestNodeSoFar);
        return null;
    }
    private void FixedUpdate()
    {
        if(Time.frameCount % 10 == 0)
        {
            nodesToRemovePenalty.Clear();
            foreach (NodeP item in penaltyColdown)
            {
                item.penalty -= 5;
                if (item.penalty < 0)
                {
                    item.penalty = 0;
                    nodesToRemovePenalty.Add(item);
                }
            }
            foreach (NodeP item in nodesToRemovePenalty)
            {
                penaltyColdown.Remove(item);
            }
        }

    }
    public void AddPenalty(NodeP node, int penalty)
    {
        if (node.penalty > 1000) return;
        node.penalty += penalty;
        penaltyColdown.Add(node);
    }
    private NodeP GetNearestWalkable(NodeP targetNode)
    {
        if(targetNode.walkable) return targetNode;

        List<NodeP> neighbors = grid.GetNeighbors(targetNode);
        NodeP nearestNode = null;
        float shortestDistance = float.MaxValue;
        foreach(NodeP neighbor in neighbors)
        {
            if(neighbor.walkable)
            {
                float dist = Vector2.Distance(neighbor.worldPos, targetNode.worldPos);
                if(dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestNode = neighbor;
                }
            }
        }

        if (nearestNode == null)
            nearestNode = grid.GetNeighborsRange(targetNode, 2);

        return nearestNode ?? targetNode;
    }
    private List<NodeP> ReplacePath(NodeP startNode, NodeP targetNode)
    {
        List<NodeP> path = new List<NodeP>();
        NodeP curNode = targetNode;
        while(curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }
        path.Reverse();
        return path;
    }

    private NodeP GetClosesPoint(NodeP startNode, NodeP targetNode, int range)
    {
        Vector2Int newPoint = new Vector2Int(
            Mathf.Clamp(targetNode.gridPos.x - startNode.gridPos.x, -range, range),
            Mathf.Clamp(targetNode.gridPos.y - startNode.gridPos.y, -range, range));
        NodeP closesNode = grid.GetNodeByGridPos(startNode.gridPos.x + newPoint.x, startNode.gridPos.y + newPoint.y);
        return closesNode.walkable ? closesNode : GetClosesPoint(startNode, targetNode, range - 1);
    }

    int GetDistance(NodeP nodeA, NodeP nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int dstY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if(dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);

    }
}
