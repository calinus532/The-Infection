// Heart
using UnityEngine;
using UnityEngine.UI;

public class Heart : HeartGenerator
{
	private int heartNumber;

	private Image heartType;

	[SerializeField]
	private Sprite[] image;

	private static Sprite currentSprite;

	private void Awake()
	{
		heartType = GetComponent<Image>();
		chooseSprite();
	}

	public void setHeartNumber(int idx)
	{
		heartNumber = idx;
	}

	private void Update()
	{
		chooseSprite();
	}

	private void chooseSprite()
	{
		if (PlayerPrefs.GetInt("Health", 5) < heartNumber)
		{
			currentSprite = image[0];
		}
		else
		{
			currentSprite = image[1];
		}
		heartType.sprite = currentSprite;
	}
}
