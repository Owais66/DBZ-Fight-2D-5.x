#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(DataBase))]
public class DataBaseEditor : Editor
{
    GameObject Player;
    DataBase DB;
    GameObject BoxSprite;
    BoxGizmos boxGizmos;
    string BoxName;
    _HitBox hitBox;
    _BlockBox blockBox;
    _HurtBox hurtBox;

    private void OnEnable()
    {
        Player = GameObject.Find(target.name.TrimEnd(new char[] { 'D', 'B' }));
        BoxSprite = Player.transform.Find("Box").gameObject;
        DB = target as DataBase;
        EditorUtility.SetDirty(DB);
        boxGizmos = Player.GetComponent<BoxGizmos>();
        
    }
    private void Toogle()
    {   
        bool toogle = !boxGizmos.DBEditActive;
        BoxSprite.SetActive(toogle);
        boxGizmos.DBEditActive = toogle;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("BoxName");
        BoxName = EditorGUILayout.TextField(BoxName);
        if (GUILayout.Button("Get HitBox"))
        {
            hitBox = null;
            if (DB.HitBoxes.ContainsKey(BoxName)) { hitBox = DB.HitBoxes[BoxName]; Debug.Log("HitBox Found"); } else { Debug.Log("HitBox Not Found"); }
            if (hitBox != null) boxGizmos.hitBox = hitBox.Hitbox; else boxGizmos.hitBox = new _HitBox().Hitbox;
            hitBox = null;
        }
        if (GUILayout.Button("Get BlockBox"))
        {
            blockBox = null;
            if (DB.BlockBoxes.ContainsKey(BoxName)) { blockBox = DB.BlockBoxes[BoxName]; Debug.Log("BlockBox Found"); } else { Debug.Log("BlockBox Not Found"); }
            if (blockBox != null) boxGizmos.blockBox = blockBox.BlockBox; else boxGizmos.blockBox = new _BlockBox().BlockBox;
            blockBox = null;
        }
        if (GUILayout.Button("Get HurtBox"))
        {
            hurtBox = null;
            if (DB.HurtBoxes.ContainsKey(BoxName)) { hurtBox = DB.HurtBoxes[BoxName]; Debug.Log("HurtBox Found"); } else { Debug.Log("HurtBox Not Found"); }
            if (hurtBox == null)
            {
                _HurtBox hurt = new _HurtBox();
                boxGizmos.headHurtBox = hurt.Head;
                boxGizmos.absHurtBox = hurt.Abs;
                boxGizmos.legsHurtBox = hurt.Legs;
                boxGizmos.grabBox = hurt.GrabBox;
                return;
            }
            boxGizmos.headHurtBox = hurtBox.Head;
            boxGizmos.absHurtBox = hurtBox.Abs;
            boxGizmos.legsHurtBox = hurtBox.Legs;
            boxGizmos.grabBox = hurtBox.GrabBox;
        }

        // Update Box
        if (GUILayout.Button("Update HitBox"))
        {
            Boxes spritebox = GetSpriteBox();
            if (DB.HitBoxes.ContainsKey(BoxName))
            {
                DB.HitBoxes[BoxName].Hitbox = spritebox; Debug.Log("HitBox Updated");
            }
            else { _HitBox hb = new _HitBox(); hb.Hitbox = spritebox; DB.HitBoxes.Add(BoxName, hb); Debug.Log("HitBox Created"); }
            boxGizmos.hitBox = spritebox;
            hitBox = null;
        }
        if (GUILayout.Button("Update BlockBox"))
        {
            Boxes spritebox = GetSpriteBox();
            if (DB.BlockBoxes.ContainsKey(BoxName))
            {
                DB.BlockBoxes[BoxName].BlockBox = spritebox; Debug.Log("BlockBox Updated");
            }
            else { _BlockBox hb = new _BlockBox(); hb.BlockBox = spritebox; DB.BlockBoxes.Add(BoxName, hb); Debug.Log("BlockBox Created"); }
            boxGizmos.blockBox = spritebox;
            blockBox = null;
        }
        if (GUILayout.Button("Update HeadBox"))
        {
            Boxes spritebox = GetSpriteBox();
            if (DB.HurtBoxes.ContainsKey(BoxName))
            {
                DB.HurtBoxes[BoxName].Head = spritebox; Debug.Log("HeadHurt Updated");
            }
            else { _HurtBox hb = new _HurtBox(); hb.Head = spritebox; DB.HurtBoxes.Add(BoxName, hb); Debug.Log("HeadHurt Created"); }
            boxGizmos.headHurtBox = spritebox;

        }
        if (GUILayout.Button("Update AbsBox"))
        {
            Boxes spritebox = GetSpriteBox();
            if (DB.HurtBoxes.ContainsKey(BoxName))
            {
                DB.HurtBoxes[BoxName].Abs = spritebox; Debug.Log("AbsHurt Updated");
            }
            else { _HurtBox hb = new _HurtBox(); hb.Abs = spritebox; DB.HurtBoxes.Add(BoxName, hb); Debug.Log("AbsHurt Created"); }
            boxGizmos.absHurtBox = spritebox;

        }
        if (GUILayout.Button("Update LegsBox"))
        {
            Boxes spritebox = GetSpriteBox();
            if (DB.HurtBoxes.ContainsKey(BoxName))
            {
                DB.HurtBoxes[BoxName].Legs = spritebox; Debug.Log("LegsHurt Updated");
            }
            else { _HurtBox hb = new _HurtBox(); hb.Legs = spritebox; DB.HurtBoxes.Add(BoxName, hb); Debug.Log("LegsHurt Created"); }
            boxGizmos.legsHurtBox = spritebox;

        }
        if (GUILayout.Button("Update GrabBox"))
        {
            Boxes spritebox = GetSpriteBox();
            if (DB.HurtBoxes.ContainsKey(BoxName))
            {
                DB.HurtBoxes[BoxName].GrabBox = spritebox; Debug.Log("GrabBox Updated");
            }
            else { _HurtBox hb = new _HurtBox(); hb.GrabBox = spritebox; DB.HurtBoxes.Add(BoxName, hb); Debug.Log("GrabBox Created"); }
            boxGizmos.grabBox = spritebox;

        }
        if (GUILayout.Button("Reset All"))
        {
            boxGizmos.ResetAllBox();

        }
        if(GUILayout.Button("Toogle")){
            Toogle();
        }
    }
    Boxes GetSpriteBox()
    {
        Boxes box = new Boxes();
        box.Offset = BoxSprite.transform.localPosition;
        box.Size = BoxSprite.transform.localScale;
        return box;
    }

}
#endif