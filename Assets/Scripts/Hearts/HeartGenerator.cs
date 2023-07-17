// HeartGenerator
using UnityEngine;

public class HeartGenerator : MonoBehaviour
{
	[SerializeField]
	private int numberOfHearts;

	[SerializeField]
	private GameObject heart;

	public static HeartGenerator heartGeneratorScript { get; private set; }

	private void Awake()
	{
		heartGeneratorScript = this;
		GameObject[] array = new GameObject[numberOfHearts];
		for (int i = 0; i < numberOfHearts; i++)
		{
			array[i] = Object.Instantiate(heart, Vector3.zero, Quaternion.identity, base.transform);
			array[i].GetComponent<RectTransform>().localPosition = new Vector3(i * 100, 0f, 0f);
			array[i].GetComponent<Heart>().setHeartNumber(i + 1);
		}
	}

	public void takeDamage(int damage)
	{
		GamePropieties.SetHealth(GamePropieties.health - damage);
	}

	private void Update()
	{
		if (GamePropieties.health < 1 && GameObject.FindWithTag("Player") != null)
		{
			GamePropieties.SetLose(true);
            GameObject.FindWithTag("Player").SetActive(value: false);
		}
	}
}
