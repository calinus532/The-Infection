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

	private Image image;

	private Slider slider;

	[SerializeField]
	private AudioClip buySound;

	[SerializeField]
	private AudioClip invalidSound;

	private void Awake()
	{
		text = GetComponentInChildren<Text>();
		image = GetComponentsInChildren<Image>()[1];
		if (GetComponentInChildren<Slider>() != null)
		{
			slider = GetComponentInChildren<Slider>();
		}
		currentCost[idx] = 0;
		if (costIncrease)
		{
			for (int i = 0; i <= GetValue(); i++)
			{
				currentCost[idx] += cost + 10 * i;
			}
		}
		else
		{
			currentCost[idx] = cost;
		}
	}

	private int GetValue()
	{
		switch(varName)
		{
			case "Health": return GamePropieties.health;
			case "Speed":  return GamePropieties.speed;
            case "LightIntensity": return GamePropieties.lightIntensity;
            case "Lights": return GamePropieties.lights;
        }
		return 0;
	}

    private void SetValue(int amount)
    {
        switch (varName)
        {
            case "Health":
                {
                    GamePropieties.SetHealth(amount);
					break;
                }
            case "Speed":
                {
                    GamePropieties.SetSpeed(amount);
					break;
                }
            case "LightIntensity":
                {
                    GamePropieties.SetLightIntensity(amount);
					break;
                }
            case "Lights":
                {
                    GamePropieties.SetLights(amount);
					break;
                }
        }
    }

    private void Update()
	{
		setButton();
		if (idx == 3)
		{
			GetComponentsInChildren<Text>()[1].text = GetValue().ToString();
		}
	}

	private void setButton()
	{
		if (GetValue() < maxValue)
		{
			image.color = new Color(0, 0, 0, 0);
			text.text = currentCost[idx].ToString();
		}
		else
		{
			image.color = Color.white;
			text.text = "";
		}
		if (GetComponentInChildren<Slider>() != null)
		{
			slider.value = GetValue();
		}
	}

	public void incrementVar()
	{
		if (GamePropieties.coins >= currentCost[idx] && text.text != "")
		{
			PlaySound(buySound);
			SetValue((int)Mathf.Clamp((float)GetValue() + 1f, 0f, maxValue));
			ShopManager.decrementCoins(currentCost[idx]);
			if (costIncrease)
			{
				currentCost[idx] += cost + 10 * GetValue();
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
        int differencePrice = 0;
        int currentPrice = 0;
        for (int i = 1; i <= GetValue(); i++)
        {
            if (costIncrease) currentPrice -= (cost + 10 * (i - 1));
            else currentPrice = -cost;
            differencePrice -= currentPrice;
        }
        SetValue(initialInt);
        currentCost[idx] = cost;
        return differencePrice;
    }
}
