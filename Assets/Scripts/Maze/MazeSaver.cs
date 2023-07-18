// MazeSaver
using System.Collections.Generic;
using UnityEngine;

public class MazeSaver : MonoBehaviour
{
    static private List<string> mazeWallsPositions;

    static private List<string> mazeCorectPathPositions;

    static private string cornerPosition;

    static private string finishPosition;

    static private string playerPosition;

    static private List<string> virusPositions;

    static private List<string> maskPositions;

    static private string isLight = "0";

    static private string mazeType = "0";

    static public List<string> mazePositions { private set; get; }

    static public void SaveMazeWalls(List<Vector3> positions)
    {
        mazeWallsPositions = new List<string>();
        foreach (Vector3 position in positions)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            int type = (int)position.z;
            mazeWallsPositions.Add(x + "," + y + "," + type);
        }
    }

    static public void SaveMazeCorrectPath(List<Vector2> positions)
    {
        mazeCorectPathPositions = new List<string>();
        foreach (Vector2 position in positions)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            mazeCorectPathPositions.Add(x + "," + y);
        }
    }

    static public void SaveCornerPosition(Vector2 position)
        => cornerPosition = position.x + "," + position.y;

    static public void SaveFinshPosition(Vector2 position)
        => finishPosition = position.x + "," + position.y; 

    static public void SavePlayerPosition(Vector2 position)
        => playerPosition = position.x + "," + position.y;

    static public void SaveEnemyPositions(List<Vector4> positions)
    {
        virusPositions = new List<string>();
        foreach (Vector4 propieties in positions)
        {
            int x = (int)propieties.x;
            int y = (int)propieties.y;
            int dir = (int)propieties.z;
            float time = propieties.w;
            virusPositions.Add(x + "," + y + "," + dir + "," + time);
        }
    }

    static public void SaveMaskPositions(List<Vector2> positions)
    {
        maskPositions = new List<string>();
        foreach (Vector2 position in positions)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            maskPositions.Add(x + "," + y);
        }
    }

    static public void SaveLightCondition(int condition) => isLight = condition.ToString();

    static public void SaveMazeType(int type) => mazeType = type.ToString();

    static public void SaveMaze()
    {
        mazePositions = new List<string>();
        string temp = "";
        foreach (string s in mazeWallsPositions)
        {
            temp += s + "/";
        }
        mazePositions.Add(temp);
        temp = "";
        foreach (string s in mazeCorectPathPositions)
        {
            temp += s + "/";
        }
        mazePositions.Add(temp);
        mazePositions.Add(cornerPosition);
        mazePositions.Add(finishPosition);
        mazePositions.Add(playerPosition);
        mazePositions.Add(isLight);
        if (virusPositions == null)
        {
            mazePositions.Add("0");
            mazePositions.Add("0");
        }
        else
        {
            temp = "";
            foreach (string s in virusPositions)
            {
                temp += s + "/";
            }
            mazePositions.Add(temp);
            temp = "";
            foreach (string s in maskPositions)
            {
                temp += s + "/";
            }
            mazePositions.Add(temp);
        }
        mazePositions.Add(mazeType);
    }
}
