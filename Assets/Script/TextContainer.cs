using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Text Container")]
public class TextContainer : ScriptableObject
{
    [SerializeField] private FailReason failReason;
    [SerializeField] private LanguageString[] gameOver;
    [SerializeField] private LanguageString[] tutorial;
    [SerializeField] private LanguageString titleInfo;
    [SerializeField] private LanguageString credit;
    public FailReason FailReason { get { return failReason; } }
    public LanguageString[] GameOver { get { return gameOver; } }
    public LanguageString[] Tutorial { get { return tutorial; } }
    public LanguageString TitleInfo { get { return titleInfo; } }
    public LanguageString Credit { get { return credit; } }
}

[System.Serializable]
public class FailReason
{
    [SerializeField] private LanguageString[] utility;
    [SerializeField] private LanguageString[] roomRoom;
    [SerializeField] private LanguageString[] lineRoom;
    [SerializeField] private LanguageString[] passwordRoom;
    [SerializeField] private LanguageString[] returnRoom;
    public LanguageString[] Utility {  get { return utility; } }
    public LanguageString[] RoomRoom { get {  return roomRoom; } }
    public LanguageString[] LineRoom { get { return lineRoom; } }
    public LanguageString[] PasswordRoom { get { return passwordRoom; } }
    public LanguageString[] ReturnRoom { get { return returnRoom; } }
}
