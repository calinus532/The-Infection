using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeStartUp : MonoBehaviour
{
    static protected int gridX;

    static protected int gridY;

    static protected float direction;

    protected bool backtrack;

    static protected float[] PIes = new float[4]
    {
        0f,
        MathF.PI / 2f,
        -MathF.PI / 2f,
        MathF.PI
    };

    protected int maxDistance, enemyDistance, maskDistance = 1;

    static protected int enemyAmount;

    static public Tilemap gridPath { private set; get; }

    static public Tilemap gridHzW { private set; get; }

    static public Tilemap gridVtW { private set; get; }

    static public Tilemap gridHzL { private set; get; }

    static public Tilemap gridVtL { private set; get; }

    static public float gridDimension { get; private set; }

    protected GameObject finishCollectible;

    static protected float playerX, playerY;

    protected void SetupGridsAndTiles()
    {
        gridPath = GameObject.Find("Paths").GetComponent<Tilemap>();
        gridDimension = gridPath.GetComponentInParent<Grid>().cellSize.x + gridPath.GetComponentInParent<Grid>().cellGap.x;
        gridHzL = GameObject.Find("Horizontal_Paths").GetComponent<Tilemap>();
        gridVtL = GameObject.Find("Vertical_Paths").GetComponent<Tilemap>();
        gridHzW = GameObject.Find("Horizontal_Walls").GetComponent<Tilemap>();
        gridVtW = GameObject.Find("Vertical_Walls").GetComponent<Tilemap>();
    }
}
public class MazeBuilder2 : MazeStartUp
{
    private bool pathVisible, wallColision = true;

    [Header("Offsets")]
    [SerializeField]
    private float finishOffsetX;

    [SerializeField]
    private float finishOffsetY;

    [Header("Tiles")]
    [SerializeField]
    public Tile tilePath;

    [SerializeField]
    public Tile tileHzW;

    [SerializeField]
    public Tile tileVtW;

    [SerializeField]
    public Tile tileHzL;

    [SerializeField]
    public Tile tileVtL;

    [SerializeField]
    private Tile tileBlack;

    private int pathIdx;

    private List<Vector2> pathPositions = new List<Vector2>();

    private List<Vector2> corectPathPositions = new List<Vector2>();

    private List<Vector3> wallPositions = new List<Vector3>();

    static public List<Vector2> enemyPositions { private set; get; }

    static public List<Vector2> maskPositions { private set; get; }

    [SerializeField] private List<Vector2> tempEnemyPositions;

    [SerializeField] private List<Vector2> tempMaskPositions;

    public void MazeAwake()
    {
        enemyPositions = new List<Vector2>();
        maskPositions = new List<Vector2>();
        tempEnemyPositions = enemyPositions;
        tempMaskPositions = maskPositions;
        
        GetComponent<MazeSetup>().SetupCorners();
        SetupGridsAndTiles();
        SetupBorders();
        MazeSetup.SetCorner();
        placePath(gridX, gridY);

        while (pathIdx >= 0)
        {
            MazeBuilding();
            CalculateDistance();
        }

        SetupShadows();
        AddCorectRoute();
        SetupPlayerAndFinish();
        MazeSetup.SetupEnemy();
        MazeSetup.MaskSetup();
    }

    public void SetupBorders()
    {
        Tile[] tiles = new Tile[3];
        tiles[0] = tileBlack;
        tiles[1] = tileHzW;
        tiles[2] = tileVtW;

        MazeSetup.SetBorders(tiles);
    }

    private void SetupShadows()
    {
        GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[0].Create();
        GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[1].Create();
        GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[0].deleteShadows();
        GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[0].Create();
    }

    private void SetupPlayerAndFinish()
    {
        finishCollectible = GameObject.Find("Finish_Collectible");
        float finishX = corectPathPositions[0].x * gridDimension + finishOffsetX;
        float finishY = corectPathPositions[0].y * gridDimension + finishOffsetY;
        finishCollectible.transform.position = new Vector2(finishX, finishY);

        GameObject.Find("Nurse").GetComponent<PlayerMovement>().SetupPlayer();
        playerX = corectPathPositions[corectPathPositions.Count - 1].x * gridDimension;
        playerY = corectPathPositions[corectPathPositions.Count - 1].y * gridDimension;
        PlayerMovement.startPlayer(playerX, playerY);
    }

    private void MazeBuilding()
    {
        if (!backtrack) placeMaze(gridX + (int)MathF.Sin(direction), gridY + (int)MathF.Cos(direction));
        else placeMaze(gridX, gridY);
    }

    private void placeMaze(int x, int y)
    {
        if (backtrack)
        {
            CalculateVirusPos();
            maskDistance = 0;

            pathPositions.RemoveAt(pathIdx);
            pathIdx--;
            if (pathIdx > -1)
            {
                gridX = (int)pathPositions[pathIdx].x;
                gridY = (int)pathPositions[pathIdx].y;
            }
            direction = ChooseDirection(gridX, gridY);
            return;
        }

        CalculateMaskPos();
        enemyDistance = 0;

        placePath(x, y);
        placeWall(x, y);
        
        gridX = x;
        gridY = y;
        direction = ChooseDirection(x, y);
    }

    private void CalculateVirusPos()
    {
        enemyDistance++;
        if (enemyDistance == 1)
            enemyPositions.Add(pathPositions[pathIdx]);
    }

    private void CalculateMaskPos()
    {
        if (maskDistance == 0)
            maskPositions.Add(pathPositions[pathIdx]);
        maskDistance++;
    }

    private float ChooseDirection(int x, int y)
    {
        Tile[] tiles = new Tile[2];
        tiles[0] = tilePath;
        tiles[1] = tileBlack;
        int idx = UnityEngine.Random.Range(0, 4);
        backtrack = false;
        for (int i = 0; i < 4 && isTile(x, y, idx % 4, tiles); i++)
        {
            if (i == 3) backtrack = true;
            idx++;
        }
        return PIes[idx % 4];
    }

    private bool isTile(int x, int y, int idx, Tile[] tiles) =>
        TilesManager.detectTilesAt(x + (int)MathF.Sin(PIes[idx]), y + (int)MathF.Cos(PIes[idx]), tiles, gridPath);


    private void placePath(int x, int y) 
    {
        if (!TilesManager.detectTileAt(x, y, tilePath, gridPath))
        {
            TilesManager.SetMapTile(x, y, tilePath, gridPath);
            pathPositions.Add(new Vector2(x, y));
            pathIdx = pathPositions.Count - 1;
        }
    }

    private void placeWall(int x, int y)
    {
        int wallX = x, wallY = y;
        int sideX = 0, sideY = 0;
        int backX = 0, backY = 0;
        Tile tileSide, tileBack;
        Tilemap tilemapSide, tilemapBack;
        int tileSideId, tileBackId;

        if ((int)MathF.Sin(direction) == 0)
        {
            tileSide = tileVtW;
            tileSideId = 1;
            tilemapSide = gridVtW;
            tileBack = tileHzW;
            tileBackId = 2;
            tilemapBack = gridHzW;
        }
        else
        {
            tileSide = tileHzW;
            tileSideId = 2;
            tilemapSide = gridHzW;
            tileBack = tileVtW;
            tileBackId = 1;
            tilemapBack = gridVtW;
        }

        for (int i = 0; i < 2; i++) 
        {
            if ((int)MathF.Sin(direction) == 0)
            {
                wallY = y - (int)MathF.Cos(direction);
                sideX = i;
                if (sideX == 0) sideX = -1;
                backY = (int)MathF.Cos(direction + Mathf.PI);
            }
            else
            {
                wallX = x - (int)MathF.Sin(direction);
                backX = (int)MathF.Sin(direction + Mathf.PI);
                sideY = i;
                if (sideY == 0) sideY = -1;
            }
            
            if (!TilesManager.detectTileAt(wallX + sideX, wallY + sideY, tilePath, gridPath))
            {  
                if (sideX < 0) sideX++;
                if (sideY < 0) sideY++;
                if (TilesManager.SetMapTile(wallX + sideX, wallY + sideY, tileSide, tilemapSide))
                    wallPositions.Add(new Vector3(wallX + sideX, wallY + sideY, tileSideId));
            }

            if (!TilesManager.detectTileAt(wallX + backX, wallY + backY, tilePath, gridPath)) 
            {
                if (backX > 0) backX++;
                if (backY > 0) backY++;
                if (TilesManager.SetMapTile(x + backX, y + backY, tileBack, tilemapBack))
                    wallPositions.Add(new Vector3(x + backX, y + backY, tileBackId));
                if (backX > 0) backX--;
                if (backY > 0) backY--;
            }

            if (backX < 0) backX++;
            if (backY < 0) backY++;
            if (TilesManager.SetMapTile(x + backX, y + backY, null, tilemapBack))
                wallPositions.Remove(new Vector3(x + backX, y + backY, tileBackId));
        }
    }

    private void CalculateDistance()
    {
        if (maxDistance < pathPositions.Count)
        {
            corectPathPositions.Clear();
            foreach(Vector2 position in pathPositions)
            {
                corectPathPositions.Add(position);
            }
            maxDistance++;
        }
    }

    private void AddCorectRoute()
    {
        for(int i = 0; i < corectPathPositions.Count - 1; i++)
        {
            Tile line = null;
            Tilemap lineMap = null;
            int offsetX = 0, offsetY = 0;
            int x, y;
            if (corectPathPositions[i].x - corectPathPositions[i + 1].x != 0) 
            {
                line = tileHzL;
                lineMap = gridHzL;
                if (corectPathPositions[i].x - corectPathPositions[i + 1].x > 0) offsetX--;
            }
            if (corectPathPositions[i].y - corectPathPositions[i + 1].y != 0) 
            {
                line = tileVtL;
                lineMap = gridVtL;
                if (corectPathPositions[i].y - corectPathPositions[i + 1].y < 0) offsetY++;
            }
            x = (int)corectPathPositions[i + 1].x - offsetX;
            y = (int)corectPathPositions[i + 1].y - offsetY;
            TilesManager.SetMapTile(x, y, line, lineMap);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pathVisible = !pathVisible;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            wallColision = !wallColision;
        }

        if (pathVisible)
        {
            gridHzL.gameObject.GetComponentInChildren<TilemapRenderer>().sortingLayerName = "Maze";
            gridVtL.gameObject.GetComponentInChildren<TilemapRenderer>().sortingLayerName = "Maze";
        }
        else
        {
            gridHzL.gameObject.GetComponentInChildren<TilemapRenderer>().sortingLayerName = "Invisible";
            gridVtL.gameObject.GetComponentInChildren<TilemapRenderer>().sortingLayerName = "Invisible";
        }

        if (wallColision)
        {
            CompositeCollider2D[] componentsInChildren = GameObject.Find("MazeWalls").GetComponentsInChildren<CompositeCollider2D>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].isTrigger = false;
            }
        }
        else
        {
            CompositeCollider2D[] componentsInChildren2 = GameObject.Find("MazeWalls").GetComponentsInChildren<CompositeCollider2D>();
            for (int j = 0; j < componentsInChildren2.Length; j++)
            {
                componentsInChildren2[j].isTrigger = true;
            }
        }
    }
}
