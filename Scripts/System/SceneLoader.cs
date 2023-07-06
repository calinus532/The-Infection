// SceneLoader
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	private AsyncOperation scene;

	private string mazeType;

	[SerializeField]
	private MazeSaver mazeSaver;

	[SerializeField]
	private AudioClip buttonPressed;

	private GameObject menu, menuBg;

	private GameObject tutorial, tutorialBg, dificultyPanel;

	private static bool firstTime = true;

	public static SceneLoader sceneLoaderScript { get; private set; }

	public static AudioClip buttonPressedClip { get; private set; }

	public static float clipLength { get; private set; }
	static public string difficulty { private set; get; }
	static public bool darkness { private set; get; }

	private void Awake()
	{
		if (firstTime)
		{
			PlayerPrefs.SetFloat("MenuSongStart", 0f);
			PlayerPrefs.SetFloat("MazeSongStart", 0f);
			firstTime = false;
		}
		sceneLoaderScript = this;
		mazeType = "Maze";
		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			menu = GameObject.Find("MainMenu");
			menuBg = GameObject.Find("MenuBackground");
            tutorial = GameObject.Find("TutorialMenu");
            tutorialBg = GameObject.Find("TutorialBackground");
            tutorial.SetActive(false);
			tutorialBg.SetActive(false);
			dificultyPanel = GameObject.Find("DifficultyPanel");
			dificultyPanel.SetActive(false);
		}
		clipLength = buttonPressed.length;
		buttonPressedClip = buttonPressed;
		StartCoroutine(WebConnection.GetDifficultyData());
	}

	private void Start()
	{
		if (PlayerPrefs.GetInt("Health", 5) == 0)
		{
			PlayerPrefs.SetInt("Health", 5);
		}
		scene = SceneManager.LoadSceneAsync(mazeType);
		scene.allowSceneActivation = false;
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
		scene.allowSceneActivation = true;
	}

	public async void NewGame(string dif)
	{
        difficulty = dif;
        StartCoroutine(WebConnection.GetScoreData());
        GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		int @int = PlayerPrefs.GetInt("MusicVolume", 50);
		int int2 = PlayerPrefs.GetInt("EffectsVolume", 50);
		int int3 = PlayerPrefs.GetInt("Fullscreen", 1);
		string @string = PlayerPrefs.GetString("NickName");
		int level = PlayerPrefs.GetInt("HighestLevel");
        float time = PlayerPrefs.GetFloat("LowestTime");
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
        int int5 = PlayerPrefs.GetInt("Darkness");
		int countryID = PlayerPrefs.GetInt("Country");
        PlayerPrefs.DeleteAll();
		PlayerPrefs.SetInt("MusicVolume", @int);
		PlayerPrefs.SetInt("EffectsVolume", int2);
		PlayerPrefs.SetInt("Fullscreen", int3);
		PlayerPrefs.SetString("NickName", @string);
        StartCoroutine(WebConnection.UpdateDificulty());
        PlayerPrefs.SetInt("HighestLevel", level);
		PlayerPrefs.SetFloat("LowestTime", time);
        PlayerPrefs.SetInt("Darkness", int5);
        PlayerPrefs.SetInt("Country", countryID);
        PlayerPrefs.SetInt("NewGame", 1);
        ReloadScene();
    }

	public async void RestartGame()
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		PlayerPrefs.SetFloat("SongStart", 0f);
		SceneManager.LoadScene("Maze");
		PlayerPrefs.SetInt("Level", 1);
		PlayerPrefs.SetInt("Lose", 0);
		PlayerPrefs.SetFloat("Time", 0f);
		PlayerPrefs.SetInt("Collected", 0);
		ReloadScene();
	}

	public async void LoadScene(string sceneName)
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
		SceneManager.LoadScene(sceneName);
	}

	public async void CloseApp()
	{
		GameSettings.PlaySound(buttonPressedClip);
		await Task.Delay(100);
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
				if (PlayerPrefs.GetInt("NewGame", 0) == 0)
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

		if (Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			PlayerPrefs.DeleteAll();
		}
		if (Input.GetKeyDown(KeyCode.F11))
		{
			GameSettings.stateChanged = false;
			GameSettings.fullscreenTg.isOn = !GameSettings.fullscreenTg.isOn;
		}
		GameSettings.gameSettingsScript.Fullscreen();

		if (GameSettings.hardmodeTg.isOn)
		{
			PlayerPrefs.SetInt("Darkness", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Darkness", 0);
		}
        if (PlayerPrefs.GetInt("Darkness") == 0) darkness = false;
        else darkness = true;

		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			if (tutorial.activeSelf || dificultyPanel.activeSelf)
				PauseMenu.pauseMenuScript.StartScene();
			
			if (dificultyPanel.activeSelf)
				if (Input.GetKeyDown(KeyCode.Escape)) 
					dificultyPanel.SetActive(false);
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
			if (!Finish.panels[0].activeSelf && !Finish.panels[1].activeSelf && PlayerPrefs.GetInt("NewGame", 0) == 0)
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
