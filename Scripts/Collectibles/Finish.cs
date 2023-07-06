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
		panels[0].SetActive(value: false);
		panels[1] = GameObject.Find("GameOver");
		panels[1].SetActive(value: false);
		coins = GameObject.Find("Coins");
		coinsQuantity = coins.GetComponentInChildren<Text>();
		anim = coins.GetComponentInChildren<Animator>();
		coins.SetActive(value: false);
	}

	public int gridX()
	{
		return (int)Mathf.Floor(base.transform.position.x / MazeBuilder.gridDimension);
	}

	public int gridY()
	{
		return (int)Mathf.Round(base.transform.position.y / MazeBuilder.gridDimension);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			RestartScene();
		}
		levelAmount.text = "Level: " + PlayerPrefs.GetInt("Level", 1);
        StartCoroutine(WebConnection.GetDifficultyData());
        difficulty.text = WebConnection.difficulty;
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
		if (PlayerPrefs.GetInt("Win", 0) == 1 && !panels[0].activeSelf)
		{
			spawnPanel(win: true);
		}
		if (PlayerPrefs.GetInt("Lose", 0) == 1 && !panels[1].activeSelf)
		{
			spawnPanel(win: false);
		}
	}

	private bool isOnScreen()
	{
		if (base.transform.position.x > CameraMovement.rightWall)
		{
			return false;
		}
		if (base.transform.position.y > CameraMovement.upWall)
		{
			return false;
		}
		if (base.transform.position.x < CameraMovement.leftWall)
		{
			return false;
		}
		if (base.transform.position.y < CameraMovement.downWall)
		{
			return false;
		}
		return true;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		despawnObject(base.gameObject);
		PlayerPrefs.SetInt("Win", 1);
	}

	public void spawnPanel(bool win)
	{
		Time.timeScale = 0f;
		GameSettings.audioSource[0].Stop();
		PlayerPrefs.SetFloat("Time", PlayerPrefs.GetFloat("Time") + Timer.time);
		if (win)
		{
			if (PlayerPrefs.GetInt("HighestLevel") < PlayerPrefs.GetInt("Level", 1))
			{
				if (PlayerPrefs.GetFloat("LowestTime") <= PlayerPrefs.GetFloat("Time"))
				{
					PlayerPrefs.SetFloat("LowestTime", PlayerPrefs.GetFloat("Time"));
				}
				PlayerPrefs.SetInt("HighestLevel", PlayerPrefs.GetInt("Level", 1));
			}
			else if (PlayerPrefs.GetInt("HighestLevel") == PlayerPrefs.GetInt("Level", 1) && PlayerPrefs.GetFloat("LowestTime") > PlayerPrefs.GetFloat("Time"))
			{
				PlayerPrefs.SetFloat("LowestTime", PlayerPrefs.GetFloat("Time"));
			}
			StartCoroutine(WebConnection.UpdateData(PlayerPrefs.GetInt("HighestLevel"), PlayerPrefs.GetFloat("LowestTime")));
			panels[0].SetActive(value: true);
			GameSettings.PlaySound(winSound);
			StartCoroutine(Coin());
		}
		else
		{
			panels[1].SetActive(value: true);
			GameSettings.PlaySound(loseSound);
		}
	}

	private IEnumerator Coin()
	{
		coins.SetActive(value: true);
		_ = anim.runtimeAnimatorController.animationClips[1];
		for (float j = 0f; j <= 1f; j += 0.0825f)
		{
			anim.Play("PopUp", 0, j);
			yield return new WaitForSecondsRealtime(0.0625f);
		}
		float coinMultiplier = 1;
		if (SceneLoader.darkness) coinMultiplier = 1.5f;
		coinsQuantity.text = "+" + (int)Mathf.Ceil(coinsAmount * coinMultiplier);
		while (PlayerPrefs.GetInt("Win") == 1 || PlayerPrefs.GetInt("Lose") == 1)
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
		GameSettings.PlaySound(SceneLoader.buttonPressedClip);
		await Task.Delay(100);
		PlayerPrefs.SetInt("NewLevel", 1);
		PlayerPrefs.SetInt("Win", 0);
		PlayerPrefs.SetInt("Lose", 0);
		PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
        float coinMultiplier = 1;
        if (SceneLoader.darkness) coinMultiplier = 1.5f;
        ShopManager.incrementCoins((int)Mathf.Ceil(coinsAmount * coinMultiplier));
		SceneLoader.sceneLoaderScript.ReloadScene();
	}
}
