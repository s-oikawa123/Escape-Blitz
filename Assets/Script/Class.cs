using Container;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting
{
    public static Language language;
}
// ‘½Œ¾Œê•Û‘¶ƒNƒ‰ƒX
[System.Serializable]
public class LanguageString
{
    public bool expandFlag;
    public string english;
    public string japanese;

    public string GetString()
    {
        return Setting.language switch
        {
            Language.English => english,
            Language.Japanese => japanese,
            _ => english,
        };
    }
}


namespace Container
{
    public enum Language
    {
        English,
        Japanese
    }
}
