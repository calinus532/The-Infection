// MaskSpawner
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    
    private static Image mark;
    
    private float timer = -1f;

    private static Text maskTimer;

    static private GameObject mask;

    static private float offsetX, offsetY;

    static private Transform parent;

    static public List<Vector2> maskPos { private set; get; } = new List<Vector2>();

    private void Awake()
	{
        if (mark == null)
        {
            mark = GameObject.Find("Mark").GetComponent<Image>();
        }
        if (maskTimer == null)
        {
            maskTimer = GameObject.Find("MaskTimer").GetComponent<Text>();
        }
    }

    public void SetupMask()
    {
        maskPos = new List<Vector2>();
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
        maskPos.Add(new Vector2(x, y));
	}
    private void Update()
    {
        if (GamePropieties.collected)
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

        MazeSaver.SaveMaskPositions(maskPos);
    }
    public void PutMask()
    {
        if (MaskAvailable())
        {
            StartCoroutine(Invincibility(duration));
            GamePropieties.SetCollected(false);
            mark.sprite = marksSp[0];
        }
    }
    private bool MaskAvailable()
    {
        if (!GamePropieties.collected) return false;
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
