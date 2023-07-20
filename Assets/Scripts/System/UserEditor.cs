using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GetUserProps : WebConnection
{
    protected private List<string> userPropieties = new List<string>();

    protected private IEnumerator UserPropsSetup()
    {
        StartCoroutine(GetUserData(GamePropieties.userName));
        yield return StartCoroutine(GetUserData(GamePropieties.userName));
        userPropieties = userProps;
    }
}

public class UserEditor : GetUserProps
{
    static public GameObject panel { private set; get; }

    private InputField[] textBoxes;

    private Dropdown dropdown;

    private Text[] Warnings = new Text[2];

    private GameObject[] buttons = new GameObject[2];

    [SerializeField]
    private string[] errorsName;

    [SerializeField]
    private string errorPassword;

    private List<string> updatedUserPropieties;

    private void Awake()
    {
        textBoxes = GameObject.Find("TextBoxes").GetComponentsInChildren<InputField>();
        dropdown = GameObject.Find("CountriesDD").GetComponent<Dropdown>();
        Warnings[0] = GameObject.Find("NameError").GetComponent<Text>();
        Warnings[1] = GameObject.Find("PasswordError").GetComponent<Text>();
        if (GameObject.Find("CountriesDD"))
            GameObject.Find("CountryManager").GetComponent<CountrySelection>().SetupCountry();
        buttons[0] = GameObject.Find("EditUser");
        buttons[1] = GameObject.Find("SaveUser");
        buttons[1].SetActive(false);
        panel = transform.GetChild(0).gameObject;
        panel.SetActive(false);
    }

    public void Preview() 
    {
        buttons[0].SetActive(true);
        buttons[1].SetActive(false);
        StartCoroutine(SetText(false));
    }

    public void Edit()
    {
        buttons[0].SetActive(false);
        buttons[1].SetActive(true);
        StartCoroutine(SetText(true));
    }

    public void Save() => StartCoroutine(SaveText());

    public void Delete() => StartCoroutine(IDelete());

    private IEnumerator IDelete()
    {
        StartCoroutine(DeleteUser());
        yield return StartCoroutine(DeleteUser());
        SceneLoader.sceneLoaderScript.LoadScene("LoginMenu");
    }

    private void Update()
    {
        if (textBoxes[0].interactable)
        {
            isHavingText(0, errorsName[0]);
            isHavingText(1, errorPassword);
        }

        /*if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(DeleteUser());
            SceneManager.LoadScene("LoginMenu");
        }*/
    }

    private IEnumerator SaveText()
    {
        StartCoroutine(GetUsernameData());
        yield return StartCoroutine(GetUsernameData());
        updatedUserPropieties = new List<string>
        {
            textBoxes[0].text.Trim(),
            textBoxes[1].text,
            dropdown.value.ToString()
        };
        if (!NoInternet)
        {
            if (isNewName() && isHavingText(1, errorPassword))
            {
                StartCoroutine(UpdateUserData(updatedUserPropieties, GamePropieties.userName));
                yield return StartCoroutine(UpdateUserData(updatedUserPropieties, GamePropieties.userName));
                if (GameObject.Find("GameProps") != null)
                {
                    StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
                    yield return StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
                }
                buttons[0].SetActive(true);
                buttons[1].SetActive(false);
                StartCoroutine(SetText(false));
            }
        }
        else Warnings[0].text = errorsName[2];
    }

    private bool isHavingText(int idx, string error)
    {
        if (textBoxes[idx].text == "")
        {
            Warnings[idx].text = error;
            return false;
        }
        if (Warnings[idx].text == error)
            Warnings[idx].text = " ";
        return true;
    }

    private bool isNewName()
    {
        if (isHavingText(0, errorsName[0]))
        {
            string[] nickNames = userNames;
            foreach (string text in nickNames)
            {
                if (textBoxes[0].text.Equals(GamePropieties.userName, StringComparison.OrdinalIgnoreCase)) break;
                if (text != null)
                {
                    if (text.Equals(textBoxes[0].text, StringComparison.OrdinalIgnoreCase))
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

    private IEnumerator SetText(bool edit)
    {
        StartCoroutine(UserPropsSetup());
        yield return StartCoroutine(UserPropsSetup());
        textBoxes[0].text = userPropieties[0];
        if (edit) textBoxes[1].inputType = InputField.InputType.Standard;
        else textBoxes[1].inputType = InputField.InputType.Password;
        textBoxes[1].text = userPropieties[1];
        textBoxes[1].ForceLabelUpdate();
        dropdown.value = int.Parse(userPropieties[2]);

        foreach (InputField i in textBoxes) i.interactable = edit;
        dropdown.interactable = edit;
        dropdown.transform.GetChild(1).gameObject.SetActive(edit);

        if (!panel.activeSelf) panel.SetActive(true);
    }

    public void LogOut()
    {
        PlayerPrefs.SetString("NickName", "");
        SceneLoader.sceneLoaderScript.LoadScene("LoginMenu");
    }
}
