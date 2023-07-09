using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeSetup : MazeStartUp
{
    [Header("Corners")]

    [SerializeField]
    private int up;

    [SerializeField]
    private int right;

    [SerializeField]
    private int down;

    [SerializeField]
    private int left;

    static private int[] corners = new int[4];

    static private int enemyChance, enemyesSpawned, nearest;

    static private List<int> spawnIdx;

    static private int type;

    public void SetupCorners()
    {
        corners[0] = up;
        corners[1] = right;
        corners[2] = down;
        corners[3] = left;
    }

    static private int Expansion()
    {
        int expansionGrowth = 5;
        switch (SceneLoader.difficulty)
        {
            case "Easy":
                expansionGrowth = 8;
                break;
            case "Normal":
                expansionGrowth = 5;
                break;
            case "Hard":
                expansionGrowth = 3;
                break;
        }

        if (PlayerPrefs.GetInt("Level", 1) <= expansionGrowth * 4)
        {
            return (int)Mathf.Floor(PlayerPrefs.GetInt("Level", 1) / expansionGrowth);
        }
        else
        {
            return 2 + (int)Mathf.Floor(PlayerPrefs.GetInt("Level", 1) / expansionGrowth * 2);
        }
    }

    static private int ChooseCorner(int corner1, int corner2, int shrink, bool wich)
    {
        if (wich)
        {
            type = 1;
            return corner1 + Expansion() / shrink;
        }
        else
        {
            type = 0;
            return corner2 - Expansion() / shrink;
        }
    }

    static public void SetCorner()
    {
        gridX = ChooseCorner(corners[1], corners[3], 1, UnityEngine.Random.Range(0, 2) == 0);
        gridY = ChooseCorner(corners[0], corners[2], 2, UnityEngine.Random.Range(0, 2) == 0);

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            if (gridX > 0) direction = PIes[2];
            else direction = PIes[1];
        }
        else
        {
            if (gridY > 0) direction = PIes[3];
            else direction = PIes[0];
        }

        GameObject.Find("Lantern").GetComponent<Lantern>().SetupLantern();
        int laternX = ChooseCorner(corners[1], corners[3], 1, type == 0);
        int laternY = ChooseCorner(corners[0], corners[2], 2, type == 0);
        if (70 - PlayerPrefs.GetInt("Level") >= 0)
        {
            if (UnityEngine.Random.Range(0, 70 - PlayerPrefs.GetInt("Level")) == 0)
            {
                Lantern.ActivateLantern(laternX, laternY);
            }
        }
        else
        {
            Lantern.ActivateLantern(laternX, laternY);
        }
    }

    static public void SetBorders(Tile[] tiles)
    {
        Vector2 corner1;
        corner1 = new Vector2(corners[3] - Expansion(), corners[2] - Expansion() / 2);
        Vector2 corner2;
        corner2 = new Vector2(corners[1] + Expansion(), corners[0] + Expansion() / 2);
        Vector2 pathExstension;


        if (Expansion() < 1)
        {
            Vector2 minCorner1 = new Vector2(-6, -2);
            Vector2 minCorner2 = new Vector2(5, 3);
            pathExstension = new Vector2(5, 3);
            TilesManager.fillTiles(gridPath, tiles[0], minCorner1 - pathExstension, minCorner2 + pathExstension - new Vector2(0, 1));
            TilesManager.fillTiles(gridHzW, tiles[1], minCorner1, minCorner2);
            TilesManager.fillTiles(gridVtW, tiles[2], minCorner1, minCorner2 + new Vector2(1, -1));
        }
        else
        {
            pathExstension = new Vector2(4, 3);
            TilesManager.fillTiles(gridPath, tiles[0], corner1 - pathExstension, corner2 + pathExstension);
            TilesManager.fillTiles(gridHzW, tiles[1], corner1, corner2 + new Vector2(0, 1));
            TilesManager.fillTiles(gridVtW, tiles[2], corner1, corner2 + new Vector2(1, 0));
        }
        TilesManager.fillTiles(gridPath, null, corner1, corner2);
        TilesManager.fillTiles(gridHzW, null, corner1 - new Vector2(0, -1), corner2);
        TilesManager.fillTiles(gridVtW, null, corner1 - new Vector2(-1, 0), corner2);
    }

    static public void SetupEnemy()
    {
        GameObject.Find("Enemyes").GetComponent<EnemyesSpawn>().VirusSetup();
        enemyChance = 1;
        enemyesSpawned = 0;
        spawnIdx = new List<int>();
        nearest = 1;

        int enemyGrowth = 5;
        float enemyChanceGrowth = 0;
        switch (SceneLoader.difficulty)
        {
            case "Easy":
                enemyGrowth = 7;
                enemyChanceGrowth = 11;
                break;
            case "Normal":
                enemyGrowth = 5;
                enemyChanceGrowth = 7;
                break;
            case "Hard":
                enemyGrowth = 4;
                enemyChanceGrowth = 5;
                break;
        }
        enemyAmount = (int)Mathf.Floor((2 + PlayerPrefs.GetInt("Level", 1)) / enemyGrowth);

        for (int i = 0; i < MazeBuilder2.enemyPositions.Count && enemyesSpawned < enemyAmount; i++)
        {
            bool isClose = false;
            int radius = 2;
            if (enemyAmount == 1) radius = 1;
            if (IntersectionPoint() > -1) 
            {
                for (int j = -radius; j < radius + 1; j++)
                    if (i == IntersectionPoint() + j)
                    {
                        isClose = true;
                        nearest = -1;
                    }
            }
            if (isClose) continue;

            enemyChance++;
            if (enemyChance % (int)Mathf.Ceil(PlayerPrefs.GetInt("Level", 1) / enemyChanceGrowth) != 0) continue;
            
            EnemyesSpawn.spawnVirus(MazeBuilder2.enemyPositions[i]);
            spawnIdx.Add(i + nearest);
            Finish.coinsAmount += 10;
            enemyesSpawned++;
        }
    }

    static private int IntersectionPoint()
    {
        for (int i = 0; i < MazeBuilder2.enemyPositions.Count; i++)
            if (MazeBuilder2.enemyPositions[i] == new Vector2(playerX, playerY) / gridDimension)
                return i;
        return -1;
    }

    static public void MaskSetup()
    {
        GameObject.Find("MaskHollder").GetComponent<MaskSpawner>().SetupMask();

        float maskGrowth = 1.5f;
        switch (SceneLoader.difficulty)
        {
            case "Easy":
                maskGrowth = 1f;
                break;
            case "Normal":
                maskGrowth = 1.5f;
                break;
            case "Hard":
                maskGrowth = 2;
                break;
        }

        for (int i = 0; i < spawnIdx.Count; i++)
        {
            if (i % maskGrowth == 1) continue;
            MaskSpawner.MaskSpawn(MazeBuilder2.maskPositions[spawnIdx[i]]);
        }
    }
}
