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
			Lantern.activateDeactivateLights(true, base.gameObject);
		}
		else
		{
			Lantern.activateDeactivateLights(false, base.gameObject);
		}
	}
	private float Horizontal(float num)
	{
        return num * (speed + 0.375f * (float)GamePropieties.speed);
    }
	private float Vertical(float num) 
	{
        return num * (speed + 0.375f * (float)GamePropieties.speed);
    }

	private void FixedUpdate()
	{
		float hz = 0;
        float vt = 0;
		if (!Application.isMobilePlatform)
		{
			hz = Horizontal(Input.GetAxisRaw("Horizontal"));
			vt = Vertical(Input.GetAxisRaw("Vertical"));
		}
		else
		{
			hz = Horizontal(JoyStick.jHz);
            vt = Vertical(JoyStick.jVt);
        }
		Animator component = GetComponent<Animator>();
		if (hz != 0f || vt != 0f)
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
		rb.velocity = new Vector2(hz, vt);

		Vector3 pos = transform.position;
		float plyX = Mathf.Floor((pos.x - offsetX) * 100000) / 100000;
        float plyY = Mathf.Floor((pos.y - offsetY) * 100000) / 100000;
        MazeSaver.SavePlayerPosition(new Vector2(plyX, plyY));
	}

	public int gridX()
	{
		return (int)Mathf.Floor(transform.position.x / MazeBuilder2.gridDimension);
	}

	public int gridY()
	{
		return (int)Mathf.Round(transform.position.y / MazeBuilder2.gridDimension);
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
