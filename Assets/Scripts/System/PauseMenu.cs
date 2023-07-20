// PauseMenu
using System.Threading.Tasks;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	private static GameObject pauseMenu;

	public static PauseMenu pauseMenuScript { get; private set; }

	public static bool isPauseMenu { get; private set; }

	private void Awake()
	{
		pauseMenuScript = this;
		pauseMenu = transform.GetChild(0).gameObject;
		isPauseMenu = true;
	}

	private void Start()
	{
		pauseMenu.SetActive(false);
	}

	public void StartScene()
	{
		Time.timeScale = 1f;
		pauseMenu.SetActive(false);
		isPauseMenu = true;
	}

	public void StopScene()
	{
		Time.timeScale = 0f;
		pauseMenu.SetActive(true);
		isPauseMenu = false;
	}

	public void ReturnToMenu()
	{
        GameSettings.PlaySound(SceneLoader.buttonPressedClip);
        Task.Delay(100);
        MazeSaver.SaveMaze();
        StartCoroutine(WebConnection.UpdateMazeData(MazeSaver.mazePositions));
        SceneLoader.LoadScene("MainMenu");
		PlayerPrefs.SetFloat("MenuSongStart", 0f);
	}
}
