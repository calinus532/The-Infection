using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountrySelection : MonoBehaviour
{
    [SerializeField] private Sprite[] flags;
    static public Sprite[] flagsSp { private set; get; }
    private string[] countries;
    private Dropdown dropdown;
    private void Awake()
    {
        flagsSp = flags;
        GetCountryNames();
        if (GameObject.Find("CountriesDD"))
        {
            dropdown = GameObject.Find("CountriesDD").GetComponent<Dropdown>();
            dropdown.ClearOptions();
            DropDownAddOptions();
        }
        
    }
    private void GetCountryNames()
    {
        countries = new string[flags.Length];
        int countriesIdx = 0;
        foreach (Sprite f in flags)
        {
            string[] flagName = f.name.Split('_');
            string countryName = "";
            for (int i = 2; i < flagName.Length; i++)
            {
                if (i > 2) countryName += " ";
                countryName += flagName[i];
                countries[countriesIdx] = countryName;
            }
            countriesIdx++;
        }
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
        PlayerPrefs.SetInt("Country", dropdown.value);
    }
}
