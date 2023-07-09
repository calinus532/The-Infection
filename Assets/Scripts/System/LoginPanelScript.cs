// LoginPanelScript
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LoginPanelScript : MonoBehaviour
{
	private InputField inputField;

	private Text error;

	private string nickName;

	[SerializeField]
	private string[] errors;

	private void Awake()
	{
		if (PlayerPrefs.GetString("NickName") != "")
		{
			SceneManager.LoadScene("MainMenu");
		}
		inputField = GetComponent<InputField>();
		error = GameObject.Find("ErrorMessage").GetComponent<Text>();
	}

	private void Start()
	{
		StartCoroutine(WebConnection.GetUsernameData(WebConnection.GetUsernameURL));
	}

	private void Update()
	{
		if (isHavingText())
		{
			nickName = inputField.text;
		}
	}

	private bool isHavingText()
	{
		if (inputField.text == "")
		{
			error.GetComponent<Text>().text = errors[0];
			return false;
		}
		if (error.text == errors[0])
		{
			error.text = " ";
		}
		return true;
	}

	public async void PressContinue()
	{
        GameSettings.PlaySound(SceneLoader.buttonPressedClip);
        await Task.Delay(100);
        StartCoroutine(CheckName());
	}

	private IEnumerator CheckName()
	{
		StartCoroutine(WebConnection.GetUsernameData(WebConnection.GetUsernameURL));
		yield return StartCoroutine(WebConnection.GetUsernameData(WebConnection.GetUsernameURL));
		if (!WebConnection.NoInternet)
		{
            if (isNewName())
			{
				PlayerPrefs.SetString("NickName", nickName);
				StartCoroutine(WebConnection.SetData(nickName, 0, 0f));
				SceneManager.LoadScene("MainMenu");
			}
		}
		else error.text = errors[2];
	}

	private bool isNewName()
	{
		if (isHavingText())
		{
			string[] nickNames = WebConnection.userNames;
			foreach (string text in nickNames)
			{
				if (text != null)
				{
					if (text.Equals(nickName, StringComparison.OrdinalIgnoreCase))
					{
						error.text = errors[1];
						return false;
					}
				}
			}
			if (error.text == errors[1])
			{
				error.text = " ";
			}
			return true;
		}
		return false;
	}
}
