// WebConnection
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebConnection : MonoBehaviour
{
	[SerializeField]
	private string[] urls;

	public static string GetScoreboardURL { get; private set; }
    
    public static string GetUsernameURL { get; private set; }
    
    public static string InsertURL { get; private set; }
	
    public static string UpdateURL { get; private set; }
    
    public static string GetDifficultyURL { get; private set; }
    
    public static string UpdateDifficultyURL { get; private set; }
    
    public static string UpdateScoreURL { get; private set; }
    
    public static string GetScoreURL { get; private set; }

    public static string DeleteUserURL { get; private set; }

    public static string[] userNames { get; private set; }

    public static string[] nickNames { get; private set; }

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
		nickNames = new string[1];
		GetScoreboardURL = urls[0];
        GetUsernameURL = urls[1];
        InsertURL = urls[2];
		UpdateURL = urls[3];
        GetDifficultyURL = urls[4];
        UpdateDifficultyURL = urls[5];
        GetScoreURL = urls[6];
        UpdateScoreURL = urls[7];
        DeleteUserURL = urls[8];

    }

	public static IEnumerator GetScoreboardData(string URL, string lDifficulty)
	{
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("getDifficulty", lDifficulty);
        using UnityWebRequest webRequest = UnityWebRequest.Post(URL, wWWForm);
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

    public static IEnumerator GetUsernameData(string URL)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(URL);
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

    public static IEnumerator SetData(string name, int level, float time)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("addNickName", name);
		wWWForm.AddField("addLevel", level);
		wWWForm.AddField("addTime", time.ToString());
        wWWForm.AddField("addCountry", PlayerPrefs.GetInt("Country").ToString());
        using UnityWebRequest www = UnityWebRequest.Post(InsertURL, wWWForm);
		yield return www.SendWebRequest();
	}

	public static IEnumerator UpdateData(int level, float time)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("editLevel", level);
		wWWForm.AddField("editTime", time.ToString());
		wWWForm.AddField("currentNickName", PlayerPrefs.GetString("NickName"));
        wWWForm.AddField("editCountry", PlayerPrefs.GetInt("Country").ToString());
        using UnityWebRequest www = UnityWebRequest.Post(UpdateURL, wWWForm);
		yield return www.SendWebRequest();
	}
	static public IEnumerator GetDifficultyData()
	{
        using UnityWebRequest webRequest = UnityWebRequest.Get(GetDifficultyURL);
        yield return webRequest.SendWebRequest();
        string[] array = webRequest.downloadHandler.text.Trim().Split('/');
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != "")
            {
                string[] array2 = array[i].Split(',');
                if (array2[0] == PlayerPrefs.GetString("NickName"))
                {
                    difficulty = array2[1];
                }
            }
        }
    }
	public static IEnumerator UpdateDificulty()
	{
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("editDifficulty", SceneLoader.difficulty);
        wWWForm.AddField("currentNickName", PlayerPrefs.GetString("NickName"));
        using UnityWebRequest www = UnityWebRequest.Post(UpdateDifficultyURL, wWWForm);
        yield return www.SendWebRequest();
    }
    static public IEnumerator GetScoreData()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentNickName", PlayerPrefs.GetString("NickName"));
        using UnityWebRequest webRequest = UnityWebRequest.Post(GetScoreURL, wWWForm);
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
        wWWForm.AddField("currentNickName", PlayerPrefs.GetString("NickName"));
        using UnityWebRequest www = UnityWebRequest.Post(UpdateScoreURL, wWWForm);
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

    static public IEnumerator DeleteUser(string name)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("currentNickName", name);
        using UnityWebRequest www = UnityWebRequest.Post(DeleteUserURL, wWWForm);
        yield return www.SendWebRequest();
    }
    static public void SetCurrentScore(string score)
    {
        currentScore = score;
    }
}
