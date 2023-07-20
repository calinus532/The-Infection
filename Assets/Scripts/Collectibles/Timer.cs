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

	static public void ResetTime() => time = 0;

	private void Update()
	{
		if (!GamePropieties.win)
		{
			time += Time.deltaTime;
		}
		text.text = "Time: " + Mathf.Ceil(time);
	}
}
