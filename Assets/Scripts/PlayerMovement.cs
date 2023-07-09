// PlayerMovement
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	private Rigidbody2D rb;

	static private Transform playerTr;

	[SerializeField]
	private float speed;

	[SerializeField]
	private int numberOfFrames;

	private SpriteRenderer spriteRenderer;

	[Header("Offsets")]
	[SerializeField]
	private float playerOffsetX;

	[SerializeField]
	private float playerOffsetY;

	static private float offsetX, offsetY;

	public static PlayerMovement playerMovementScript { get; private set; }

	private void Awake()
	{
		Physics2D.IgnoreLayerCollision(3, 10, ignore: false);
		playerMovementScript = this;
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

    public void SetupPlayer()
    {
        offsetX = playerOffsetX;
        offsetY = playerOffsetY;
        playerTr = transform;
    }

    static public void startPlayer(float playerX, float playerY) {
		playerTr.position = new Vector3(playerX, playerY, 0f) + new Vector3(offsetX, offsetY, 0f); 
	}

	private void Update()
	{
		if (!Lantern.isLighting)
		{
			Lantern.activateDeactivateLights(state: true, base.gameObject);
		}
		else
		{
			Lantern.activateDeactivateLights(state: false, base.gameObject);
		}
	}
	private float Horizontal(float num)
	{
        return num * (speed + 0.375f * (float)PlayerPrefs.GetInt("Speed", 0));
    }
	private float Vertical(float num) 
	{
        return num * (speed + 0.375f * (float)PlayerPrefs.GetInt("Speed", 0));
    }

	private void FixedUpdate()
	{
		float num = 0;
        float num2 = 0;
		if (!Application.isMobilePlatform)
		{
			num = Horizontal(Input.GetAxisRaw("Horizontal"));
			num2 = Vertical(Input.GetAxisRaw("Vertical"));
		}
		else
		{
			num = Horizontal(JoyStick.jHz);
            num2 = Vertical(JoyStick.jVt);
        }
		Animator component = GetComponent<Animator>();
		if (num != 0f || num2 != 0f)
		{
			if (!CollectiblesManager.invincible)
			{
				component.CrossFade("Move", 0f, 0);
			}
			else
			{
				component.CrossFade("MoveMask", 0f, 0);
			}
		}
		else if (!CollectiblesManager.invincible)
		{
			component.CrossFade("Idle", 0f, 0);
		}
		else
		{
			component.CrossFade("IdleMask", 0f, 0);
		}
		rb.velocity = new Vector2(num, num2);
	}

	public int gridX()
	{
		return (int)Mathf.Floor(base.transform.position.x / MazeBuilder.gridDimension);
	}

	public int gridY()
	{
		return (int)Mathf.Round(base.transform.position.y / MazeBuilder.gridDimension);
	}

	public IEnumerator IFrames()
	{
		Physics2D.IgnoreLayerCollision(3, 10, ignore: true);
		for (int i = 0; i < numberOfFrames; i++)
		{
			spriteRenderer.color = new Color(1f, 1f, 1f, 0.75f);
			yield return new WaitForSeconds(1.5f / (float)(numberOfFrames * 2));
			spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
			yield return new WaitForSeconds(1.5f / (float)(numberOfFrames * 2));
		}
		Physics2D.IgnoreLayerCollision(3, 10, ignore: false);
	}
}
