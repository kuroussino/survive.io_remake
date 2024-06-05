using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;

[Serializable]
public class ItemReference
{
    public static string[] PopupOptions => new string[]
       {
            "Weapon",
            "Health Pack",
            "Armor Pack"
       };

    const string defaultChosenValue = "weapon";
    [SerializeField] string chosenValue = defaultChosenValue;
    [SerializeField] A_Weapon weapon;
    [SerializeField] HealthPack healthPack;
    [SerializeField] ArmorPack armorPack;
    public I_Item value => GetValue();

    I_Item GetValue()
    {
        I_Item item = weapon;
        int index = StringValueToIndex(chosenValue);
        switch (index)
        {
            case 0:
                item = weapon;
                break;
            case 1:
                item = healthPack;
                break;
            case 2:
                item = armorPack;
                break;
            default:
                item = weapon;
                break;
        }
        return item;
    }
    public static int StringValueToIndex(string value)
    {
        for (int i = 0; i < PopupOptions.Length; i++)
        {
            if (PopupOptions[i] == value)
                return i;
        }
        return 0;
    }
}
