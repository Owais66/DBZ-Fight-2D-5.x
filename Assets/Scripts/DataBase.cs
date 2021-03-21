using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System;

[CreateAssetMenu(fileName = "PlayerDB", menuName = "DataBase")]
public class DataBase : ScriptableObject
{
    public Hitbox HitBoxes;
    public BlockBox BlockBoxes;
    public HurtBox HurtBoxes;
    [Serializable] public class Hitbox : SerializableDictionaryBase<string, _HitBox> { }
    [Serializable] public class BlockBox : SerializableDictionaryBase<string, _BlockBox> { }
    [Serializable] public class HurtBox : SerializableDictionaryBase<string, _HurtBox> { }
}
[Serializable]
public class Boxes
{   
    public Boxes(){}
    public Boxes(Vector3 offset, Vector2 size){Offset = offset; Size = size;}
    public Vector3 Offset;
    public Vector2 Size;
}
[Serializable]
public class _HitBox
{
    public Boxes Hitbox;
    public string Effect;
    public float Damage;
    public int StandHRID, CrouchHRID, InAirHRID;
}
[Serializable]
public class _BlockBox
{   
    public Boxes BlockBox;
}
[Serializable]
public class _HurtBox
{   
    public HBT HurtBoxType;
    public Boxes Head;
    public Boxes Abs;
    public Boxes Legs;
    public Boxes GrabBox;
    public Vector2 CenterPos;
}
[Serializable]
public class HBT{
    public State CMS;
    public bool canBlock, canJump, canFight, canRun, canCrouch;
    public State2 CMS2;

     /// <summary>
    /// is Used to create  a new memory of HBT so that it does not reference it to Database HBT;
    /// </summary>
    /// <param name="hbt"></param>
    public static HBT NewHBT(HBT hbt)
    {
        HBT Newhbt = new HBT();
        Newhbt.CMS = hbt.CMS;
        Newhbt.CMS2 = hbt.CMS2;
        Newhbt.canRun = hbt.canRun;
        Newhbt.canBlock = hbt.canBlock;
        Newhbt.canCrouch = hbt.canCrouch;
        Newhbt.canFight = hbt.canFight;
        Newhbt.canJump = hbt.canJump;
        return Newhbt;
    }
   
    /// <summary>
    /// Returns True if hbt1 and hbt2 has same values
    /// </summary>
    /// <param name="hbt1"></param>
    /// <param name="hbt2"></param>
    /// <returns></returns>
    public static bool CompareHBT(HBT hbt1, HBT hbt2){
        if(hbt1.CMS != hbt2.CMS) return false;
        if(hbt1.CMS2 != hbt2.CMS2) return false;
        if(hbt1.canRun != hbt2.canRun) return false;
        if(hbt1.canBlock != hbt2.canBlock) return false;
        if(hbt1.canCrouch != hbt2.canCrouch) return false;
        if(hbt1.canFight != hbt2.canFight) return false;
        if(hbt1.canJump != hbt2.canJump) return false;
        return  true;
    }
    public bool CheckFighting(){
        if(this.CMS == State.StandFight || this.CMS == State.CrouchFight|| this.CMS == State.inAirFight) return true;
        return false;
    }
}