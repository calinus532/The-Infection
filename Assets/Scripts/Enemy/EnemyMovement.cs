// EnemyMovement
using System;
using UnityEngine;

public class EnemyMovement : EnemyesSpawn
{
	private float startX;

	private float startY;

	private float timer;

	private int directionIdx;

	private float dir;

	private float startTime;

	private int index;

	[SerializeField]
	private float cameraOffsetX;

	[SerializeField]
	private float cameraOffsetY;

	private Vector2 finishVc;

	private readonly float[] PIes = new float[4]
	{
        2 * Mathf.PI,
		MathF.PI / 2f,
		-MathF.PI / 2f,
		MathF.PI
	};

	[SerializeField]
	private AudioClip damageSound;

	private void Awake()
	{
		index = idx;
		directionIdx = dirIdx;
		timer = time;
        startX = gridX() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
		startY = gridY() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
        finishVc = calculationVc(directionIdx);
        CheckDirection();
		Movement();
    }

    private void Update()
	{
		startTime += Time.deltaTime;
		if (startTime >= 0.75f)
		{
			Movement();
		}
		if (!GameSettings.darkness && !Lantern.isLighting)
		{
			if (!isOnScreen())
			{
				Lantern.activateDeactivateLights(false, gameObject);
			}
			else
			{
				Lantern.activateDeactivateLights(true, gameObject);
			}
		}
		else
		{
			Lantern.activateDeactivateLights(false, gameObject);
		}
	}

	private bool isOnScreen()
	{
		if (transform.position.x >= CameraMovement.rightWall + cameraOffsetX)
		{
			return false;
		}
		if (transform.position.y >= CameraMovement.upWall - 0.375f + cameraOffsetY)
		{
			return false;
		}
		if (transform.position.x <= CameraMovement.leftWall - cameraOffsetX)
		{
			return false;
		}
		if (transform.position.y <= CameraMovement.downWall - cameraOffsetY)
		{
			return false;
		}
		return true;
	}

	private int gridX() => (int)Mathf.Floor(transform.position.x / MazeBuilder2.gridDimension);

	private int gridY() => (int)Mathf.Round(transform.position.y / MazeBuilder2.gridDimension);

	private void Movement()
	{
		Vector2 start = new Vector2(startX, startY);
        timer += Time.deltaTime * EnemyesSpawn.speed;
		transform.position = Vector2.Lerp(start, finishVc, timer);
        if (timer >= 1f)
		{
            startX = (float)gridX() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
			startY = (float)gridY() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
            finishVc = calculationVc(0);
			CheckDirection();
			timer = 0f;
		}
        Vector4 vector4 = Vector4.zero;
        vector4.x = (startX - EnemyesSpawn.offsetX) / MazeBuilder2.gridDimension;
        vector4.y = (startY - EnemyesSpawn.offsetY) / MazeBuilder2.gridDimension;
        vector4.z = directionIdx;
        vector4.w = timer;
        virusPositions[index] = vector4;
    }

	private Vector2 calculationVc(int direction)
	{
		float x = (float)gridX() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
		float y = (float)gridY() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
		bool xAxis = false;
		bool yAxis = false;
		if (Up() && Down())
		{
			if (dir == PIes[1])
			{
				x = (Right() ? ((float)(gridX() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX) 
					: ((float)(gridX() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX));
				xAxis = true;
			}
			if (dir == PIes[2])
			{
				x = (Left() ? ((float)(gridX() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX) 
					: ((float)(gridX() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX));
				xAxis = true;
			}
		}
		if (Right() && Left())
		{
			if (dir == PIes[0])
			{
				y = (Up() ? ((float)(gridY() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY) 
					: ((float)(gridY() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY));
				yAxis = true;
			}
			if (dir == PIes[3])
			{
				y = (Down() ? ((float)(gridY() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY) 
					: ((float)(gridY() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY));
				yAxis = true;
			}
		}

		int firstDir = 0;
		while (!xAxis && !yAxis)
		{
			int idx = UnityEngine.Random.Range(1, 5);
			if (direction > 0 && firstDir == 0) idx = direction;
			firstDir++;
			if (idx == 1 && dir != PIes[3] && !Up())
			{
				y = (float)(gridY() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
				yAxis = true;
			}
			if (idx == 2 && dir != PIes[2] && !Right())
			{
				x = (float)(gridX() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
				xAxis = true;
			}
			if (idx == 3 && dir != PIes[0] && !Down())
			{
				y = (float)(gridY() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
				yAxis = true;
			}
			if (idx == 4 && dir != PIes[1] && !Left())
			{
				x = (float)(gridX() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
				xAxis = true;
			}
            directionIdx = idx;
        }
        return new Vector2(x, y);
	}

	private void CheckDirection()
	{
		if ((int)Mathf.Round(finishVc.y - startY) == 1)
		{
			dir = PIes[0];
		}
		if ((int)Mathf.Round(finishVc.y - startY) == -1)
		{
			dir = PIes[3];
		}
		if ((int)Mathf.Round(finishVc.x - startX) == 1)
		{
			dir = PIes[1];
		}
		if ((int)Mathf.Round(finishVc.x - startX) == -1)
		{
			dir = PIes[2];
		}
	}

	private void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player" && !CollectiblesManager.invincible)
		{
			HeartGenerator.heartGeneratorScript.takeDamage(1);
			StartCoroutine(PlayerMovement.playerMovementScript.IFrames());
			GameSettings.PlaySound(damageSound);
		}
	}

	private bool Up()
	{
		if (TilesManager.detectTileAt(gridX(), gridY() + 1, GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileHzW, MazeBuilder2.gridHzW))
			return true;
		return false;
	}

	private bool Right()
	{
		if (TilesManager.detectTileAt(gridX() + 1, gridY(), GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileVtW, MazeBuilder2.gridVtW))
			return true;
		return false;
	}

	private bool Down()
	{
		if (TilesManager.detectTileAt(gridX(), gridY(), GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileHzW, MazeBuilder2.gridHzW))
			return true;
		return false;
	}

	private bool Left()
	{
		if (TilesManager.detectTileAt(gridX(), gridY(), GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileVtW, MazeBuilder2.gridVtW))
			return true;
		return false;
	}
}
