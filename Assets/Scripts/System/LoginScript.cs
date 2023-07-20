using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : WebConnection
{
    private InputField[] inputFields;

    private Text[] Warning = new Text[2];

    private string password;

    [SerializeField]
    private string[] errorsPassword;

    private void Awake()
    {   
        inputFields = GetComponentsInChildren<InputField>();
        Warning[0] = GameObject.Find("NameError").GetComponent<Text>();
        Warning[1] = GameObject.Find("PasswordError").GetComponent<Text>();
    }

    public void LoginButton()
    {
        StartCoroutine(CheckLogin());
    }

    private IEnumerator CheckLogin()
    {
        StartCoroutine(GetPassword(inputFields[0].text));
        yield return StartCoroutine(GetPassword(inputFields[0].text));
        password = userPassword;
        if (password != "")
        {
            if (inputFields[1].text.Trim().Equals(password))
            {
                PlayerPrefs.SetString("NickName", inputFields[0].text.Trim());
                currentNickName = inputFields[0].text.Trim();
                StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
                yield return StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                Warning[1].text = errorsPassword[0];
                Warning[0].text = "";
            }
        }
        else
        {
            Warning[0].text = errorsPassword[1];
            Warning[1].text = "";
        }
    }
}
