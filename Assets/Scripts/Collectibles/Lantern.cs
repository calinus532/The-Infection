// Lantern
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lantern : CollectiblesManager
{
	private Light2D pathLight;

	private Light2D[] playerLights;

	static private SpriteRenderer spriteRend;

	static private Light2D[] light2D;

	static private Collider2D coll;

	static private bool isActive;

	static private Transform lightTransform;

	public static bool isLighting { get; private set; }

	private void Awake()
	{
		isLighting = false;
		pathLight = GameObject.Find("GlobalLight").GetComponent<Light2D>();
		playerLights = GameObject.Find("Player").GetComponentsInChildren<Light2D>();
		setLightIntensity();
	}

	public void SetupLantern()
	{
        isActive = false;
		lightTransform = transform;
		coll = GetComponent<Collider2D>();
        coll.enabled = false;
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.sortingLayerName = "Invisible";
        light2D = GetComponentsInChildren<Light2D>();
        for (int i = 0; i < light2D.Length; i++)
            light2D[i].enabled = false;

    }

	static public void ActivateLantern(int x, int y)
	{
		if (coll != null) coll.enabled = true;
		spriteRend.sortingLayerName = "Collectible";
		for (int i = 0; i < light2D.Length; i++)
		{
			light2D[i].enabled = true;
		}
		lightTransform.position = new Vector2((float)x * 0.875f + 0.4f, (float)y * 0.875f);
		isActive = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			LightMaze(false);
		}
		setLightIntensity();
		if (!SceneLoader.darkness && !isLighting)
		{
			if (isActive)
			{
				activateDeactivateLights(state: true, base.gameObject);
			}
		}
		else
		{
			activateDeactivateLights(state: false, base.gameObject);
		}
	}

	public static void activateDeactivateLights(bool state, GameObject gameObject)
	{
		Light2D[] componentsInChildren = gameObject.GetComponentsInChildren<Light2D>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = state;
		}
	}

	private void OnTriggerStay2D(Collider2D collider)
	{
		if (PlayerPrefs.GetInt("Lights") < 3)
		{
			despawnObject(base.gameObject);
		}
	}

	public async void LightMaze(bool button)
	{
		if (button)
		{
			GameSettings.PlaySound(SceneLoader.buttonPressedClip);
			await Task.Delay(100);
		}
        if (PlayerPrefs.GetInt("Lights") > 0 && !isLighting && PauseMenu.isPauseMenu && !Finish.panels[0].activeSelf && !Finish.panels[1].activeSelf)
		{
			StartCoroutine(IlightMaze());
			PlayerPrefs.SetInt("Lights", PlayerPrefs.GetInt("Lights") - 1);
		}
	}

	private IEnumerator IlightMaze()
	{
		isLighting = true;
		pathLight.intensity = 1f;
		yield return new WaitForSeconds(3.75f);
		pathLight.intensity = 0f;
		isLighting = false;
	}

	private void setLightIntensity()
	{
		for (int i = 0; i < playerLights.Length; i++)
		{
			playerLights[i].pointLightOuterRadius = 0.75f + 0.2f * (float)PlayerPrefs.GetInt("LightIntensity", 0);
		}
	}
}
