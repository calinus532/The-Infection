using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    static public bool newGame;
    
    private void Awake()
    {
        Time.timeScale = 1f;
        GetComponent<MazeBuilder2>().MazeAwake(newGame);
        newGame = false;
    }
}
