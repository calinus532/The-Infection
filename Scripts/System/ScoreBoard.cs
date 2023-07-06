// ScoreBoard
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
	private Text[] names;

	private Text[] levels;

	private Text[] times;

	private Image[] images;

	private float time;
	private string dificulty;

	private void Awake()
	{
		names = GameObject.Find("Names").GetComponentsInChildren<Text>();
		levels = GameObject.Find("Levels").GetComponentsInChildren<Text>();
		times = GameObject.Find("Times").GetComponentsInChildren<Text>();
		images = GameObject.Find("Countries").GetComponentsInChildren<Image>();
	}

	private void Start()
	{
		dificulty = WebConnection.difficulty;
        StartCoroutine(SetText());
	}
	public void SetDifficulty(string dif)
	{
		dificulty = dif;
        StartCoroutine(SetText());
    }

	private void Update()
	{
		if (time >= 2f)
		{
			StartCoroutine(SetText());
			time = 0f;
		}
		time += Time.deltaTime;
	}

	private IEnumerator SetText()
	{
		StartCoroutine(WebConnection.GetScoreboardData(WebConnection.GetScoreboardURL, dificulty));
		yield return StartCoroutine(WebConnection.GetScoreboardData(WebConnection.GetScoreboardURL, dificulty));
		for (int i = 0; i < 10; i++)
		{
			if (WebConnection.nickNames[i] != null)
			{
				names[i].text = WebConnection.nickNames[i];
				levels[i].text = WebConnection.highestLevels[i];
				times[i].text = timeTransformed(i);
				int countryIdx = int.Parse(WebConnection.countriesIdxes[i], CultureInfo.InvariantCulture.NumberFormat);
				images[i].sprite = CountrySelection.flagsSp[countryIdx];
			}
			else
			{
                names[i].text = "";
				levels[i].text = "";
				times[i].text = "";
				images[i].sprite = null;
            }
		}
	}

	private string timeTransformed(int idx)
	{
		float num = float.Parse(WebConnection.lowestTimes[idx], CultureInfo.InvariantCulture.NumberFormat);
		int num2 = (int)(num / 3600f);
		int num3 = (int)(num / 60f);
		int num4 = (int)(num % 60f);
		if (num3 < 10)
		{
			if (num4 < 10)
			{
				return num2 + ":0" + num3 + ":0" + num4;
			}
			return num2 + ":0" + num3 + ":" + num4;
		}
		if (num4 < 10)
		{
			return num2 + ":" + num3 + ":0" + num4;
		}
		return num2 + ":" + num3 + ":" + num4;
	}
}
