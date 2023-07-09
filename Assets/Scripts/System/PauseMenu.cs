// PauseMenu
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	private static GameObject pauseMenu;

	public static PauseMenu pauseMenuScript { get; private set; }

	public static bool isPauseMenu { get; private set; }

	private void Awake()
	{
		pauseMenuScript = this;
		pauseMenu = base.transform.GetChild(0).gameObject;
		isPauseMenu = true;
	}

	private void Start()
	{
		pauseMenu.SetActive(value: false);
	}

	public void StartScene()
	{
		Time.timeScale = 1f;
		pauseMenu.SetActive(value: false);
		isPauseMenu = true;
	}

	public void StopScene()
	{
		Time.timeScale = 0f;
		pauseMenu.SetActive(value: true);
		isPauseMenu = false;
	}

	public void ReturnToMenu()
	{
		GameObject.Find("SceneChanger").GetComponent<SceneLoader>().LoadScene("MainMenu");
		PlayerPrefs.SetFloat("MenuSongStart", 0f);
	}
}
