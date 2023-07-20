// Finish
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Finish : CollectiblesManager
{
	private Text levelAmount, difficulty;

	static public int coinsAmount;

	[SerializeField]
	private AudioClip winSound;

	[SerializeField]
	private AudioClip loseSound;

	private GameObject coins;

	private Text coinsQuantity;

	private Animator anim;

	public static Finish finishScript { get; private set; }

	public static GameObject[] panels { get; private set; }

	private void Awake()
	{
		panels = new GameObject[2];
		finishScript = this;
		coinsAmount = 10;
		levelAmount = GameObject.Find("LevelAmount").GetComponent<Text>();
        difficulty = GameObject.Find("Difficulty").GetComponent<Text>();
        panels[0] = GameObject.Find("GameWin");
		panels[0].SetActive(false);
		panels[1] = GameObject.Find("GameOver");
		panels[1].SetActive(false);
		coins = GameObject.Find("Coins");
		coinsQuantity = coins.GetComponentInChildren<Text>();
		anim = coins.GetComponentInChildren<Animator>();
		coins.SetActive(false);
	}

	public int gridX()
	{
		return (int)Mathf.Floor(transform.position.x / MazeBuilder2.gridDimension);
	}

	public int gridY()
	{
		return (int)Mathf.Round(transform.position.y / MazeBuilder2.gridDimension);
	}

	private void Update()
	{
		/*if (Input.GetKeyDown(KeyCode.N))
		{
			RestartScene();
		}*/
		levelAmount.text = "Level: " + GamePropieties.level;
        StartCoroutine(WebConnection.GetDifficultyData());
        difficulty.text = WebConnection.difficulty;
		if (!Lantern.isLighting)
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
		if (GamePropieties.win && !panels[0].activeSelf)
		{
			spawnPanel(true);
		}
		if (GamePropieties.lose && !panels[1].activeSelf)
		{
			spawnPanel(false);
		}
	}

	private bool isOnScreen()
	{
		if (transform.position.x > CameraMovement.rightWall)
		{
			return false;
		}
		if (transform.position.y > CameraMovement.upWall)
		{
			return false;
		}
		if (transform.position.x < CameraMovement.leftWall)
		{
			return false;
		}
		if (transform.position.y < CameraMovement.downWall)
		{
			return false;
		}
		return true;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		despawnObject(gameObject);
		GamePropieties.SetWin(true);
    }

	public void spawnPanel(bool win)
	{
		Time.timeScale = 0f;
		GameSettings.audioSource[0].Stop();
		GamePropieties.SetDisplayTime(GamePropieties.displayTime + Timer.time);
		if (win)
		{
			if (GamePropieties.highestLevel < GamePropieties.level)
			{
				if (GamePropieties.lowestTime <= GamePropieties.displayTime)
				{
					GamePropieties.SetLowestTime(GamePropieties.displayTime);
				}
				GamePropieties.SetHighestLevel(GamePropieties.level);
			}
			else if (GamePropieties.highestLevel == GamePropieties.level 
				&& GamePropieties.lowestTime > GamePropieties.displayTime)
			{
				GamePropieties.SetLowestTime(GamePropieties.displayTime);
			}
			StartCoroutine(WebConnection.UpdateData(GamePropieties.highestLevel, GamePropieties.lowestTime));
			panels[0].SetActive(true);
			GameSettings.PlaySound(winSound);
			StartCoroutine(Coin());
		}
		else
		{
			panels[1].SetActive(true);
			GameSettings.PlaySound(loseSound);
		}
	}

	private IEnumerator Coin()
	{
		coins.SetActive(true);
		_ = anim.runtimeAnimatorController.animationClips[1];
		for (float j = 0f; j <= 1f; j += 0.0825f)
		{
			anim.Play("PopUp", 0, j);
			yield return new WaitForSecondsRealtime(0.0625f);
		}
		float coinMultiplier = 1;
		if (GameSettings.darkness) coinMultiplier = 1.5f;
		coinsQuantity.text = "+" + (int)Mathf.Ceil(coinsAmount * coinMultiplier);
		while (GamePropieties.win || GamePropieties.lose)
		{
			for (float j = 0f; j <= 1f; j += 0.09f)
			{
				if (j != 0f)
				{
					yield return new WaitForSecondsRealtime(0.0675f);
				}
				anim.Play("RestRotation", 0, j);
			}
		}
	}

	public async void RestartScene()
	{
		LevelSetup.newGame = true;
		GameSettings.PlaySound(SceneLoader.buttonPressedClip);
		await Task.Delay(100);
        GamePropieties.SetWin(false);
        GamePropieties.SetLose(false);
        GamePropieties.SetLevel(GamePropieties.level + 1);
        float coinMultiplier = 1;
        if (GameSettings.darkness) coinMultiplier = 1.5f;
        ShopManager.incrementCoins((int)Mathf.Ceil(coinsAmount * coinMultiplier));
		SceneLoader.sceneLoaderScript.ReloadScene();
	}
}
