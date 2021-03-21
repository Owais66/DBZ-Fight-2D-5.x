#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class ColliderGizmos : MonoBehaviour {
    [SerializeField] BoxCollider2D[] boxes;
    public bool ShawGizmos;
    private void OnDrawGizmos() {
        if(!ShawGizmos) return;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Color color = Color.grey;
        color.a = 0.5f;
        Gizmos.color = color;
        if(boxes != null) foreach(var box in boxes){
        Gizmos.DrawCube(box.offset, box.size);
        }
    }    
}
#endif