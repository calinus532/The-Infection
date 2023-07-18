using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountrySelection : MonoBehaviour
{
    [SerializeField] private Sprite[] flags;

    private Dropdown dropdown;

    static public Sprite[] flagsSp { private set; get; }
    
    private string[] countries;
    
    private void Awake()
    {
        flagsSp = flags;
    }

    public void SetupCountry()
    {
        GetCountryNames();
        dropdown = GameObject.Find("CountriesDD").GetComponent<Dropdown>();
        dropdown.ClearOptions();
        DropDownAddOptions();
    }
    private void GetCountryNames()
    {
        countries = new string[flags.Length];
        for(int i = 0; i < flags.Length; i++)
            countries[i] = flags[i].name;
    }
    private void DropDownAddOptions()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < flags.Length; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(countries[i], flags[i]);
            options.Add(option);
        }
        dropdown.AddOptions(options);
    }
    public void SetCountry()
    {
        GamePropieties.SetCountry(dropdown.value);
    }
}
