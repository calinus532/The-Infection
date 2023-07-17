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
        GamePropieties.SetCoins(GamePropieties.coins + amount);
	}

	public static void decrementCoins(int amount)
	{
        GamePropieties.SetCoins(GamePropieties.coins - amount);
		if (GamePropieties.coins < 0)
            GamePropieties.SetCoins(0);
	}

	private void Update()
	{
		GameObject.Find("Coins").GetComponent<Text>().text = GamePropieties.coins.ToString();
	}

	public void ResetShop()
	{
		int value = GamePropieties.coins;
		for (int i = 1; i < GetComponentsInChildren<ShopButton>().Length; i++)
		{
			value += GetComponentsInChildren<ShopButton>()[i].ResetCost();
		}
        GamePropieties.SetCoins(value);
	}
}
