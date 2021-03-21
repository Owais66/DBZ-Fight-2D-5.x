using UnityEngine;
using System.Collections;


public class KiBlast : MonoBehaviour {
    Transform trans;
    [SerializeField] float velocity;
    [SerializeField] float LifeSpan;
    Animator Anim;
    public ControlsManager PlayerCM;
    private void Start() {
        trans = GetComponent<Transform>();
        Anim = GetComponent<Animator>();
        StartCoroutine(lifeSpan());
    }
    private void FixedUpdate() {
        move();
    }
    void move(){
        transform.Translate(Vector3.right * velocity);
    }
    [SerializeField]Vector3 off;
    void Explode(){
        _HitBox Hb = new _HitBox();
        Hb.Damage = 5;
        Hb.StandHRID = 1;
        Hb.Effect = "Explosion";
        Hb.Hitbox = new Boxes(off, Vector3.zero);
        PlayerCM.OnHit(Hb);
        Destroy(gameObject);
    }
    IEnumerator lifeSpan(){
        float time = 0;
        float Fixeddelta = Time.fixedDeltaTime;
        while(time < LifeSpan){
            time += Fixeddelta;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject,0.2f);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == PlayerCM.Target.tag){
            Explode();
        }
    }
}