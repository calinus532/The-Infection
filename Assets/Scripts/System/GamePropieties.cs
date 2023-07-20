using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePropieties : WebConnection
{
    private List<string> gamePropieties;

    public void GetGameProps()
    {
        StartCoroutine(GetValues());
    }

    public IEnumerator GetValues()
    {
        StartCoroutine(GetGamePropsData());
        yield return StartCoroutine(GetGamePropsData());
        StartCoroutine(SetValues());
        yield return StartCoroutine(SetValues());
    }

    static public string userName { private set; get; } = string.Empty;

    static public int country { get; private set; } = 0;
    static public void SetCountry(int idx) => country = idx;

    private IEnumerator SetValues()
    {
        string[] props;

        if (gameProps.Count == 0) yield break;
        userName = gameProps[0];
        country = int.Parse(gameProps[1]);

        if (gameProps[2] == "") yield break;
        props = gameProps[2].Split('/');
        level = int.Parse(props[0]);
        win = IntToBool(int.Parse(props[1]));
        lose = IntToBool(int.Parse(props[2]));
        displayTime = float.Parse(props[3]);
        highestLevel = int.Parse(props[4]);
        lowestTime = float.Parse(props[5]);
        newGame = IntToBool(int.Parse(props[6]));

        props = gameProps[3].Split('/');
        coins = int.Parse(props[0]);
        health = int.Parse(props[1]);
        speed = int.Parse(props[2]);
        lightIntensity = int.Parse(props[3]);
        lights = int.Parse(props[4]);

        props = gameProps[4].Split("/");
        collected = IntToBool(int.Parse(props[0]));
        darkness = IntToBool(int.Parse(props[1]));
        noEnemies = IntToBool(int.Parse(props[2]));
    }

    private string levelProps() => level + "/" + BoolToInt(win) + "/" + BoolToInt(lose)
                                    + "/" + displayTime + "/" + highestLevel + "/" + lowestTime + "/" + BoolToInt(newGame);
    private string shopProps() => coins + "/" + health + "/" + speed + "/" + lightIntensity + "/" + lights;

    private string playerProps() => BoolToInt(collected) + "/" + BoolToInt(darkness) + "/" + BoolToInt(noEnemies);

    static public int level { private set; get; } = 1;
    static public void SetLevel(int lvl) => level = lvl;

    static public bool win { private set; get; } = false;
    static public void SetWin(bool cond) => win = cond;

    static public bool lose { private set; get; } = false;
    static public void SetLose(bool cond) => lose = cond;

    static public float displayTime { private set; get; } = 0;
    static public void SetDisplayTime(float seconds) => displayTime = seconds;

    static public int highestLevel { private set; get; } = 0;
    static public void SetHighestLevel(int lvl) => highestLevel = lvl;

    static public float lowestTime { private set; get; } = 0;
    static public void SetLowestTime(float seconds) => lowestTime = seconds;

    static public bool newGame { private set; get; } = false;
    static public void SetNewGame(bool isNew) => newGame = isNew;

    static public int coins { private set; get; } = 0;
    static public void SetCoins(int amount) => coins = amount;

    static public int health { private set; get; } = 5;
    static public void SetHealth(int amount) => health = amount;

    static public int speed { private set; get; } = 0;
    static public void SetSpeed(int amount) => speed = amount;

    static public int lightIntensity { private set; get; } = 0;
    static public void SetLightIntensity(int amount) => lightIntensity = amount;

    static public int lights { private set; get; } = 0;
    static public void SetLights(int amount) => lights = amount;

    static public bool collected { private set; get; } = false;
    static public void SetCollected(bool isTrue) => collected = isTrue;

    static public bool darkness { private set; get; } = false;
    static public void SetDarkness(bool isTrue) => darkness = isTrue;

    static public bool noEnemies { private set; get; } = false;
    static public void SetNoEnemies(bool isTrue) => noEnemies = isTrue;

    static public void InitializeVars()
    {
        userName = string.Empty;
        country = 0;
        level = 1;
        win = false;
        lose = false;
        displayTime = 0;
        highestLevel = 0;
        lowestTime = 0;
        newGame = false;
        coins = 0;
        health = 5;
        speed = 0;
        lightIntensity = 0;
        lights = 0;
        collected = false;
        darkness = false;
        noEnemies = false;
    }
    
    public void SaveGameSettings()
    {
        gamePropieties = new List<string>()
        {
            country.ToString(),
            levelProps(),
            shopProps(),
            playerProps()
        };

        StartCoroutine(UpdateGamePropsData(gamePropieties, userName));
    }

    private int BoolToInt(bool value)
    {
        if (value) return 1;
        else return 0;
    }

    private bool IntToBool(int value)
    {
        if (value == 1) return true;
        else return false;
    }

}
