// WebConnection
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class WebConnection : MonoBehaviour
{
	[SerializeField]
	protected private string[] urls;

    static private List<string> connections = new List<string>();

    public static string[] userNames { get; private set; }

    public static string[] nickNames { get; private set; }

    static protected private string currentNickName = "";

	public static string[] highestLevels { get; private set; }

	public static string[] lowestTimes { get; private set; }
	public static string[] countriesIdxes { private set; get; }
    static public bool NoInternet { private set; get; }
	static public string difficulty { private set; get; }
    
    static private string currentLevel;
    
    static private string currentTime;
    static public string currentScore { private set; get; }

    private void Awake()
	{
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        nickNames = new string[1];
        if (connections.Count == 0)
        {
            connections = new List<string>();
            foreach (string s in urls) connections.Add(s);
        }
        if (PlayerPrefs.GetString("NickName") != "")
        {
            currentNickName = PlayerPrefs.GetString("NickName");
            if (GameObject.Find("GameProps") != null)
            {
                StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
                yield return StartCoroutine(GameObject.Find("GameProps").GetComponent<GamePropieties>().GetValues());
            }
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                StartCoroutine(GetMazeData());
                yield return StartCoroutine(GetMazeData());
            }
            if (SceneManager.GetActiveScene().name == "LoginMenu")
            {
                SceneManager.LoadScene("MainMenu");
                yield break;
            }
        }
    }

	public static IEnumerator GetScoreboardData(string lDifficulty, string type)
	{
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("getDifficulty", lDifficulty);
        wWWForm.AddField("getMazeType", type);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[0], wWWForm);
		yield return webRequest.SendWebRequest();
		string[] array = webRequest.downloadHandler.text.Trim().Split('/');
		nickNames = new string[10];
		highestLevels = new string[10];
		lowestTimes = new string[10];
        countriesIdxes = new string[10];
        for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "")
			{
				string[] array2 = array[i].Split(',');
				nickNames[i] = array2[0];
				highestLevels[i] = array2[1];
				lowestTimes[i] = array2[2];
				countriesIdxes[i] = array2[3];
            }
		}
	}

    public static IEnumerator GetUsernameData()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(connections[1]);
        yield return webRequest.SendWebRequest();
        string[] array = webRequest.downloadHandler.text.Trim().Split('/');
        userNames = new string[array.Length - 1];
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != "")
            {
                userNames[i] = array[i];
            }
        }
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                NoInternet = true;
                break;
            case UnityWebRequest.Result.Success:
                NoInternet = false;
                break;
        }
    }

    static protected private IEnumerator SetData(string name, string password)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("addNickName", name);
		wWWForm.AddField("addLevel", 0);
		wWWForm.AddField("addTime", 0);
        wWWForm.AddField("addPassword", password);
        wWWForm.AddField("addCountry", GamePropieties.country);
        using UnityWebRequest www = UnityWebRequest.Post(connections[2], wWWForm);
		yield return www.SendWebRequest();
	}

	public static IEnumerator UpdateData(int level, float time)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("editLevel", level);
		wWWForm.AddField("editTime", time.ToString());
		wWWForm.AddField("currentNickName", GamePropieties.userName);
        wWWForm.AddField("editCountry", GamePropieties.country);
        using UnityWebRequest www = UnityWebRequest.Post(connections[3], wWWForm);
		yield return www.SendWebRequest();
	}
	static public IEnumerator GetDifficultyData()
	{
        using UnityWebRequest webRequest = UnityWebRequest.Get(connections[4]);
        yield return webRequest.SendWebRequest();
        string[] difficulties = webRequest.downloadHandler.text.Trim().Split('/');
        foreach(string s in difficulties)
        {
            if (s == "") break;
            string[] strings = s.Split(',');
            if (strings[0] == GamePropieties.userName)
            {
                difficulty = strings[1];
            }
        }
    }
	public static IEnumerator UpdateDificulty()
	{
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("editDifficulty", SceneLoader.difficulty);
        wWWForm.AddField("currentNickName", GamePropieties.userName);
        using UnityWebRequest www = UnityWebRequest.Post(connections[5], wWWForm);
        yield return www.SendWebRequest();
    }
    static public IEnumerator GetScoreData()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentNickName", GamePropieties.userName);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[6], wWWForm);
        yield return webRequest.SendWebRequest();
        string[] array = webRequest.downloadHandler.text.Trim().Split('/');
        currentLevel = array[0];
        currentTime = array[1];
        currentScore = array[2];
    }
    static public IEnumerator UpdateScoreData(int level, float time)
    {
        WWWForm wWWForm = new WWWForm();
        UpdateScore();
        wWWForm.AddField("editLevel", level);
        wWWForm.AddField("editTime", time.ToString());
        wWWForm.AddField("editScore", currentScore);
        wWWForm.AddField("currentNickName", GamePropieties.userName);
        using UnityWebRequest www = UnityWebRequest.Post(connections[7], wWWForm);
        yield return www.SendWebRequest();
    }
    static private void UpdateScore()
    {
        string[] array = currentScore.Split("|");
        string[] array2 = array[0].Split(",");
        string[] array3 = array[1].Split(",");
        switch (difficulty)
        {
            case "Easy":
                array2[0] = currentLevel;
                array3[0] = currentTime;
                break;
            case "Normal":
                array2[1] = currentLevel;
                array3[1] = currentTime;
                break;
            case "Hard":
                array2[2] = currentLevel;
                array3[2] = currentTime;
                break;
        }
        SetCurrentScore(array2[0] + "," + array2[1] + "," + array2[2] + "|" + array3[0] + "," + array3[1] + "," + array3[2]);
    }

    static protected private IEnumerator DeleteUser()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentNickName", GamePropieties.userName);
        PlayerPrefs.SetString("NickName", "");
        using UnityWebRequest www = UnityWebRequest.Post(connections[8], wWWForm);
        yield return www.SendWebRequest();
    }
    static public void SetCurrentScore(string score) => currentScore = score;

    static public IEnumerator UpdateMazeData(List<string> list)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("editWalls", list[0]);
        wWWForm.AddField("editCorectPath", list[1]);
        wWWForm.AddField("editCorner", list[2]);
        wWWForm.AddField("editFinish", list[3]);
        wWWForm.AddField("editPlayer", list[4]);
        wWWForm.AddField("editLight", list[5]);
        wWWForm.AddField("editViruses", list[6]);
        wWWForm.AddField("editMasks", list[7]);
        wWWForm.AddField("editType", list[8]);
        wWWForm.AddField("currentUser", GamePropieties.userName);
        using UnityWebRequest www = UnityWebRequest.Post(connections[9], wWWForm);
        yield return www.SendWebRequest();
    }

    static public List<string> mazePositions {private set; get;}

    static public IEnumerator GetMazeData()
    {
        mazePositions = new List<string>();
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentUser", GamePropieties.userName);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[10], wWWForm);
        yield return webRequest.SendWebRequest();
        string[] positions = webRequest.downloadHandler.text.Trim().Split('|');
        foreach (string s in positions) mazePositions.Add(s);
        yield break;
    }

    static protected internal string userPassword = "";

    static protected private IEnumerator GetPassword(string user)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentUser", user);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[11], wWWForm);
        yield return webRequest.SendWebRequest();
        userPassword = webRequest.downloadHandler.text.Trim();
    }

    static protected internal List<string> userProps;

    static protected private IEnumerator GetUserData(string name)
    {
        userProps = new List<string>();
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentUser", name);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[12], wWWForm);
        yield return webRequest.SendWebRequest();
        string[] props = webRequest.downloadHandler.text.Trim().Split('|');
        foreach (string s in props) userProps.Add(s);
        yield break;
    }

    static protected private IEnumerator UpdateUserData(List<string> list, string name)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentUser", name);
        wWWForm.AddField("editName", list[0]);
        PlayerPrefs.SetString("NickName", list[0]);
        currentNickName = list[0];
        wWWForm.AddField("editPassword", list[1]);
        wWWForm.AddField("editCountry", int.Parse(list[2]));
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[13], wWWForm);
        yield return webRequest.SendWebRequest();
    }

    static protected internal List<string> gameProps = new List<string>();

    static protected private IEnumerator GetGamePropsData()
    {
        if (PlayerPrefs.GetString("NickName") == "" || connections.Count == 0) yield break;
        if (currentNickName == "") yield break;
        gameProps = new List<string>();
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentUser", currentNickName);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[14], wWWForm);
        yield return webRequest.SendWebRequest();
        string[] props = webRequest.downloadHandler.text.Trim().Split('|');
        foreach (string s in props) gameProps.Add(s);
        yield break;
    }

    static protected private IEnumerator UpdateGamePropsData(List<string> list, string name)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentUser", name);
        wWWForm.AddField("editCountry", int.Parse(list[0]));
        wWWForm.AddField("editLevelProps", list[1]);
        wWWForm.AddField("editShopProps", list[2]);
        wWWForm.AddField("editPlayerProps", list[3]);
        using UnityWebRequest webRequest = UnityWebRequest.Post(connections[15], wWWForm);
        yield return webRequest.SendWebRequest();
    }
}
