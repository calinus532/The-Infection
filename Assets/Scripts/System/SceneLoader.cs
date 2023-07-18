// SceneLoader
using System;
using System.Globalization;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

	public static SceneLoader sceneLoaderScript { get; private set; }

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
		sceneLoaderScript = this;
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

	private void Start()
	{
		if (GamePropieties.health == 0)
		{
			GamePropieties.SetHealth(5);
		}
		GameSettings.SetAudioSources();
		StartCoroutine(GameSettings.CheckNewGame());
	}

	public async void ContinueGame()
	{
		difficulty = WebConnection.difficulty;
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		PlayerPrefs.SetInt("SongStart", 0);
		ReloadScene();
	}

	public async void Tutorial(bool isTutorial)
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		if (isTutorial)
		{
			menu.SetActive(value: false);
			menuBg.SetActive(false);
			tutorial.SetActive(value: true);
			tutorialBg.SetActive(true);
		}
		else
		{
			menu.SetActive(value: true);
			menuBg.SetActive(true);
			tutorial.SetActive(value: false);
			tutorialBg.SetActive(false);
		}
	}

	public void ReloadScene()
	{
		GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
		Timer.ResetTime();
		SceneManager.LoadScene("Maze");
	}

	public async void NewGame(string dif)
	{
        StartCoroutine(WebConnection.GetScoreData());
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		Initialization(dif);
        StartCoroutine(WebConnection.UpdateDificulty());
		GamePropieties.SetNewGame(true);
		LevelSetup.newGame = true;
        ReloadScene();
    }

	private void Initialization(string dif)
	{
        difficulty = dif;
        int level = GamePropieties.highestLevel;
        float time = GamePropieties.lowestTime;
        if (WebConnection.difficulty != dif)
        {
            string[] array = WebConnection.currentScore.Split("|");
            string[] array2 = array[0].Split(",");
            string[] array3 = array[1].Split(",");
            switch (WebConnection.difficulty)
            {
                case "Easy":
                    WebConnection.SetCurrentScore(level + "," + array2[1] + "," + array2[2] + "|" + time + "," + array3[1] + "," + array3[2]);
                    break;
                case "Normal":
                    WebConnection.SetCurrentScore(array2[0] + "," + level + "," + array2[2] + "|" + array3[0] + "," + time + "," + array3[2]);
                    break;
                case "Hard":
                    WebConnection.SetCurrentScore(array2[0] + "," + array2[1] + "," + level + "|" + array3[0] + "," + array3[1] + "," + time);
                    break;
            }
            switch (difficulty)
            {
                case "Easy":
                    level = int.Parse(array2[0], CultureInfo.InvariantCulture.NumberFormat);
                    time = float.Parse(array3[0], CultureInfo.InvariantCulture.NumberFormat);
                    break;
                case "Normal":
                    level = int.Parse(array2[1], CultureInfo.InvariantCulture.NumberFormat); ;
                    time = float.Parse(array3[1], CultureInfo.InvariantCulture.NumberFormat);
                    break;
                case "Hard":
                    level = int.Parse(array2[2], CultureInfo.InvariantCulture.NumberFormat); ;
                    time = float.Parse(array3[2], CultureInfo.InvariantCulture.NumberFormat);
                    break;
            }
            StartCoroutine(WebConnection.UpdateScoreData(level, time));
        }
        GamePropieties.SetLevel(1);
        GamePropieties.SetWin(false);
        GamePropieties.SetLose(false);
        GamePropieties.SetHighestLevel(level);
        GamePropieties.SetLowestTime(time);
		GamePropieties.SetCoins(0);
		GamePropieties.SetHealth(5);
		GamePropieties.SetSpeed(0);
		GamePropieties.SetLightIntensity(0);
		GamePropieties.SetLights(0);
		GamePropieties.SetCollected(false);
    }

	public async void RestartGame()
	{
		LevelSetup.newGame = true;
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		PlayerPrefs.SetFloat("SongStart", 0f);
		GamePropieties.SetLevel(1);
        GamePropieties.SetLose(false);
		GamePropieties.SetDisplayTime(0);
		Timer.ResetTime();
		GamePropieties.SetCollected(false);
		ReloadScene();
	}

	public async void LoadScene(string sceneName)
	{
        GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		SceneManager.LoadScene(sceneName);
	}

	public async void CloseApp()
	{
		GameSettings.PlaySound(buttonPressedClip);
		GameObject.Find("GameProps").GetComponent<GamePropieties>().SaveGameSettings();
		await Task.Delay(200);
		Application.Quit();
	}

	public async void ResumeGame()
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		PauseMenu.pauseMenuScript.StartScene();
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
				PauseMenu.pauseMenuScript.StartScene();
			
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
				PauseMenu.pauseMenuScript.StopScene();
			}
			else
			{
				PauseMenu.pauseMenuScript.StartScene();
			}
		}
	}

	public async void ActivateDeactivateMenu()
	{
		if (SceneManager.GetActiveScene().name == "Maze")
		{
			if (!Finish.panels[0].activeSelf && !Finish.panels[1].activeSelf && !GamePropieties.newGame)
			{
				GameSettings.PlaySound(buttonPressedClip);
				await Task.Delay(100);
				if (PauseMenu.isPauseMenu)
				{
					PauseMenu.pauseMenuScript.StopScene();
				}
				else
				{
					PauseMenu.pauseMenuScript.StartScene();
				}
			}
		}
		else
		{
			GameSettings.PlaySound(buttonPressedClip);
			await Task.Delay(100);
			if (PauseMenu.isPauseMenu)
			{
				PauseMenu.pauseMenuScript.StopScene();
			}
			else
			{
				PauseMenu.pauseMenuScript.StartScene();
			}
		}
	}
}
