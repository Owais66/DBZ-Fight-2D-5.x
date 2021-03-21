using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
[CreateAssetMenu(fileName = "MainDatabase", menuName = "DBZ Fight 2D 5.x/MainDatabase", order = 0)]
public class MainDatabase : ScriptableObject
{
    public CharectersDict Charecters;
    public List<Arena> Arenas;
    [Serializable] public class CharectersDict : SerializableDictionaryBase<string, Charecter> { }
    //[Serializable] public class ArenaDict : SerializableDictionaryBase<string, Arena> { }
}
[Serializable]
public struct Charecter
{   
    public string BaseName;
    public DataBase DB;
    public Forms[] forms;
    [Serializable]public struct Forms
    {
        public Sprite FormFace;
        public string FormName;
        public int MinLvlReq;
    }
    public float Exp;
    public int Lvl;
    public float Strength, Speed, Power;
}

[Serializable]
public struct Arena
{   
    public string name;
    public Sprite sprite;
}