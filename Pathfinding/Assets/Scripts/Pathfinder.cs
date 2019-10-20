using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [Header("Parameters")]
    public Vector3Int startPosition;
    public bool hidden = true;

    [HideInInspector] public Vector3Int goalPosition;
    [HideInInspector] public Vector3Int nextPosition;
    [HideInInspector] public Vector3Int oldPosition;
    [HideInInspector] public Vector3Int gridPosition;
    [HideInInspector] public float speed = 1f;

    GridManager gridManager;

    List<Node> open;
    List<Node> close;

    List<Node> obstacles;
    List<Vector3Int> visitedTiles;
    List<Node> path;

    Node FindPath()
    {
        //--initialisation
        visitedTiles = new List<Vector3Int>();
        open = new List<Node>();
        close = new List<Node>();
        open.Add(new Node(gridPosition, 0, null));

        //--deroulement
        while (open.Count != 0)
        {
            open = SortByHeuristic(open);
            Node currentNode = open[0];
            open.Remove(currentNode);
            close.Add(currentNode);

            if (currentNode.position == goalPosition)
                return currentNode;
            
            //----Pour 8 voisins----
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Node neighborNode = new Node(new Vector3Int(currentNode.position.x + x, currentNode.position.y + y, 0), currentNode.cost + 1, currentNode);

                    bool opened = open.Contains(neighborNode);
                    bool closed = close.Contains(neighborNode);
                    bool isObstacle = obstacles.Contains(neighborNode);

                    if (!opened && !closed && !isObstacle)
                    {
                        neighborNode.heuristic = neighborNode.cost + Mathf.Max(Mathf.Abs(neighborNode.position.x - goalPosition.x), Mathf.Abs(neighborNode.position.y - goalPosition.y));
                        open.Add(neighborNode);
                        visitedTiles.Add(neighborNode.position);//le node est visité
                        if(!hidden)//on dessine seulement si hidden est false
                            gridManager.mapPath.SetTile(neighborNode.position, GetComponentInParent<GridManager>().visitedTile);
                    }
                }
            }
        }
        return null;
    }

    List<Node> SortByHeuristic(List<Node> list)
    {
        List<Node> sorted = new List<Node>();
        sorted.Add(list[0]);
        for(int i = 1; i < list.Count; i++)
        {
            if (list[i].heuristic < sorted[0].heuristic)
                sorted.Insert(0, list[i]);
            else if (list[i].heuristic == sorted[0].heuristic)
            {
                int rnd = UnityEngine.Random.Range(0, 2);
                sorted.Insert(rnd, list[i]);
            }
            else
                sorted.Add(list[i]);
        }
        return sorted;
    }

    void CreatePath(Node node)
    {
        if(node != null && node.parent != null)
        {
            path.Insert(0, node);
            if(!hidden)//on dessine seulement si hidden est false
                gridManager.mapPath.SetTile(node.parent.position, GetComponentInParent<GridManager>().pathTile);
            CreatePath(node.parent);
        }
    }

    private void Start()
    {
        gridManager = GetComponentInParent<GridManager>();

        obstacles = GetComponentInParent<GridManager>().obstacles;

        goalPosition = GetComponentInParent<GameManager>().goalPosition;
        gridPosition = startPosition;        
        nextPosition = gridPosition;

        path = new List<Node>();
        InvokeRepeating("Move", 0f, speed);
    }

    private void Move()
    {
        if (!gridManager.obstacles.Exists(n => n.position == nextPosition))
        {
            oldPosition = gridPosition;
            gridPosition = nextPosition;
            gridManager.mapEnemies.SetTile(oldPosition, null);
            gridManager.mapEnemies.SetTile(gridPosition, gridManager.enemyTile);
        }

        gridManager.mapPath.ClearAllTiles();

        Node goal = FindPath();
        CreatePath(goal);

        if (path.Count > 0)
            nextPosition = path[0].position;
    }

    private void Update()
    {
        if (GetComponentInParent<GameManager>().paused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
