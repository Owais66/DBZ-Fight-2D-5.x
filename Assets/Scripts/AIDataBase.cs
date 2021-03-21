using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System;
public enum State{
    Standing ,Crouching, Jump,StandFight, CrouchFight, inAirFight, HitRec, CrouchBlock, StandBlock, StandSpecial,KiBlast,KiCharge ,None, InsTrans}

public enum State2{Standing, Crouching, Jumping}


[CreateAssetMenu(fileName = "EnemyDB", menuName = "AIDataBase")]
public class AIDataBase : ScriptableObject
{   
    public AIDataDictionary AIdataDict;
    
    [Serializable] public class AIDataDictionary : SerializableDictionaryBase<State, _AIDataArray> { }
}
[Serializable]
public class _AIDataArray{
    public _AIData[] AIDatas;
    public int GetMinProbablity(List<_AIData> Data){
        if(Data.Count == 0) return 0;
        int Probablity = 0;
        foreach(var data in Data){
            if(data.Probablity<Probablity) Probablity = data.Probablity;
        }
        return Probablity;
    }
}
[Serializable]
public class _AIData{
    public enum Priority{Easy, Normal,Hard, Impossible}
    public Priority priority;
    [Range(1,100)]public int Probablity;
    public State AIState;
    [Range(-1,1)]public float Direction;
    public float MaxDuration, MinDuration;
    public float MinRange, MaxRange;
    [Range(0,100)]public float MinMana;
    public bool Grounded;
    public _AIData NewAIData(){
        _AIData NewData = new _AIData();
        NewData.Direction = this.Direction;
        NewData.priority = this.priority;
        NewData.AIState = this.AIState;
        NewData.MinRange = this.MinRange;
        NewData.MaxRange = this.MaxRange;
        NewData.MinDuration = this.MinDuration;
        NewData.MaxDuration = this.MaxDuration;
        NewData.MinMana = this.MinMana;
        return NewData;
    }
}