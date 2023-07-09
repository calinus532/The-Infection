// EnemyesSpawn
using UnityEngine;

public class EnemyesSpawn : MonoBehaviour
{
	[SerializeField]
	private GameObject virus;

	[SerializeField]
	private float virusOffsetX;

	[SerializeField]
	private float virusOffsetY;

	static public float offsetX, offsetY;
	
	static private GameObject virusObj;

	static private Transform parent;

	public static float speed { get; private set; }

	private void Start()
	{
		int speedGrowth = 10;
        switch (SceneLoader.difficulty)
        {
            case "Easy":
                speedGrowth = 15;
                break;
            case "Normal":
                speedGrowth = 10;
                break;
            case "Hard":
                speedGrowth = 7;
                break;
        }
        speed = (float)(int)Mathf.Floor(PlayerPrefs.GetInt("Level", 1) / speedGrowth) * 0.0625f + 1.25f;
	}

	public void VirusSetup()
	{
		offsetX = virusOffsetX;
		offsetY = virusOffsetY;
		virusObj = virus;
		parent = transform;
	}

	static public void spawnVirus(Vector2 pos)
	{
		float virusX = pos.x * MazeBuilder2.gridDimension + offsetX;
		float virusY = pos.y * MazeBuilder2.gridDimension + offsetY;

        Object.Instantiate(virusObj, new Vector3(virusX, virusY, 0f), Quaternion.identity, parent);
	}
}
