// SceneLoader
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
	[SerializeField]
	private AudioClip buttonPressed;

	private GameObject menu, menuBg;

	private GameObject tutorial, tutorialBg, dificultyPanel;

	private static bool firstTime = true;

	public static AudioClip buttonPressedClip { get; private set; }

	public static float clipLength { get; private set; }
	static public string difficulty { private set; get; }

    private void Awake()
	{
        if (firstTime)
		{
			PlayerPrefs.SetFloat("MenuSongStart", 0f);
			PlayerPrefs.SetFloat("MazeSongStart", 0f);
			firstTime = false;
		}
		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
            menu = GameObject.Find("MainMenu");
			menuBg = GameObject.Find("MenuBackground");
            tutorial = GameObject.Find("TutorialMenu");
            tutorialBg = GameObject.Find("TutorialBackground");
            tutorial.SetActive(false);
			tutorialBg.SetActive(false);
			dificultyPanel = GameObject.Find("DifficultyPanel");
            GameSettings.hardmodeTg = GameObject.Find("Darkness").GetComponent<Toggle>();
            GameSettings.noEnemiesTg = GameObject.Find("NoEnemies").GetComponent<Toggle>();
            GameSettings.hardmodeTg.isOn = GamePropieties.darkness;
            GameSettings.noEnemiesTg.isOn = GamePropieties.noEnemies;
            dificultyPanel.SetActive(false);
            if (GamePropieties.userName != "") StartCoroutine(WebConnection.GetDifficultyData());
        }
		clipLength = buttonPressed.length;
		buttonPressedClip = buttonPressed;
	}

	private async void Start()
	{
		if (GamePropieties.health == 0)
		{
			GamePropieties.SetHealth(5);
		}
		GameSettings.SetAudioSources();
		await Task.Delay(100);
		StartCoroutine(GameSettings.CheckNewGame());
	}

	public async void ContinueGame()
	{
		difficulty = WebConnection.difficulty;
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(200);
		PlayerPrefs.SetInt("SongStart", 0);
		ReloadScene(false);
	}

	public async void Tutorial(bool isTutorial)
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		if (isTutorial)
		{
			menu.SetActive(false);
			menuBg.SetActive(false);
			tutorial.SetActive(true);
			tutorialBg.SetActive(true);
		}
		else
		{
			menu.SetActive(true);
			menuBg.SetActive(true);
			tutorial.SetActive(false);
			tutorialBg.SetActive(false);
		}
	}

	static public void ReloadScene(bool reset)
	{
		GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
        if (reset) Timer.ResetTime();
		SceneManager.LoadScene("Maze");
	}

	public async void NewGame(string dif)
	{
        StartCoroutine(WebConnection.GetScoreData());
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(200);
		StartCoroutine(INewGame(dif));
    }

	private IEnumerator INewGame(string dif)
	{
        StartCoroutine(Initialization(dif));
		yield return StartCoroutine(Initialization(dif));
        StartCoroutine(WebConnection.UpdateDificulty());
		yield return StartCoroutine(WebConnection.UpdateDificulty());
        LevelSetup.newGame = true;
        ReloadScene(true);
    }

	private IEnumerator Initialization(string dif)
	{
        difficulty = dif;
        PlayerPrefs.SetFloat("SongStart", 0f);
        int level = GamePropieties.highestLevel;
        float time = GamePropieties.lowestTime;
		if (WebConnection.difficulty != dif)
		{
			string[] records = WebConnection.currentScore.Split("|");
			string[][] levels = 
			{ 
				records[0].Split(";")[0].Split(","),
                records[0].Split(";")[1].Split(","),
                records[0].Split(";")[2].Split(",")
            };
            string[][] times = 
			{
				records[1].Split(";")[0].Split(","),
                records[1].Split(";")[1].Split(","),
                records[1].Split(";")[2].Split(",")
            };

			string finalLevel = string.Empty;
			string finalTime = string.Empty;
			for (int i = 0; i < 3; i++)
			{
				if (i == WebConnection.mazeType)
				{
                    switch (WebConnection.difficulty)
                    {
                        case "Easy":
							levels[i][0] = level.ToString();
							times[i][0] = time.ToString();
                            break;
                        case "Normal":
                            levels[i][1] = level.ToString();
                            times[i][1] = time.ToString();
                            break;
                        case "Hard":
                            levels[i][2] = level.ToString();
                            times[i][2] = time.ToString();
                            break;
                    }
                }

                finalLevel += levels[i][0] + "," + levels[i][1] + "," + levels[i][2];
                finalTime += times[i][0] + "," + times[i][1] + "," + times[i][2];
                if (i < 2)
				{
					finalLevel += ";";
					finalTime += ";";
				}
            }
            WebConnection.SetCurrentScore(finalLevel + "|" + finalTime);

            int mazeType = 0;
            if (GamePropieties.darkness) mazeType = 1;
            if (GamePropieties.noEnemies) mazeType = 2;
            switch (difficulty)
            {
                case "Easy":
                    level = int.Parse(levels[mazeType][0], CultureInfo.InvariantCulture.NumberFormat);
                    time = float.Parse(times[mazeType][0], CultureInfo.InvariantCulture.NumberFormat);
                    break;
                case "Normal":
                    level = int.Parse(levels[mazeType][1], CultureInfo.InvariantCulture.NumberFormat);
                    time = float.Parse(times[mazeType][1], CultureInfo.InvariantCulture.NumberFormat);
                    break;
                case "Hard":
                    level = int.Parse(levels[mazeType][2], CultureInfo.InvariantCulture.NumberFormat);
                    time = float.Parse(times[mazeType][2], CultureInfo.InvariantCulture.NumberFormat);
                    break;
            }
            StartCoroutine(WebConnection.UpdateScoreData(level, time));
			yield return StartCoroutine(WebConnection.UpdateScoreData(level, time));
        }
        GamePropieties.SetLevel(1);
        GamePropieties.SetWin(false);
        GamePropieties.SetLose(false);
		GamePropieties.SetDisplayTime(0);
        GamePropieties.SetHighestLevel(level);
        GamePropieties.SetLowestTime(time);
        GamePropieties.SetNewGame(true);
        GamePropieties.SetCoins(0);
		GamePropieties.SetHealth(5);
		GamePropieties.SetSpeed(0);
		GamePropieties.SetLightIntensity(0);
		GamePropieties.SetLights(0);
		GamePropieties.SetCollected(false);
		yield return null;
    }

	public async void RestartGame()
	{
		LevelSetup.newGame = true;
        PlayerPrefs.SetFloat("SongStart", 0f);
        GamePropieties.SetLevel(1);
        GamePropieties.SetLose(false);
        GamePropieties.SetDisplayTime(0);
		GamePropieties.SetHealth(5);
        GamePropieties.SetCollected(false);
        GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(200);
		ReloadScene(true);
	}

	static public async void LoadScene(string sceneName)
	{
        GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(300);
		SceneManager.LoadScene(sceneName);
	}

	public async void CloseApp()
	{
        GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(300);
		Application.Quit();
	}

	public async void ResumeGame()
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(200);
		PauseMenu.StartScene();
	}

	private void Update()
	{
        if (SceneManager.GetActiveScene().name == "Maze")
		{
			if (!Finish.panels[0].activeSelf && !Finish.panels[1].activeSelf)
			{
				if (!GamePropieties.newGame)
				{
					PauseGame();
				}
				if (GameSettings.audioSource[0].isPlaying)
				{
					PlayerPrefs.SetFloat("MazeSongStart", GameSettings.audioSource[0].time);
				}
			}
		}
		else
		{
			if (tutorial != null)
			{
				PauseGame();
			}
			if (GameSettings.audioSource[0].isPlaying)
			{
				PlayerPrefs.SetFloat("MenuSongStart", GameSettings.audioSource[0].time);
			}
		}

		if (Input.GetKeyDown(KeyCode.F11))
		{
			GameSettings.stateChanged = false;
			GameSettings.fullscreenTg.isOn = !GameSettings.fullscreenTg.isOn;
		}
		GameSettings.gameSettingsScript.Fullscreen();

		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			if (tutorial.activeSelf || dificultyPanel.activeSelf || UserEditor.panel.activeSelf)
				PauseMenu.StartScene();
			
			if (dificultyPanel.activeSelf)
				if (Input.GetKeyDown(KeyCode.Escape)) 
					dificultyPanel.SetActive(false);

            if (UserEditor.panel.activeSelf)
                if (Input.GetKeyDown(KeyCode.Escape))
                    UserEditor.panel.SetActive(false);
        }
    }

    private void PauseGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (PauseMenu.isPauseMenu)
			{
				PauseMenu.StopScene();
			}
			else
			{
				PauseMenu.StartScene();
			}
		}
	}

	static public async void ActivateDeactivateMenu()
	{
		if (SceneManager.GetActiveScene().name == "Maze")
		{
			if (!Finish.panels[0].activeSelf && !Finish.panels[1].activeSelf && !GamePropieties.newGame)
			{
				GameSettings.PlaySound(buttonPressedClip);
				await Task.Delay(200);
				if (PauseMenu.isPauseMenu)
				{
					PauseMenu.StopScene();
				}
				else
				{
					PauseMenu.StartScene();
				}
			}
		}
		else
		{
			GameSettings.PlaySound(buttonPressedClip);
			await Task.Delay(200);
			if (PauseMenu.isPauseMenu)
			{
				PauseMenu.StopScene();
			}
			else
			{
				PauseMenu.StartScene();
			}
		}
	}
}
