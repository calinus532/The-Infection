// ScoreBoard
using System.Collections;
using System.Collections.Generic;
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

	private Transform[] buttons;

	private Dropdown dropDown;

	private void Awake()
	{
		names = GameObject.Find("Names").GetComponentsInChildren<Text>();
		levels = GameObject.Find("Levels").GetComponentsInChildren<Text>();
		times = GameObject.Find("Times").GetComponentsInChildren<Text>();
		images = GameObject.Find("Countries").GetComponentsInChildren<Image>();
		buttons = GameObject.Find("DifficultyButtons").GetComponentsInChildren<Transform>();
        dropDown = GameObject.Find("MazeTypes").GetComponent<Dropdown>();
    }

	private void Start()
	{
		dificulty = WebConnection.difficulty;
        ChangeButtons();
        StartCoroutine(SetText());
	}
	
	public void SetDifficulty(string dif)
	{
		dificulty = dif;
        ChangeButtons();
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

    private void ChangeButtons()
    {
        for (int i = 1; i < buttons.Length / 2 + 1; i++) buttons[i].gameObject.SetActive(true);
        for (int i = 1; i < buttons.Length / 2 + 1; i++) buttons[i + 3].gameObject.SetActive(false);
        switch (dificulty)
        {
            case "Easy":
                {
                    buttons[1].gameObject.SetActive(false);
                    buttons[4].gameObject.SetActive(true);
                    break;
                }
            case "Normal":
                {
                    buttons[2].gameObject.SetActive(false);
                    buttons[5].gameObject.SetActive(true);
                    break;
                }
            case "Hard":
                {
                    buttons[3].gameObject.SetActive(false);
                    buttons[6].gameObject.SetActive(true);
                    break;
                }
        }
    }

    public void ChangeScoreboard()
	{
		StartCoroutine(SetText());
	}

	private IEnumerator SetText()
	{
		StartCoroutine(WebConnection.GetScoreboardData(dificulty, dropDown.value.ToString()));
		yield return StartCoroutine(WebConnection.GetScoreboardData(dificulty, dropDown.value.ToString()));
		for (int i = 0; i < 10; i++)
		{
			if (WebConnection.nickNames[i] != null)
			{
				names[i].text = WebConnection.nickNames[i];
				levels[i].text = WebConnection.highestLevels[i];
				times[i].text = timeTransformed(i);
				int countryIdx = int.Parse(WebConnection.countriesIdxes[i], CultureInfo.InvariantCulture.NumberFormat);
				images[i].color = Color.white;
				images[i].sprite = CountrySelection.flagsSp[countryIdx];
			}
			else
			{
                names[i].text = "";
				levels[i].text = "";
				times[i].text = "";
				images[i].color = new Color(0, 0, 0, 0);
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
