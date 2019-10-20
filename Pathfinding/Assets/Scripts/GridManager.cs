using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    int xSize;
    int ySize;

    [Header("Grid")]
    public Grid grid;
    public Tilemap mapGrass;
    public Tilemap mapEnemies;
    public Tilemap mapPlayer;
    public Tilemap mapObjects;
    public Tilemap mapPath;

    [Header("Tiles")]
    public Tile grassTile;
    public Tile enemyTile;
    public Tile goalTile;
    public Tile visitedTile;
    public Tile pathTile;
    public Tile obstacleTile;
    public Tile playerTile;

    public List<Node> obstacles;

    private void RedrawMap()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (!obstacles.Exists(n => n.position == new Vector3Int(x, y, 0)))
                {
                    mapGrass.SetTile(tilePos, grassTile);
                }
            }
        }
    }

    private void CreateWalls()
    {
        for (int x = -1; x <= xSize; x++)
        {
            obstacles.Add(new Node(new Vector3Int(x, -1, 0), 0, null));
            mapGrass.SetTile(obstacles[obstacles.Count - 1].position, obstacleTile);
            obstacles.Add(new Node(new Vector3Int(x, ySize, 0), 0, null));
            mapGrass.SetTile(obstacles[obstacles.Count - 1].position, obstacleTile);
        }
        for (int y = -1; y <= ySize; y++)
        {
            obstacles.Add(new Node(new Vector3Int(-1, y, 0), 0, null));
            mapGrass.SetTile(obstacles[obstacles.Count - 1].position, obstacleTile);
            obstacles.Add(new Node(new Vector3Int(xSize, y, 0), 0, null));
            mapGrass.SetTile(obstacles[obstacles.Count - 1].position, obstacleTile);
        }
    }

    public void Awake()
    {
        xSize = GetComponentInParent<GameManager>().gridSizeX;
        ySize = GetComponentInParent<GameManager>().gridSizeY;

        obstacles = new List<Node>();

        RedrawMap();
        CreateWalls();

        foreach (Pathfinder pf in GetComponentInParent<GameManager>().enemies)
            mapEnemies.SetTile(pf.startPosition, enemyTile);

        mapObjects.SetTile(GetComponentInParent<GameManager>().goalPosition, goalTile); ;
    }

    public void Update()
    {
        WallCreatorManager();
    }

    private void WallCreatorManager()
    {
        if (Input.GetKey(KeyCode.W))//Well creator
        {
            if (Input.GetMouseButton(0))//Build wall
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = grid.WorldToCell(worldPoint);

                Node node = new Node(position, 0, null);
                obstacles.Add(node);
                mapObjects.SetTile(position, obstacleTile);

            }

            if (Input.GetMouseButton(1))//Erase wall
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = grid.WorldToCell(worldPoint);
                mapGrass.SetTile(position, grassTile);
            }
        }
    }
}
