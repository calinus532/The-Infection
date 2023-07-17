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

    static private List<Vector4> enemiesSpawnedPropieties;

    static private List<Vector2> maskSpawnedPositions;

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

        if (GamePropieties.level <= expansionGrowth * 4)
        {
            return (int)Mathf.Floor(GamePropieties.level / expansionGrowth);
        }
        else
        {
            return 2 + (int)Mathf.Floor(GamePropieties.level / (expansionGrowth * 2));
        }
    }

    static private int ChooseCorner(int corner1, int corner2, int shrink)
    {
        if (UnityEngine.Random.Range(0, 2) == 0)
            return corner1 + Expansion() / shrink;
        else
            return corner2 - Expansion() / shrink;
    }

    static public void SetCorner(Vector2 pos, bool light)
    {
        if (pos == Vector2.zero)
        {
            gridX = ChooseCorner(corners[1], corners[3], 1);
            gridY = ChooseCorner(corners[0], corners[2], 2);
        }
        else
        {
            gridX = (int)pos.x;
            gridY = (int)pos.y;
        }

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
        int laternX = corners[3] - Expansion();
        if (gridX - corners[1] < 0) laternX = corners[1] + Expansion();
        int laternY = corners[2] - Expansion() / 2;
        if (gridY - corners[0] < 0) laternY = corners[0] + Expansion() / 2;

        if (70 - GamePropieties.level >= 0)
        {
            if (UnityEngine.Random.Range(0, 70 - GamePropieties.level) == 0 || light)
            {
                Lantern.ActivateLantern(laternX, laternY);
                MazeSaver.SaveLightCondition(1);
            }
            else MazeSaver.SaveLightCondition(0);
        }
        else
        {
            Lantern.ActivateLantern(laternX, laternY);
            MazeSaver.SaveLightCondition(1);
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

    static public void FillInside(Tile tile)
    {
        Vector2 corner1 = new Vector2(corners[3] - Expansion(), corners[2] - Expansion() / 2);
        Vector2 corner2 = new Vector2(corners[1] + Expansion(), corners[0] + Expansion() / 2);
        TilesManager.fillTiles(gridPath, tile, corner1, corner2);
    }

    static public void SetupEnemy(bool save)
    {
        enemiesSpawnedPropieties = new List<Vector4>();
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
        enemyAmount = (int)Mathf.Floor((2 + GamePropieties.level) / enemyGrowth);

        if (!save)
        {
            for (int i = 0; i < MazeBuilder2.enemyPropieties.Count && enemyesSpawned < enemyAmount; i++)
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
                if (enemyChance % (int)Mathf.Ceil(GamePropieties.level / enemyChanceGrowth) != 0) continue;

                EnemyesSpawn.spawnVirus(MazeBuilder2.enemyPropieties[i]);
                enemiesSpawnedPropieties.Add(MazeBuilder2.enemyPropieties[i]);
                spawnIdx.Add(i + nearest);
                Finish.coinsAmount += 10;
                enemyesSpawned++;
            }
            if (enemiesSpawnedPropieties.Count > 0) MazeSaver.SaveEnemyPositions(enemiesSpawnedPropieties);
        }
        else
        {
            if (MazeBuilder2.enemyPropieties == null) return;
            if (MazeBuilder2.enemyPropieties.Count == 0) return;
            foreach (Vector4 props in MazeBuilder2.enemyPropieties)
            {
                EnemyesSpawn.spawnVirus(props);
                Finish.coinsAmount += 10;
            }
            MazeSaver.SaveEnemyPositions(MazeBuilder2.enemyPropieties);
        }
    }

    static private int IntersectionPoint()
    {
        for (int i = 0; i < MazeBuilder2.enemyPropieties.Count; i++)
            if(MazeBuilder2.enemyPropieties[i].x == playerX / gridDimension && MazeBuilder2.enemyPropieties[i].y == playerY / gridDimension)
                return i;
        return -1;
    }

    static public void MaskSetup(bool save)
    {
        GameObject.Find("MaskHollder").GetComponent<MaskSpawner>().SetupMask();
        maskSpawnedPositions = new List<Vector2>();

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

        if (!save)
        {
            for (int i = 0; i < spawnIdx.Count; i++)
            {
                if (i % maskGrowth == 1) continue;
                MaskSpawner.MaskSpawn(MazeBuilder2.maskPositions[spawnIdx[i]]);
                maskSpawnedPositions.Add(MazeBuilder2.maskPositions[spawnIdx[i]]);
            }
            MazeSaver.SaveMaskPositions(maskSpawnedPositions);
        }
        else
        {
            if (MazeBuilder2.maskPositions == null) return;
            if (MazeBuilder2.maskPositions.Count == 0) return;
            foreach (Vector2 vector2 in MazeBuilder2.maskPositions)
                MaskSpawner.MaskSpawn(vector2);
            MazeSaver.SaveMaskPositions(MazeBuilder2.maskPositions);
        }
    }
}
