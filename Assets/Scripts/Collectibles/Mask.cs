// Mask
using UnityEngine;
using UnityEngine.UI;

public class Mask : CollectiblesManager
{
	private static SpriteRenderer nurse;

	[SerializeField]
	private AudioClip collectedSound;

	[SerializeField]
	private float cameraOffsetX;

	[SerializeField]
	private float cameraOffsetY;

	private void Awake()
	{
		if (nurse == null)
		{
			nurse = GameObject.Find("Nurse").GetComponent<SpriteRenderer>();
		}
	}

	private void Update()
	{
		if (!GameSettings.darkness && !Lantern.isLighting)
		{
			if (!isOnScreen())
			{
				Lantern.activateDeactivateLights(state: false, base.gameObject);
			}
			else
			{
				Lantern.activateDeactivateLights(state: true, base.gameObject);
			}
		}
		else
		{
			Lantern.activateDeactivateLights(state: false, base.gameObject);
		}
	}
	private bool isOnScreen()
	{
		if (base.transform.position.x >= CameraMovement.rightWall + cameraOffsetX)
		{
			return false;
		}
		if (base.transform.position.y >= CameraMovement.upWall - 0.375f + cameraOffsetY)
		{
			return false;
		}
		if (base.transform.position.x <= CameraMovement.leftWall - cameraOffsetX)
		{
			return false;
		}
		if (base.transform.position.y <= CameraMovement.downWall - cameraOffsetY)
		{
			return false;
		}
		return true;
	}

	private void OnTriggerStay2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player" && !GamePropieties.collected && !invincible)
		{
			despawnObject(gameObject);
			GamePropieties.SetCollected(true);
			GameSettings.PlaySound(collectedSound);
			MaskSpawner.maskPos.Remove(new Vector2(transform.position.x, transform.position.y));
		}
	}
}
