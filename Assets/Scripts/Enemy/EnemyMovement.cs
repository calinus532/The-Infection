// EnemyMovement
using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	private float startX;

	private float startY;

	private float time;

	private float dir;

	private float startTime;

	[SerializeField]
	private float cameraOffsetX;

	[SerializeField]
	private float cameraOffsetY;

	private Vector2 finishVc;

	private float[] PIes = new float[4]
	{
		0f,
		MathF.PI / 2f,
		-MathF.PI / 2f,
		MathF.PI
	};

	[SerializeField]
	private AudioClip damageSound;

	private void Awake()
	{
		startX = gridX() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
		startY = gridY() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
		finishVc = calculationVc();
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
		if (!SceneLoader.darkness && !Lantern.isLighting)
		{
			if (!isOnScreen())
			{
				Lantern.activateDeactivateLights(state: false, base.gameObject);
			}
			else
			{
				Lantern.activateDeactivateLights(state: true, base.gameObject);
			}
		}
		else
		{
			Lantern.activateDeactivateLights(state: false, base.gameObject);
		}
	}

	private bool isOnScreen()
	{
		if (base.transform.position.x >= CameraMovement.rightWall + cameraOffsetX)
		{
			return false;
		}
		if (base.transform.position.y >= CameraMovement.upWall - 0.375f + cameraOffsetY)
		{
			return false;
		}
		if (base.transform.position.x <= CameraMovement.leftWall - cameraOffsetX)
		{
			return false;
		}
		if (base.transform.position.y <= CameraMovement.downWall - cameraOffsetY)
		{
			return false;
		}
		return true;
	}

	private int gridX()
	{
		return (int)Mathf.Floor(base.transform.position.x / MazeBuilder2.gridDimension);
	}

	private int gridY()
	{
		return (int)Mathf.Round(base.transform.position.y / MazeBuilder2.gridDimension);
	}

	private void Movement()
	{
		Vector2 a = new Vector2(startX, startY);
		time += Time.deltaTime * EnemyesSpawn.speed;
		base.transform.position = Vector2.Lerp(a, finishVc, time);
		if (time >= 1f)
		{
			startX = (float)gridX() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
			startY = (float)gridY() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
			finishVc = calculationVc();
			CheckDirection();
			time = 0f;
		}
	}

	private Vector2 calculationVc()
	{
		float x = (float)gridX() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
		float y = (float)gridY() * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
		bool flag = false;
		bool flag2 = false;
		if (Up() && Down())
		{
			if (dir == PIes[1])
			{
				x = (Right() ? ((float)(gridX() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX) : ((float)(gridX() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX));
				flag = true;
			}
			if (dir == PIes[2])
			{
				x = (Left() ? ((float)(gridX() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX) : ((float)(gridX() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX));
				flag = true;
			}
		}
		if (Right() && Left())
		{
			if (dir == PIes[0])
			{
				y = (Up() ? ((float)(gridY() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY) : ((float)(gridY() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY));
				flag2 = true;
			}
			if (dir == PIes[3])
			{
				y = (Down() ? ((float)(gridY() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY) : ((float)(gridY() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY));
				flag2 = true;
			}
		}
		while (!flag && !flag2)
		{
			int num = UnityEngine.Random.Range(1, 5);
			if (num == 1 && dir != PIes[3] && !Up())
			{
				y = (float)(gridY() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
				flag2 = true;
			}
			if (num == 2 && dir != PIes[2] && !Right())
			{
				x = (float)(gridX() + 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
				flag = true;
			}
			if (num == 3 && dir != PIes[0] && !Down())
			{
				y = (float)(gridY() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetY;
				flag2 = true;
			}
			if (num == 4 && dir != PIes[1] && !Left())
			{
				x = (float)(gridX() - 1) * MazeBuilder2.gridDimension + EnemyesSpawn.offsetX;
				flag = true;
			}
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
		{
			return true;
		}
		return false;
	}

	private bool Right()
	{
		if (TilesManager.detectTileAt(gridX() + 1, gridY(), GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileVtW, MazeBuilder2.gridVtW))
		{
			return true;
		}
		return false;
	}

	private bool Down()
	{
		if (TilesManager.detectTileAt(gridX(), gridY(), GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileHzW, MazeBuilder2.gridHzW))
		{
			return true;
		}
		return false;
	}

	private bool Left()
	{
		if (TilesManager.detectTileAt(gridX(), gridY(), GameObject.Find("MazeBuilder").GetComponent<MazeBuilder2>().tileVtW, MazeBuilder2.gridVtW))
		{
			return true;
		}
		return false;
	}
}
