// Arrow
using UnityEngine;

public class Arrow : MonoBehaviour
{
	private Transform finish;

	[SerializeField]
	private float cameraOffsetX;

	[SerializeField]
	private float cameraOffsetY;

	private float arrowX;

	private float arrowY;

	[SerializeField]
	private float leftRight;

	[SerializeField]
	private float upDown;

	private float rotation;

	private void Awake()
	{
		finish = GameObject.Find("Finish_Collectible").transform;
		SpawnArrow();
	}

	private void SpawnArrow()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		isOnScreen();
		if (!isOnScreen())
		{
			component.sortingLayerName = "Camera";
			base.transform.localPosition = new Vector3(arrowX, arrowY, base.transform.localPosition.z);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
		}
		else
		{
			component.sortingLayerName = "Invisible";
		}
	}

	private void Update()
	{
		SpawnArrow();
	}

	private bool isOnScreen()
	{
		Transform transform = GetComponentsInParent<Transform>()[1];
		bool[] array = new bool[4];
		if (finish.position.x >= CameraMovement.rightWall + cameraOffsetX)
		{
			array[0] = true;
		}
		if (finish.position.y >= CameraMovement.upWall + cameraOffsetY)
		{
			array[1] = true;
		}
		if (finish.position.x <= CameraMovement.leftWall - cameraOffsetX)
		{
			array[2] = true;
		}
		if (finish.position.y <= CameraMovement.downWall - cameraOffsetY)
		{
			array[3] = true;
		}
		if (array[0])
		{
			rotation = directions(-90f, array[3], array[1]);
			arrowX = leftRight;
			if (!array[1] && !array[3])
			{
				arrowY = finish.position.y - transform.position.y;
			}
		}
		if (array[1])
		{
			rotation = directions(0f, array[0], array[2]);
			arrowY = upDown;
			if (!array[0] && !array[2])
			{
				arrowX = finish.position.x - transform.position.x;
			}
		}
		if (array[2])
		{
			rotation = directions(90f, array[1], array[3]);
			arrowX = 0f - leftRight;
			if (!array[1] && !array[3])
			{
				arrowY = finish.position.y - transform.position.y;
			}
		}
		if (array[3])
		{
			rotation = directions(180f, array[2], array[0]);
			arrowY = 0f - upDown;
			if (!array[0] && !array[2])
			{
				arrowX = finish.position.x - transform.position.x;
			}
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i])
			{
				return false;
			}
		}
		return true;
	}

	private float directions(float Lrotation, bool direction1, bool direction2)
	{
		if (direction1)
		{
			return Lrotation - 45f;
		}
		if (direction2)
		{
			return Lrotation + 45f;
		}
		return Lrotation;
	}
}
