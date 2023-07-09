// Timer
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	private Text text;

	public static float time { get; private set; }

	private void Awake()
	{
		text = GameObject.Find("Timer").GetComponent<Text>();
	}

	private void Start()
	{
		time = 0f;
	}

	private void Update()
	{
		if (PlayerPrefs.GetInt("Win") != 1)
		{
			time += Time.deltaTime;
		}
		text.text = "Time: " + Mathf.Ceil(time);
	}
}
