// MaskSpawner
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MaskSpawner : CollectiblesManager
{
    [SerializeField]
    private float duration;

    [SerializeField]
    private Sprite[] marksSp;

    [SerializeField]
	private float maskOffsetX;

	[SerializeField]
	private float maskOffsetY;

	[SerializeField]
	private GameObject maskObj;

	public static MaskSpawner maskSpawnerScript { get; private set; }
    
    private static Image mark;
    
    private float timer = -1f;

    private static Text maskTimer;

    static private GameObject mask;

    static private float offsetX, offsetY;

    static private Transform parent;



    private void Awake()
	{
		maskSpawnerScript = this;
        if (mark == null)
        {
            mark = GameObject.Find("Mark").GetComponent<Image>();
        }
        if (maskTimer == null)
        {
            maskTimer = GameObject.Find("MaskTimer").GetComponent<Text>();
        }
    }
    private void Start()
    {
        if (MazeBuilder.spawnCamp)
        {
            StartCoroutine(Invincibility(duration - 1.51f));
        }
    }

    public void SetupMask()
    {
        mask = maskObj;
        offsetX = maskOffsetX;
        offsetY = maskOffsetY;
        parent = transform;
    }

    static public void MaskSpawn(Vector2 pos)
	{
        float x = pos.x * MazeBuilder2.gridDimension + offsetX;
        float y = pos.y * MazeBuilder2.gridDimension + offsetY;
        Object.Instantiate(mask, new Vector3(x, y, 0), Quaternion.identity, parent);
	}
    private void Update()
    {
        if (PlayerPrefs.GetInt("Collected") == 1)
        {
            mark.sprite = marksSp[1];
        }
        else
        {
            mark.sprite = marksSp[0];
        }
        if (timer > 0f)
        {
            maskTimer.text = Mathf.Round(timer).ToString();
            timer -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PutMask();
        }
    }
    public void PutMask()
    {
        if (MaskAvailable())
        {
            StartCoroutine(Invincibility(duration));
            PlayerPrefs.SetInt("Collected", 0);
            mark.sprite = marksSp[0];
        }
    }
    private bool MaskAvailable()
    {
        if (PlayerPrefs.GetInt("Collected") == 0) return false;
        if (CollectiblesManager.invincible) return false;
        if (Finish.panels[0].activeSelf) return false;
        if (Finish.panels[1].activeSelf) return false;
        if (!PauseMenu.isPauseMenu) return false;
        return true;
    }
    private IEnumerator Invincibility(float time)
    {
        CollectiblesManager.invincible = true;
        timer = time;
        yield return new WaitForSeconds(time);
        CollectiblesManager.invincible = false;
        maskTimer.text = "";
    }
}
