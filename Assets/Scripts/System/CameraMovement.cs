// CameraMovement
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	private Transform playerTr;

	[SerializeField]
	private float up;

	[SerializeField]
	private float right;

	[SerializeField]
	private float down;

	[SerializeField]
	private float left;

	public static float leftWall { get; private set; }

	public static float upWall { get; private set; }

	public static float rightWall { get; private set; }

	public static float downWall { get; private set; }

	private void Awake()
	{
		getWalls();
	}

	private void Start()
	{
		playerTr = GameObject.FindWithTag("Player").GetComponentsInChildren<Transform>()[1];
	}

	private void Update()
	{
		float x = 0f;
		float y = 0f;
		if (playerTr.position.x >= right)
		{
			x = playerTr.position.x - right;
		}
		if (playerTr.position.x <= left)
		{
			x = playerTr.position.x - left;
		}
		if (playerTr.position.y >= up)
		{
			y = playerTr.position.y - up;
		}
		if (playerTr.position.y <= down)
		{
			y = playerTr.position.y - down;
		}
		getWalls();
		base.transform.position = new Vector3(x, y, base.transform.position.z);
	}

	private void getWalls()
	{
		leftWall = GameObject.Find("LeftWall").transform.position.x;
		upWall = GameObject.Find("TopWall").transform.position.y;
		rightWall = GameObject.Find("RightWall").transform.position.x;
		downWall = GameObject.Find("UnderWall").transform.position.y;
	}
}
