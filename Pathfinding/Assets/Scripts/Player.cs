using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Parameters")]
    public Vector3Int startPosition;
    
    [HideInInspector] public Vector3Int gridPosition;
    GridManager gridManager;

    void Awake()
    {
        gridManager = GetComponentInParent<GridManager>();
        gridPosition = startPosition;
    }

    void Update()
    {
        if (!GetComponentInParent<GameManager>().paused)
        {
            gridManager.mapPlayer.ClearAllTiles();
            gridManager.mapPlayer.SetTile(gridPosition, gridManager.playerTile);
            CheckKeys();
        }
    }

    void CheckKeys()
    {
        //UP AND DOWN
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z))
        {
            Vector3Int nextPos = gridPosition + new Vector3Int(0, 1, 0);
            if (!gridManager.obstacles.Exists(p => nextPos == p.position))
                gridPosition.y += 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Vector3Int nextPos = gridPosition + new Vector3Int(0, -1, 0);
            if (!gridManager.obstacles.Exists(p => nextPos == p.position))
                gridPosition.y -= 1;
        }

        //LEFT AND RIGHT
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
        {
            Vector3Int nextPos = gridPosition + new Vector3Int(-1, 0, 0);
            if (!gridManager.obstacles.Exists(p => nextPos == p.position))
                gridPosition.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Vector3Int nextPos = gridPosition + new Vector3Int(1, 0, 0);
            if (!gridManager.obstacles.Exists(p => nextPos == p.position))
                gridPosition.x += 1;
        }
    }
}
