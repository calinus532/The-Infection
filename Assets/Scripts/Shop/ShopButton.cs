// ShopButton
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
	[SerializeField]
	private string varName;

	[SerializeField]
	private string buttonName;

	[SerializeField]
	private int initialInt;

	[SerializeField]
	private int maxValue;

	[SerializeField]
	private int cost;

	[SerializeField]
	private bool costIncrease;

	[SerializeField]
	private int idx;

	private static int[] currentCost = new int[4];

	private Text text;

	private Slider slider;

	[SerializeField]
	private AudioClip buySound;

	[SerializeField]
	private AudioClip invalidSound;

	private void Awake()
	{
		text = GetComponentInChildren<Text>();
		if (GetComponentInChildren<Slider>() != null)
		{
			slider = GetComponentInChildren<Slider>();
		}
		currentCost[idx] = 0;
		if (costIncrease)
		{
			for (int i = 0; i <= PlayerPrefs.GetInt(varName, initialInt); i++)
			{
				currentCost[idx] += cost + 10 * i;
			}
		}
		else
		{
			currentCost[idx] = cost;
		}
	}

	private void Update()
	{
		setButton();
		if (idx == 3)
		{
			GetComponentsInChildren<Text>()[1].text = PlayerPrefs.GetInt(varName).ToString();
		}
	}

	private void setButton()
	{
		if (PlayerPrefs.GetInt(varName, initialInt) < maxValue)
		{
			text.text = currentCost[idx].ToString();
		}
		else
		{
			text.text = "MAX";
		}
		if (GetComponentInChildren<Slider>() != null)
		{
			slider.value = PlayerPrefs.GetInt(varName, initialInt);
		}
	}

	public void incrementVar()
	{
		if (PlayerPrefs.GetInt("Coins") >= currentCost[idx] && text.text != "MAX")
		{
			PlaySound(buySound);
			PlayerPrefs.SetInt(varName, (int)Mathf.Clamp((float)PlayerPrefs.GetInt(varName, initialInt) + 1f, 0f, maxValue));
			ShopManager.decrementCoins(currentCost[idx]);
			if (costIncrease)
			{
				currentCost[idx] += cost + 10 * PlayerPrefs.GetInt(varName, initialInt);
			}
		}
		else
		{
			PlaySound(invalidSound);
		}
	}

	public void PlaySound(AudioClip clip)
	{
		GameObject.Find("AudioManager").GetComponentsInChildren<AudioSource>()[1].PlayOneShot(clip);
	}

	public int ResetCost()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 1; i <= PlayerPrefs.GetInt(varName, initialInt); i++)
		{
			num2 = ((!costIncrease) ? (-cost) : (num2 - (cost + 10 * (i - 1))));
			num -= num2;
		}
		PlayerPrefs.SetInt(varName, initialInt);
		currentCost[idx] = cost;
		return num;
	}
}
