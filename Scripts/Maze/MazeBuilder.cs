// MazeBuilder
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeBuilder : MonoBehaviour
{
    private Tilemap gridHzL;

	private Tilemap gridVtL;

	private GameObject finishCollect;

	private float dir;

	private int[] virusX;

	private int[] virusY;

	private int[] maskX;

	private int[] maskY;

	private float[] PIes = new float[4]
	{
		0f,
		MathF.PI / 2f,
		-MathF.PI / 2f,
		MathF.PI
	};

	private float[] dirs = new float[1];

	private int gridX;

	private int gridY;

	private int dirIdx = 1;

	private int maxDis;

	private int backDis;

	private int stepDis;

	private bool backtracking;

	private bool first = true;

	private bool backtracked;

	private bool pathVisible;

	private bool wallColision = true;

	private bool spawnDiference;

	private int enemyesSpawned;

	private int masksSpawned;

	private int spawnChecks;

	private int closestEnemy;

	private static int enemyAmount;

	private static int expansion;

	private float enemyChance;

    [SerializeField]
	private MazeSaver mazeSaver;

	[Header("Offsets")]
	[SerializeField]
	private float finishOffsetX;

	[SerializeField]
	private float finishOffsetY;

	[Header("Tiles")]
	public Tile tilePath;

	public Tile tileHzW;

	public Tile tileVtW;

	[SerializeField]
	private Tile tileHzL;

	[SerializeField]
	private Tile tileVtL;

	[SerializeField]
	private Tile tileBlack;

	[Header("Corners")]
	[SerializeField]
	private int up;

	[SerializeField]
	private int right;

	[SerializeField]
	private int down;

	[SerializeField]
	private int left;

	public static MazeBuilder mazeBuilderScript { get; private set; }

	static public Tilemap gridPath { get; private set; }

    static public Tilemap gridHzW { get; private set; }

    static public Tilemap gridVtW { get; private set; }

	public float playerX { get; private set; }

	public float playerY { get; private set; }

	static public float gridDimension { get; private set; }

	public static bool spawnCamp { get; private set; }

    private void Awake()
	{
        Time.timeScale = 1f;
		int enemyGrowth = 5;
        switch (SceneLoader.difficulty)
        {
            case "Easy":
                enemyGrowth = 7;
                break;
            case "Normal":
                enemyGrowth = 5;
                break;
            case "Hard":
                enemyGrowth = 4;
                break;
        }
        enemyAmount = (int)Mathf.Floor((2 + PlayerPrefs.GetInt("Level", 1)) / enemyGrowth);
        mazeBuilderScript = this;
        gridPath = GameObject.Find("Paths").GetComponent<Tilemap>();
		gridDimension = gridPath.GetComponentInParent<Grid>().cellSize.x + gridPath.GetComponentInParent<Grid>().cellGap.x;
		gridHzL = GameObject.Find("Horizontal_Paths").GetComponent<Tilemap>();
		gridVtL = GameObject.Find("Vertical_Paths").GetComponent<Tilemap>();
		gridHzW = GameObject.Find("Horizontal_Walls").GetComponent<Tilemap>();
		gridVtW = GameObject.Find("Vertical_Walls").GetComponent<Tilemap>();
        finishCollect = GameObject.Find("Finish_Collectible");
		virusX = new int[enemyAmount];
		for (int i = 0; i < virusX.Length; i++)
		{
			virusX[i] = 100;
		}
		virusY = new int[enemyAmount];
		for (int j = 0; j < virusY.Length; j++)
		{
			virusY[j] = 100;
		}
		maskX = new int[enemyAmount];
		for (int k = 0; k < maskX.Length; k++)
		{
			maskX[k] = 100;
		}
		maskY = new int[enemyAmount];
		for (int l = 0; l < maskY.Length; l++)
		{
			maskY[l] = 100;
		}
		spawnCamp = false;
}

    private IEnumerator Start()
	{
		PlayerPrefs.SetInt("NewLevel", 0);
		chooseCorner();
		placeTile(gridX, gridY);
		finishCollect.transform.position = new Vector3((float)gridX * gridDimension + finishOffsetX, (float)gridY * gridDimension + finishOffsetY, 0f);
		do
		{
			mazeBuilding();
		}
		while (dirIdx > 1);
        GameObject.Find("Nurse").GetComponent<PlayerMovement>().SetupPlayer();
        PlayerMovement.startPlayer(playerX, playerY);
		for (int i = 0; i < enemyAmount; i++)
		{
			if (virusX[i] != 100 && virusY[i] != 100)
			{
                GameObject.Find("Enemyes").GetComponent<EnemyesSpawn>().VirusSetup();

                EnemyesSpawn.spawnVirus(new Vector2(virusX[i], virusY[i]));
				Finish.coinsAmount += 10;
			}
		}
		for (int j = 0; j < masksSpawned; j++)
		{
			if (maskX[j] != 100 && maskY[j] != 100 && GameObject.FindGameObjectsWithTag("Enemy").Length > 1)
			{
                GameObject.Find("MaskHollder").GetComponent<MaskSpawner>().SetupMask();

                MaskSpawner.MaskSpawn(new Vector2(maskX[j], maskY[j]));
			}
		}
		yield return new WaitForSeconds(0f);
		addShadows();
	}

	private void addShadows()
	{
		GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[0].Create();
		GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[1].Create();
		GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[0].deleteShadows();
		GameObject.Find("MazeWalls").GetComponentsInChildren<ShadowCreator>()[0].Create();
	}

	private void chooseCorner()
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
			expansion = (int)Mathf.Floor(PlayerPrefs.GetInt("Level", 1) / expansionGrowth);
		}
		else
		{
			expansion = 2 + (int)Mathf.Floor(PlayerPrefs.GetInt("Level", 1) / expansionGrowth * 2);
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			gridX = right + expansion;
			num2 = left - expansion;
			num = 2;
		}
		else
		{
			gridX = left - expansion;
			num2 = right + expansion;
			num = 1;
		}
		int num4 = 0;
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			gridY = up + expansion / 2;
			num3 = down - expansion / 2;
			num4 = 3;
		}
		else
		{
			gridY = down - expansion / 2;
			num3 = up + expansion / 2;
			num4 = 0;
		}
		if (70 - PlayerPrefs.GetInt("Level") >= 0)
		{
			if (UnityEngine.Random.Range(0, 70 - PlayerPrefs.GetInt("Level")) == 0)
			{
				Lantern.lanternScript.activateLantern(num2, num3);
			}
		}
		else
		{
			Lantern.lanternScript.activateLantern(num2, num3);
		}
		if (expansion < 1)
		{
			TilesManager.fillTiles(gridPath, tileBlack, new Vector2(-11, -5), new Vector2(10, 5));
            TilesManager.fillTiles(gridHzW, tileHzW, new Vector2(-6, -2), new Vector2(5, 3));
            TilesManager.fillTiles(gridVtW, tileVtW, new Vector2(-6, -2), new Vector2(6, 2));
		}
		else
		{
            TilesManager.fillTiles(gridPath, tileBlack, new Vector2(left - 4 - expansion, down - 3 - expansion / 2), new Vector2(right + 4 + expansion, up + 3 + expansion / 2));
            TilesManager.fillTiles(gridHzW, tileHzW, new Vector2(left - expansion, down - expansion / 2), new Vector2(right + expansion, up + 1 + expansion / 2));
            TilesManager.fillTiles(gridVtW, tileVtW, new Vector2(left - expansion, down - expansion / 2), new Vector2(right + 1 + expansion, up + expansion / 2));
		}
        TilesManager.fillTiles(gridPath, null, new Vector2(left - expansion, down - expansion / 2), new Vector2(right + expansion, up + expansion / 2));
        TilesManager.fillTiles(gridHzW, null, new Vector2(left - expansion, down + 1 - expansion / 2), new Vector2(right + expansion, up + expansion / 2));
        TilesManager.fillTiles(gridVtW, null, new Vector2(left + 1 - expansion, down - expansion / 2), new Vector2(right + expansion, up + expansion / 2));
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			dir = PIes[num];
		}
		else
		{
			dir = PIes[num4];
		}
	}

	private void mazeBuilding()
	{
		if (!backtracking)
		{
			stepDis = dirIdx;
			dirIdx++;
			placeTile(gridX + (int)Mathf.Sin(dir), gridY + (int)Mathf.Cos(dir));
		}
		else if (dirs.Length > 1)
		{
			dirIdx--;
			placeTile(gridX - (int)Mathf.Sin(dirs[dirIdx]), gridY - (int)Mathf.Cos(dirs[dirIdx]));
		}
	}

	private void placeTile(int x, int y)
	{
		if (enemyesSpawned < enemyAmount && stepDis == dirIdx)
		{
			float enemyChanceGrowth = 25;
            switch (SceneLoader.difficulty)
            {
                case "Easy":
                    enemyChanceGrowth = 12;
                    break;
                case "Normal":
                    enemyChanceGrowth = 25;
                    break;
                case "Hard":
                    enemyChanceGrowth = 37;
                    break;
            }
            if (enemyChance % (int)Mathf.Ceil((float)PlayerPrefs.GetInt("Level", 1) / enemyChanceGrowth) == 0f)
			{
				virusX[enemyesSpawned] = gridX;
				virusY[enemyesSpawned] = gridY;
				enemyesSpawned++;
				closestEnemy = enemyesSpawned;
			}
			enemyChance += 1f;
		}
		gridX = x;
		gridY = y;
		if (maxDis < dirIdx)
		{
			maxDis = dirIdx;
			backDis = dirIdx;
			playerX = (float)gridX * gridDimension;
			playerY = (float)gridY * gridDimension;
			spawnCamp = false;
			spawnChecks = 0;
			for (int i = -11; i <= 11; i++)
			{
				for (int j = -7; j <= 7; j++)
				{
					gridHzL.SetTile(new Vector3Int(i, j), null);
					gridVtL.SetTile(new Vector3Int(i, j), null);
				}
			}
		}
		if (backtracking && maxDis >= dirIdx && backDis > dirIdx && dirIdx > 0)
		{
			backDis = dirIdx;
			if (dirs[dirIdx] == PIes[0])
			{
				gridVtL.SetTile(new Vector3Int(x, y), tileVtL);
			}
			if (dirs[dirIdx] == PIes[1])
			{
				gridHzL.SetTile(new Vector3Int(x + 1, y), tileHzL);
			}
			if (dirs[dirIdx] == PIes[2])
			{
				gridHzL.SetTile(new Vector3Int(x, y), tileHzL);
			}
			if (dirs[dirIdx] == PIes[3])
			{
				gridVtL.SetTile(new Vector3Int(x, y - 1), tileVtL);
			}
			if (closestEnemy > 0 && (float)virusX[closestEnemy - 1] == playerX / gridDimension && (float)virusY[closestEnemy - 1] == playerY / gridDimension)
			{
				virusX[closestEnemy - 1] = gridX;
				virusY[closestEnemy - 1] = gridY;
			}
			if (spawnChecks < 5 && closestEnemy > 0)
			{
				if ((Mathf.Abs(virusX[closestEnemy - 1] - gridX) < 2 && Mathf.Abs(virusY[closestEnemy - 1] - gridY) < 1) || (Mathf.Abs(virusX[closestEnemy - 1] - gridX) < 1 && Mathf.Abs(virusY[closestEnemy - 1] - gridY) < 2))
				{
					spawnCamp = true;
				}
				spawnChecks++;
			}
		}
		float[] array = new float[dirIdx];
		for (int k = 0; k < array.Length; k++)
		{
			if (k < dirs.Length)
			{
				array[k] = dirs[k];
			}
		}
		if (dirIdx >= dirs.Length)
		{
			array[dirIdx - 1] = dir;
		}
		dirs = new float[dirIdx];
		for (int l = 0; l < dirs.Length; l++)
		{
			dirs[l] = array[l];
		}
		if (gridPath.GetTile(new Vector3Int(x, y)) != tilePath && gridPath.GetTile(new Vector3Int(x, y)) != tileBlack)
		{
			gridPath.SetTile(new Vector3Int(x, y), tilePath);
			int idx = 2;
			if (first)
			{
				idx = 0;
			}
			if (!backtracking && !backtracked)
			{
				checkForWallInAllDirections(x, y, idx);
			}
		}
		first = false;
		backtracked = false;
		if (backtracking)
		{
			bool flag = false;
			if (TilesManager.detectTileAt(x + 1, y, null, gridPath))
			{
				gridVtW.SetTile(new Vector3Int(x + 1, y), null);
				flag = true;
			}
			if (TilesManager.detectTileAt(x - 1, y, null, gridPath))
			{
				gridVtW.SetTile(new Vector3Int(x, y), null);
				flag = true;
			}
			if (TilesManager.detectTileAt(x, y + 1, null, gridPath))
			{
				gridHzW.SetTile(new Vector3Int(x, y + 1), null);
				flag = true;
			}
			if (TilesManager.detectTileAt(x, y - 1, null, gridPath))
			{
				gridHzW.SetTile(new Vector3Int(x, y), null);
				flag = true;
			}
			if (flag)
			{
				if (enemyAmount > 0 && spawnDiference && enemyAmount - enemyesSpawned < enemyAmount && masksSpawned < enemyAmount)
				{
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
                    if (masksSpawned % maskGrowth < 1)
					{
						maskX[masksSpawned] = x;
						maskY[masksSpawned] = y;
					}
					masksSpawned++;
				}
				spawnDiference = true;
			}
			backtracked = true;
			backtracking = false;
		}
		do
		{
			dir = PIes[UnityEngine.Random.Range(0, 4)];
			isWall(x, y);
		}
		while (!backtracking && isWall(x, y));
	}

	private void checkForWallInAllDirections(int x, int y, int idx)
	{
		if (dir == PIes[0])
		{
			if (checkForWall(1, 1, idx))
			{
				gridVtW.SetTile(new Vector3Int(x, y - 1), tileVtW);
			}
			if (checkForWall(2, 2, idx))
			{
				gridVtW.SetTile(new Vector3Int(x + 1, y - 1), tileVtW);
			}
			if (checkForWall(0, 3, idx))
			{
				gridHzW.SetTile(new Vector3Int(x, y - 1), tileHzW);
			}
		}
		if (dir == PIes[1])
		{
			if (checkForWall(0, 0, idx))
			{
				gridHzW.SetTile(new Vector3Int(x - 1, y), tileHzW);
			}
			if (checkForWall(3, 3, idx))
			{
				gridHzW.SetTile(new Vector3Int(x - 1, y + 1), tileHzW);
			}
			if (checkForWall(1, 2, idx))
			{
				gridVtW.SetTile(new Vector3Int(x - 1, y), tileVtW);
			}
		}
		if (dir == PIes[2])
		{
			if (checkForWall(0, 0, idx))
			{
				gridHzW.SetTile(new Vector3Int(x + 1, y), tileHzW);
			}
			if (checkForWall(3, 3, idx))
			{
				gridHzW.SetTile(new Vector3Int(x + 1, y + 1), tileHzW);
			}
			if (checkForWall(1, 2, idx))
			{
				gridVtW.SetTile(new Vector3Int(x + 2, y), tileVtW);
			}
		}
		if (dir == PIes[3])
		{
			if (checkForWall(1, 1, idx))
			{
				gridVtW.SetTile(new Vector3Int(x, y + 1), tileVtW);
			}
			if (checkForWall(2, 2, idx))
			{
				gridVtW.SetTile(new Vector3Int(x + 1, y + 1), tileVtW);
			}
			if (checkForWall(0, 3, idx))
			{
				gridHzW.SetTile(new Vector3Int(x, y + 2), tileHzW);
			}
		}
	}

	private bool checkForWall(int lDir1, int lDir2, int idx)
	{
		if (idx > 0 && dirs[dirIdx - idx] != PIes[lDir1] && dirs[dirIdx - idx] != PIes[lDir2])
		{
			return true;
		}
		return false;
	}

	private bool isWall(int x, int y)
	{
		if (detectAllSides(x, y))
		{
			backtracking = true;
			return true;
		}
		if (detectTileAtAllDirections(x, y, tileBlack))
		{
			return true;
		}
		if (detectTileAtAllDirections(x, y, tilePath))
		{
			return true;
		}
		return false;
	}

	private bool detectAllSides(int x, int y)
	{
		bool upWardsCheck = TilesManager.detectTileAt(x, y + 1, tilePath, gridPath) || TilesManager.detectTileAt(x, y + 1, tileBlack, gridPath);
		bool downWardsCheck = TilesManager.detectTileAt(x, y - 1, tilePath, gridPath) || TilesManager.detectTileAt(x, y - 1, tileBlack, gridPath);
		bool leftWardsCheck = TilesManager.detectTileAt(x - 1, y, tilePath, gridPath) || TilesManager.detectTileAt(x - 1, y, tileBlack, gridPath);
		bool rightWardsCheck = TilesManager.detectTileAt(x + 1, y, tilePath, gridPath) || TilesManager.detectTileAt(x + 1, y, tileBlack, gridPath);

        if (upWardsCheck && rightWardsCheck && leftWardsCheck && downWardsCheck)
		{
			return true;
		}
		return false;
	}

	private bool detectTileAtAllDirections(int x, int y, Tile tile)
	{
		if (dir == PIes[0] && (TilesManager.detectTileAt(x, y + 1, tileHzW, gridHzW) || TilesManager.detectTileAt(x, y + 1, tile, gridPath)))
		{
			return true;
		}
		if (dir == PIes[1] && (TilesManager.detectTileAt(x + 1, y, tileVtW, gridVtW) || TilesManager.detectTileAt(x + 1, y, tile, gridPath)))
		{
			return true;
		}
		if (dir == PIes[2] && (TilesManager.detectTileAt(x, y, tileVtW, gridVtW) || TilesManager.detectTileAt(x - 1, y, tile, gridPath)))
		{
			return true;
		}
		if (dir == PIes[3] && (TilesManager.detectTileAt(x, y, tileHzW, gridHzW) || TilesManager.detectTileAt(x, y - 1, tile, gridPath)))
		{
			return true;
		}
		return false;
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
