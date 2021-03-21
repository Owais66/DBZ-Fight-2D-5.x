using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class BoxGizmos : MonoBehaviour
{
    [SerializeField] BoxCollider2D BlockBox, HeadHurtBox, AbsHurtBox, LegsHurtBox, GrabBox;
    /// <summary>
    /// controlled by Database Editor when active;
    /// </summary>
    [HideInInspector] public Boxes hitBox, blockBox, headHurtBox, absHurtBox, legsHurtBox, grabBox;
    /// <summary>
    /// is Controlled by BoxController of this object when player hits;
    /// </summary>
    public Boxes HitBox;
    public bool DBEditActive;
    public void StartHitCoroutine(Boxes HB)
    {
        StartCoroutine(HitBoxCoroutine());
        HitBox = HB;
    }
    IEnumerator HitBoxCoroutine()
    {
        HitBoxGizmos = true;
        yield return new WaitForSeconds(2);
        HitBoxGizmos = false;
    }

    public void ResetAllBox(){
        Boxes empty = new Boxes();
        empty.Offset = Vector3.zero; empty.Size = Vector3.zero;
        hitBox = empty;
        blockBox = empty;
        headHurtBox = empty;
        absHurtBox = empty;
        legsHurtBox = empty;
        grabBox = empty;
    }
    private bool HitBoxGizmos; // Trigs Hitbox Gizmos on and off with HitBoxCoroutine its used only for devolopment build
    [SerializeField] bool ShowHitBox;
    [SerializeField] bool ShowHurtBox;
    [SerializeField] bool ShowGrabBox;
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        //HitBox
        Color red = Color.red;
        red.a = 0.5f;
        Gizmos.color = red;
        if (DBEditActive) Gizmos.DrawCube(hitBox.Offset, hitBox.Size);
        else if (HitBoxGizmos && ShowHitBox)
        {
            Gizmos.DrawCube(HitBox.Offset, HitBox.Size);
        }

        //BlockBox
        Color grey = Color.grey;
        grey.a = 0.5f;
        Gizmos.color = grey;
        if (DBEditActive)
        {
            Gizmos.DrawCube(blockBox.Offset, blockBox.Size);
        }
        else if (BlockBox.isActiveAndEnabled)
        {
            Gizmos.DrawCube(BlockBox.offset, BlockBox.size);
        }


        //HurtBox
        Color green = Color.green;
        green.a = 0.5f;
        Gizmos.color = green;
        if (DBEditActive)
        {
            Gizmos.DrawCube(headHurtBox.Offset, headHurtBox.Size);
            Gizmos.DrawCube(absHurtBox.Offset, absHurtBox.Size);
            Gizmos.DrawCube(legsHurtBox.Offset, legsHurtBox.Size);
        }
        else if (ShowHurtBox)
        {
            Gizmos.DrawCube(HeadHurtBox.offset, HeadHurtBox.size);
            Gizmos.DrawCube(AbsHurtBox.offset, AbsHurtBox.size);
            Gizmos.DrawCube(LegsHurtBox.offset, LegsHurtBox.size);
        }
        //GrabBox
        Color blue = Color.blue;
        blue.a = 0.5f;
        Gizmos.color = blue;
        if (DBEditActive)
        {
            Gizmos.DrawCube(grabBox.Offset, grabBox.Size);
        }
        else if (ShowGrabBox)
        {

            Gizmos.DrawCube(GrabBox.offset, GrabBox.size);
        }
    }
}