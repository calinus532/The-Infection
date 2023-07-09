// GameSettings
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
	public static Toggle fullscreenTg;

	public static Toggle hardmodeTg;

	private Slider[] slider = new Slider[2];

	private GameObject menuButton;

	private GameObject resumeButton;

	[SerializeField]
	private int[] _screenWidths;

	[SerializeField]
	private int[] _screenHeights;

	[SerializeField]
	private AudioClip mainMenuSong;

	[SerializeField]
	private AudioClip pauseMenuSong;

	[SerializeField]
	private AudioClip[] mazeSongs;

	[SerializeField]
	private AudioClip newGamePressed;

	private AudioClip lastSongPlayed;

	private float lastSongTime;

	private static AudioClip[] songs = new AudioClip[4];

	private static AudioClip newGameSound;

	public static bool stateChanged;

	private static float lowerPercentage;
	public static GameSettings gameSettingsScript { get; private set; }

	public static int _currentScreenWidth { get; private set; }

	public static int _currentScreenHeight { get; private set; }

	public static int _minimScreenWidth { get; private set; }

	public static int _minimScreenHeight { get; private set; }

	public static AudioSource[] audioSource { get; private set; }

	private void Awake()
	{
		gameSettingsScript = this;
		audioSource = new AudioSource[1];
		lowerPercentage = 95f;
		fullscreenTg = GetComponentsInChildren<Toggle>()[0];
		hardmodeTg = GetComponentsInChildren<Toggle>()[1];
		if (Application.isMobilePlatform) fullscreenTg.gameObject.SetActive(false);
		if (PlayerPrefs.GetInt("Fullscreen", 1) == 1)
		{
			fullscreenTg.isOn = true;
		}
		else
		{
			fullscreenTg.isOn = false;
		}
		if (PlayerPrefs.GetInt("Darkness") == 1)
		{
			hardmodeTg.isOn = true;
		}
		else
		{
			hardmodeTg.isOn = false;
		}
		slider[0] = GetComponentsInChildren<Slider>()[0];
		slider[1] = GetComponentsInChildren<Slider>()[1];
		slider[0].value = (float)PlayerPrefs.GetInt("MusicVolume", 50) * slider[0].maxValue / 100f;
		slider[1].value = (float)PlayerPrefs.GetInt("EffectsVolume", 50) * slider[1].maxValue / 100f;
		menuButton = base.transform.GetChild(0).GetChild(3).gameObject;
		resumeButton = base.transform.GetChild(0).GetChild(2).gameObject;
		songs[0] = mainMenuSong;
		songs[1] = pauseMenuSong;
		for (int i = 0; i < mazeSongs.Length; i++)
		{
			songs[i + 2] = mazeSongs[i];
		}
		newGameSound = newGamePressed;
		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			menuButton.SetActive(value: false);
			resumeButton.transform.position -= new Vector3(0f, 50f, 0f);
		}
		if (!Application.isMobilePlatform)
		{
            _currentScreenWidth = Screen.mainWindowDisplayInfo.width;
            _currentScreenHeight = Screen.mainWindowDisplayInfo.height;
            SetResolutions();
            if (PlayerPrefs.GetInt("Fullscreen", 1) == 1)
			{
				Screen.SetResolution(_currentScreenWidth, _currentScreenHeight, fullscreen: true);
			}
			else
			{
				Screen.SetResolution(_minimScreenWidth, _minimScreenHeight, fullscreen: false);
			}
		}
	}

	private static float musicLowerPercentage()
	{
		return 200f - lowerPercentage + 5f;
	}

	private static float sfxLowerPercentage()
	{
		return 200f - lowerPercentage;
	}

	public void changeState()
	{
		stateChanged = false;
	}

	public static void SetAudioSources()
	{
		audioSource = new AudioSource[2];
		audioSource[0] = GameObject.Find("AudioManager").GetComponentsInChildren<AudioSource>()[0];
		float num = PlayerPrefs.GetInt("MusicVolume", 50);
		audioSource[0].volume = num / musicLowerPercentage();
		audioSource[1] = GameObject.Find("AudioManager").GetComponentsInChildren<AudioSource>()[1];
		audioSource[1].volume = (float)PlayerPrefs.GetInt("EffectsVolume", 50) / sfxLowerPercentage();
	}

	private void SetResolutions()
	{
		if (Mathf.Round((float)_currentScreenWidth / (float)_currentScreenHeight * Mathf.Pow(10f, 2f)) != Mathf.Round(1.77777779f * Mathf.Pow(10f, 2f)))
		{
			for (int num = _screenWidths.Length - 1; num >= 0; num--)
			{
				if (_currentScreenWidth >= _screenWidths[num])
				{
					_currentScreenWidth = _screenWidths[num];
					_currentScreenHeight = _screenHeights[num];
					if (num > 0)
					{
						_minimScreenWidth = _screenWidths[num - 1];
						_minimScreenHeight = _screenHeights[num - 1];
					}
					else
					{
						_minimScreenWidth = _screenWidths[num] / 2;
						_minimScreenHeight = _screenHeights[num] / 2;
					}
					num = -1;
				}
			}
			return;
		}
		for (int num2 = _screenWidths.Length - 1; num2 >= 0; num2--)
		{
			if (_currentScreenWidth >= _screenWidths[num2])
			{
				if (num2 > 0)
				{
					_minimScreenWidth = _screenWidths[num2 - 1];
					_minimScreenHeight = _screenHeights[num2 - 1];
				}
				else
				{
					_minimScreenWidth = _screenWidths[num2] / 2;
					_minimScreenHeight = _screenHeights[num2] / 2;
				}
				num2 = -1;
			}
		}
	}

	private void Update()
	{
		PlayerPrefs.SetInt("MusicVolume", (int)Mathf.Round(slider[0].value * 100f / slider[0].maxValue));
		PlayerPrefs.SetInt("EffectsVolume", (int)Mathf.Round(slider[1].value * 100f / slider[1].maxValue));
		if (audioSource != null)
		{
			if (!PauseMenu.isPauseMenu)
			{
				if (audioSource[0].clip != songs[1])
				{
					lastSongPlayed = audioSource[0].clip;
					lastSongTime = audioSource[0].time;
					audioSource[0].clip = songs[1];
					audioSource[0].time = 0;
					audioSource[0].Play();
				}
			}
			else if (audioSource[0].clip == songs[1])
			{
				audioSource[0].clip = lastSongPlayed;
				audioSource[0].time = lastSongTime;
				audioSource[0].Play();
			}
			float num = 0f;
			num = PlayerPrefs.GetInt("MusicVolume", 50);
			audioSource[0].volume = num / musicLowerPercentage();
			audioSource[1].volume = (float)PlayerPrefs.GetInt("EffectsVolume") / sfxLowerPercentage();
		}
	}

	public void Fullscreen()
	{
		if (Application.isMobilePlatform)
		{
			return;
		}
		if (fullscreenTg.isOn)
		{
			PlayerPrefs.SetInt("Fullscreen", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Fullscreen", 0);
		}
		SetResolutions();
		if (!stateChanged)
		{
			if (PlayerPrefs.GetInt("Fullscreen", 1) == 1)
			{
				Screen.SetResolution(_currentScreenWidth, _currentScreenHeight, fullscreen: true);
				stateChanged = true;
			}
			else
			{
				Screen.SetResolution(_minimScreenWidth, _minimScreenHeight, fullscreen: false);
				stateChanged = true;
			}
		}
	}

	public static void PlaySound(AudioClip clip)
	{
		if (audioSource.Length > 1)
		{
			if (audioSource[1] == null)
			{
				audioSource[1] = GameObject.Find("AudioManager").GetComponentsInChildren<AudioSource>()[1];
			}
			audioSource[1].PlayOneShot(clip);
		}
	}

	static public IEnumerator CheckNewGame()
	{
        if (SceneManager.GetActiveScene().name == "Maze" && PlayerPrefs.GetInt("NewGame") == 1)
        {
            if (audioSource[1].volume > 0f)
            {
                PlaySound(newGameSound);
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(newGameSound.length);
                Time.timeScale = 1f;
            }
            PlayerPrefs.SetInt("NewGame", 0);
        }
		PlaySongs();
    }
	static private void PlaySongs()
	{
        if (SceneManager.GetActiveScene().name != "Maze")
        {
            audioSource[0].clip = songs[0];
            audioSource[0].time = PlayerPrefs.GetFloat("MenuSongStart", 0f);
        }
        if (SceneManager.GetActiveScene().name == "Maze")
        {
            int num2 = Random.Range(2, songs.Length);
            if (num2 == PlayerPrefs.GetInt("SongIdx", 2))
            {
                audioSource[0].clip = songs[PlayerPrefs.GetInt("SongIdx", 2)];
                audioSource[0].time = PlayerPrefs.GetFloat("MazeSongStart");
            }
            else
            {
                audioSource[0].clip = songs[num2];
                PlayerPrefs.SetInt("SongIdx", num2);
            }
        }
        audioSource[0].PlayScheduled(1.0);
    }
}
