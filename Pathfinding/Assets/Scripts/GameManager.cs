using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Parameters")]
    public int gridSizeX = 20;
    public int gridSizeY = 20;
    public Vector3Int goalPosition;
    public int displayedIndex = 0;
    public Grid grid;

    [Header("UI")]
    public Text gameoverText;
    public Text finalScoreText;
    public Text scoreText;
    public Button restartButton;

    [Header("Prefabs")]
    public Transform enemyPrefab;

    [HideInInspector] public List<Pathfinder> enemies;
    [HideInInspector] public float enemySpd = 1f;
    [HideInInspector] public int enemyNumber = 1;
    [HideInInspector] public Player player;
    [HideInInspector] public int score = 0;
    [HideInInspector] public bool paused;

    GridManager gridManager;

    private void Awake()
    {
        paused = false;

        gridManager = GetComponentInChildren<GridManager>();
        enemies = new List<Pathfinder>();
        player = GetComponentInChildren<Player>();

        enemySpd = 1f;
        enemyNumber = 0;
    }

    void Update()
    {
        if (!paused)
        {
            scoreText.text = "Score = " + score;
            if(enemies.Count != 0)
            {
                enemies[enemies.Count - 1 - displayedIndex].hidden = false;
                int i = 0;
                bool gameOver = false;
                while (i < enemies.Count && !gameOver)
                {
                    Pathfinder tmpPF = enemies[i];//Ennemi en cours, copie temporaire
                    if (tmpPF.gridPosition == goalPosition)
                    {
                        gameOver = true;
                        paused = true;
                        gameoverText.gameObject.SetActive(true);
                        finalScoreText.text = "Score = " + score;
                        finalScoreText.gameObject.SetActive(true);
                        restartButton.gameObject.SetActive(true);
                        scoreText.gameObject.SetActive(false);
                    }
                    if (player.gridPosition == tmpPF.gridPosition)
                    {
                        gridManager.mapEnemies.SetTile(tmpPF.gridPosition, null);//enleve l'ennemi de l'affichage
                        enemies.Remove(tmpPF);//enleve l'ennemi du tableau d'ennemis
                        Destroy(tmpPF.gameObject);//detruit le GO
                        score++;//incremente le score
                    }
                    i++;
                }
            }
            else
            {
                CreateEnemies(enemyNumber, enemySpd);
                enemyNumber++;
                enemySpd *= 0.9f;
            }
        }
    }

    private void CreateEnemies(int number, float speed) 
    {
        for(int i = 0; i < number; i++)
        {
            Transform enemy = Instantiate(enemyPrefab, grid.GetCellCenterWorld(new Vector3Int(Random.Range(0, gridSizeX / 2), Random.Range(0, gridSizeY / 2), 0)), new Quaternion(), this.transform.Find("Grid").GetChild(3));
            Pathfinder pf = enemy.GetComponent<Pathfinder>();
            pf.startPosition = grid.WorldToCell(enemy.position);
            pf.speed = enemySpd;
            enemies.Add(pf);
        }
    }
}