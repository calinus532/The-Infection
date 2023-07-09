using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("NewLevel", 0);
        GetComponent<MazeBuilder2>().MazeAwake();
    }
}
