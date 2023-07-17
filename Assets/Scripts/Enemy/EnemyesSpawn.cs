// EnemyesSpawn
using System.Collections.Generic;
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

	static protected int idx;

	static protected int dirIdx;

	static protected float time;

	static protected List<Vector4> virusPositions = new List<Vector4>();

	public static float speed { get; private set; }

	private void Awake()
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
        speed = (float)(int)Mathf.Floor(GamePropieties.level / speedGrowth) * 0.0625f + 1.25f;
	}

	public void VirusSetup()
	{
		offsetX = virusOffsetX;
		offsetY = virusOffsetY;
		virusObj = virus;
		parent = transform;
		idx = 0;
		virusPositions = new List<Vector4>();
	}

	static public void spawnVirus(Vector4 propieties)
	{
		float virusX = propieties.x * MazeBuilder2.gridDimension + offsetX;
		float virusY = propieties.y * MazeBuilder2.gridDimension + offsetY;
		dirIdx = (int)propieties.z;
		time = propieties.w;
        virusPositions.Add(new Vector4(virusX, virusY, dirIdx, time));
        Instantiate(virusObj, new Vector3(virusX, virusY, 0f), Quaternion.identity, parent);
		idx++;
    }

    private void Update()
    {
		if (virusPositions != null)
				MazeSaver.SaveEnemyPositions(virusPositions);
    }
}
