// Shadow
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shadow : MonoBehaviour
{
	private ShadowCaster2D shCast;

	private Vector3[] points;

	private void Awake()
	{
		shCast = GetComponent<ShadowCaster2D>();
	}

	private void Start()
	{
		activateShadow();
	}

	private void Update()
	{
		activateShadow();
	}

	private void activateShadow()
	{
		points = shCast.shapePath;
		if (!isOnScreen())
		{
			shCast.enabled = false;
		}
		else
		{
			shCast.enabled = true;
		}
	}

	private bool isOnScreen()
	{
		if (points[0].x < CameraMovement.leftWall && points[0].y > CameraMovement.upWall)
		{
			return false;
		}
		if (points[1].x < CameraMovement.leftWall && points[1].y < CameraMovement.downWall)
		{
			return false;
		}
		if (points[2].x > CameraMovement.rightWall && points[2].y < CameraMovement.downWall)
		{
			return false;
		}
		if (points[3].x > CameraMovement.rightWall && points[3].y > CameraMovement.upWall)
		{
			return false;
		}
		return true;
	}
}
