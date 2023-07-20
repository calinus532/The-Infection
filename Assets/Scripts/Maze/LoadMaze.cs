using System.Collections.Generic;
using UnityEngine;

public class LoadMaze : MonoBehaviour
{
    static private string[] values;
    
    static public List<Vector3> mazeWallsPositions {  get; private set; }

    static public List<Vector2> mazeCorectPathPositions { get; private set; }

    static public Vector2 cornerPosition { get; private set; }

    static public Vector2 finishPosition { get; private set; }

    static public Vector2 playerPosition { get; private set; }

    static public List<Vector4> virusPropieties { get; private set; }

    static public List<Vector2> maskPositions { get; private set; }

    static public bool isLight { get; private set; }

    static public void DecodeMaze(List<string> list)
    {
        mazeWallsPositions = new List<Vector3>();
        values = list[0].Trim().Split('/');
        foreach (string s in values)
        {
            if (s == "") break;
            string[] pos = s.Split(',');
            int x = int.Parse(pos[0]);
            int y = int.Parse(pos[1]);
            int type = int.Parse(pos[2]);
            mazeWallsPositions.Add(new Vector3(x, y, type));
        }

        mazeCorectPathPositions = new List<Vector2>();
        values = list[1].Trim().Split('/');
        foreach (string s in values)
        {
            if (s == "") break;
            string[] pos = s.Split(',');
            int x = int.Parse(pos[0]);
            int y = int.Parse(pos[1]);
            mazeCorectPathPositions.Add(new Vector2(x, y));
        }

        string[] corner = list[2].Split(',');
        int cornerX = int.Parse(corner[0]);
        int cornerY = int.Parse(corner[1]);
        cornerPosition = new Vector2(cornerX, cornerY);

        string[] finish = list[3].Split(',');
        float finishX = float.Parse(finish[0]);
        float finishY = float.Parse(finish[1]);
        finishPosition = new Vector2(finishX, finishY);

        string[] player = list[4].Split(',');
        float playerX = float.Parse(player[0]);
        float playerY = float.Parse(player[1]);
        playerPosition = new Vector2(playerX, playerY);

        if (int.Parse(list[5]) == 1) isLight = true;
        else isLight = false;

        if (list[6] != "0")
        {
            virusPropieties = new List<Vector4>();
            values = list[6].Trim().Split('/');
            foreach (string s in values)
            {
                if (s == "") break;
                string[] props = s.Split(',');
                int x = int.Parse(props[0]);
                int y = int.Parse(props[1]);
                int dir = int.Parse(props[2]);
                float time = float.Parse(props[3]);
                virusPropieties.Add(new Vector4(x, y, dir, time));
            }

            maskPositions = new List<Vector2>();
            values = list[7].Trim().Split('/');
            foreach (string s in values)
            {
                if (s == "") break;
                string[] pos = s.Split(',');
                int x = int.Parse(pos[0]);
                int y = int.Parse(pos[1]);
                maskPositions.Add(new Vector2(x, y));
            }
        }
    }
}
