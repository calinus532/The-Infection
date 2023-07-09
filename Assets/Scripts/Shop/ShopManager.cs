// ShopManager
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	private void Awake()
	{
		GetComponentInChildren<Animator>().CrossFade("Normal", 0f, 0);
	}

	public static void incrementCoins(int amount)
	{
		PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + amount);
	}

	public static void decrementCoins(int amount)
	{
		PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - amount);
		if (PlayerPrefs.GetInt("Coins") < 0)
		{
			PlayerPrefs.SetInt("Coins", 0);
		}
	}

	private void Update()
	{
		GameObject.Find("Coins").GetComponent<Text>().text = PlayerPrefs.GetInt("Coins").ToString();
	}

	public void ResetShop()
	{
		int num = PlayerPrefs.GetInt("Coins");
		for (int i = 1; i < GetComponentsInChildren<ShopButton>().Length; i++)
		{
			num += GetComponentsInChildren<ShopButton>()[i].ResetCost();
		}
		PlayerPrefs.SetInt("Coins", num);
	}
}
