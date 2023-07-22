// PauseMenu
using System.Threading.Tasks;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static GameObject pauseMenu;

	public static bool isPauseMenu;

	private void Awake()
	{
		pauseMenu = transform.GetChild(0).gameObject;
		isPauseMenu = true;
	}

	private void Start()
	{
		pauseMenu.SetActive(false);
	}

	static public void StartScene()
	{
		Time.timeScale = 1f;
        isPauseMenu = true;
        pauseMenu.SetActive(false);
	}

	static public void StopScene()
	{
		Time.timeScale = 0f;
        isPauseMenu = false;
        pauseMenu.SetActive(true);
	}

	public void ReturnToMenu()
	{
        isPauseMenu = true;
        StartCoroutine(MazeSaver.SaveMaze());
        StartCoroutine(WebConnection.UpdateMazeData(MazeSaver.mazePositions));
        PlayerPrefs.SetFloat("MenuSongStart", 0f);
        GameSettings.PlaySound(SceneLoader.buttonPressedClip);
        Task.Delay(300);
        SceneLoader.LoadScene("MainMenu");
	}
}
