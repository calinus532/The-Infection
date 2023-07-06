// CollectiblesManager
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CollectiblesManager : MonoBehaviour
{
	public static bool invincible { get; protected set; }

	private void Awake()
	{
		invincible = false;
	}

	public void despawnObject(GameObject collectible)
	{
		SpriteRenderer component = collectible.GetComponent<SpriteRenderer>();
		Light2D[] componentsInChildren = collectible.GetComponentsInChildren<Light2D>();
		Collider2D component2 = collectible.GetComponent<Collider2D>();
		if (collectible.gameObject.name == "Finish_Collectible")
		{
			component.sortingLayerName = "Invisible";
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(value: false);
			}
		}
		if (collectible.gameObject.tag == "Mask")
		{
			component2.enabled = false;
			component.sortingLayerName = "Invisible";
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].gameObject.SetActive(value: false);
			}
		}
		if (collectible.gameObject.name == "Lantern")
		{
			component2.enabled = false;
			component.sortingLayerName = "Invisible";
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				componentsInChildren[k].gameObject.SetActive(value: false);
			}
			PlayerPrefs.SetInt("Lights", Mathf.Clamp(PlayerPrefs.GetInt("Lights") + 1, 0, 3));
		}
	}
}
