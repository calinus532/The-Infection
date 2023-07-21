using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class RegisterScript : WebConnection
{
	private InputField[] inputFields;

    private Text[] Warnings = new Text[2];

	private string nickName;

	[SerializeField]
	private string[] errorsName;

    [SerializeField]
    private string errorPassword;

    private void Awake()
	{
		inputFields = GetComponentsInChildren<InputField>();
		Warnings[0] = GameObject.Find("NameErrorMessage").GetComponent<Text>();
		Warnings[1] = GameObject.Find("PasswordErrorMessage").GetComponent<Text>();
		if (GameObject.Find("CountriesDD"))
			GameObject.Find("CountryManager").GetComponent<CountrySelection>().SetupCountry();
		GameObject.Find("CanvasRegister").SetActive(false);
    }

	private void Start()
	{
		StartCoroutine(GetUsernameData());
	}

	private void Update()
	{
		Name();
		isHavingText(1, errorPassword);
    }

	private void Name()
	{
		if (isHavingText(0, errorsName[0]))
            nickName = inputFields[0].text;
    }

    private bool isHavingText(int idx, string error)
	{
		if (inputFields[idx].text == "")
		{
			Warnings[idx].text = error;
			return false;
		}
		if (Warnings[idx].text == error)
		{
            Warnings[idx].text = " ";
		}
		return true;
	}

	public async void PressRegister()
	{
        GameSettings.PlaySound(SceneLoader.buttonPressedClip);
        await Task.Delay(100);
        StartCoroutine(CheckName());
	}

	private IEnumerator CheckName()
	{
		StartCoroutine(GetUsernameData());
		yield return StartCoroutine(GetUsernameData());
		if (!NoInternet)
		{
            if (isNewName() && isHavingText(1, errorPassword))
			{
				PlayerPrefs.SetString("NickName", nickName.Trim());
				currentNickName = nickName.Trim();
				StartCoroutine(SetData(nickName, inputFields[1].text.Trim()));
				yield return StartCoroutine(SetData(nickName, inputFields[1].text.Trim()));
                StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
				yield return StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
                SceneManager.LoadScene("MainMenu");
            }
		}
		else Warnings[0].text = errorsName[2];
	}

	private bool isNewName()
	{
		if (isHavingText(0, errorsName[0]))
		{
			string[] nickNames = userNames;
			foreach (string text in nickNames)
			{
				if (text != null)
				{
					if (text.Equals(nickName, StringComparison.OrdinalIgnoreCase))
					{
                        Warnings[0].text = errorsName[1];
						return false;
					}
				}
			}
			if (Warnings[0].text == errorsName[1])
			{
                Warnings[0].text = " ";
			}
			return true;
		}
		return false;
	}
}
