using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

//[RequireComponent (typeof(BoxController))]
[RequireComponent(typeof(Rigidbody2D))]
public class ControlsManager : MonoBehaviour
{
    #region Events
    public delegate void OnHBTDel(HBT hbt);
    #endregion

    #region Components
    [HideInInspector] public Transform Trans;
    [HideInInspector] public Rigidbody2D RB;
    #endregion

    #region HBTs and Target
    [Header("General Components")]
    public HBT HBType;
    /// <summary>
    /// is Updated autmatically by gamemanager events
    /// </summary>
    public HBT EnemyHBType;
    public GameObject Target;
    public ControlsManager TargetCM;
    #endregion

    #region Playerstats
    public float Health = 100, Mana = 0;
    const float RunSpeed = 10;
    const float JumpForce = 60;
    const float AuraChargeSpeed = 0.2f;
    const float SpecialMana = 35;
    const float KiBlastMana = 15;
    const float InstTransMana = 8;
    #endregion
    private void Start()
    {
        CMStart();
        AnimStart();
        BoxStart();

    }
    private void Update()
    {
        SetCenterPos();
        if(Input.GetKeyDown(KeyCode.N) && tag=="Player") InstantTrans();
    }
    void CMStart()
    {
        Trans = GetComponent<Transform>();
        RB = GetComponent<Rigidbody2D>();

        CenterPos = Vector3.zero;
        CameraController.Flip += RotateOBJ;

        BattleManager.Instance.OnHitEvent += OnHitRec;
        BattleManager.Instance.OnHBTEvent += (PlayerData data) => { if (data.ThisPlayer.tag != tag) EnemyHBType = data.hBT; };
    }
    private void OnDestroy() {
        CameraController.Flip -= RotateOBJ;
    }
    #region Flip, Rotation and PosCenter For CameraController
    [HideInInspector] public Vector3 CenterPos;
    float flipangle;
    bool flipActive;
    void RotateOBJ(bool side){
         if ((TargetCM.CenterPos - CenterPos).x >= 0) { if (!flipActive) StartCoroutine(RotateOBJCore()); flipangle = 0; }
            else { if (!flipActive) StartCoroutine(RotateOBJCore()); flipangle = 180; }
    }
    IEnumerator RotateOBJCore()
    {
        flipActive = true;
        yield return new WaitForSeconds(0.1f);

        yield return new WaitUntil(() => (!HBType.CheckFighting() && HBType.CMS != State.HitRec));
        yield return new WaitForFixedUpdate();
        if (transform.rotation.y != flipangle)
        {
            Vector3 IPos = transform.position;
            ParentAnim.transform.rotation = Quaternion.Euler(0, flipangle, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.position = IPos;
            transform.position -= transform.right * 4;
        }
        flipActive = false;
    }
    void SetCenterPos()
    {
        Vector3 Offset = new Vector3(Hurtbox.GrabBox.Offset.x * transform.right.x, Hurtbox.GrabBox.Offset.y);
        CenterPos = transform.position + Offset;
    }
    #endregion
    #region RunAndBlock
    /// <summary>
    /// Determines the Direction of movement in refence to the side the player is Facing
    /// </summary>
    [HideInInspector] public float direction;
    public void Run(float dir)
    {
        direction = dir * transform.right.x;
        animator.SetInteger("Direction", (int)direction);
        if (!HBType.canRun || dir == 0) return;

        Vector2 vel = RB.velocity;
        vel.x = dir * RunSpeed;
        RB.velocity = vel;
    }
    #endregion

    #region Jump
    public void Jump()
    {
        if (!jump_Coroutine && Grounded && HBType.canJump)
        {
            HBType.CMS = State.Jump; HBType.canJump = false;
            animator.SetTrigger("Jump");
            StartCoroutine(Jump_Coroutine());
        }
    }
    private bool jump_Coroutine = false;
    IEnumerator Jump_Coroutine()
    {
        jump_Coroutine = true;
        RB.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        jump_Coroutine = false;
    }
    #endregion

    #region Cornered
    public bool CheckCornered()
    {
        return RangeCheck(transform.right * -1, 4, "Border");
    }
    #endregion

    #region Crouch
    public void IsCrouched(bool cond)
    {
        if (cond && HBType.CMS2 != State2.Crouching) animator.SetBool("Crouch", true);
        else if (!cond && HBType.CMS2 == State2.Crouching) animator.SetBool("Crouch", false);
    }
    #endregion
    #region Combos
    [Space(-1)]
    [Header("Combos")]
    [SerializeField] Vector3 KiOff;
    [SerializeField] GameObject KiBlastPrefab;
    #region ComboBtns
    public void onFight()
    {
        if (HBType.canFight)
        {
            HBType.canJump = false;
            HBType.canBlock = false;
            HBType.canRun = false;
            animator.SetTrigger("Fight");
        }

    }
    public void onFight2()
    {
        if (ManaChargeTrig)
        {
            onPower();
            ManaChargeStop();
            return;
        }

        if (HBType.canFight)
        {
            HBType.canJump = false;
            HBType.canBlock = false;
            HBType.canRun = false;
            animator.SetTrigger("Fight2");
        }
    }
    public void onPower()
    {
        if (HBType.CMS == State.KiCharge && Grounded && Mana > SpecialMana)
        {
            SetMana(Mana - SpecialMana);
            HBType.CMS = State.StandSpecial;
            HBType.canJump = false;
            HBType.canFight = false;
            HBType.canRun = false;
            animator.SetTrigger("Power");
            SpecialFX.SetTrigger("Kamehame");
        }
        else if(Blocking) InstantTrans();
        else onKiBlast();
    }
    #endregion
    public void onKiBlast()
    {
        if (HBType.canFight && (HBType.CMS == State.Standing || HBType.CMS == State.KiBlast) && Grounded && Mana > KiBlastMana)
        {

            SetMana(Mana - KiBlastMana);
            HBType.CMS = State.KiBlast;
            HBType.canJump = false;
            HBType.canRun = false;
            HBType.canCrouch = false;
            animator.SetTrigger("KiBlast");
        }
    }
    /// <summary>
    /// KiBlast Event for Animation
    /// </summary>
    public void KiBlastEve()
    {
        GameObject KiBlastObj = Instantiate(KiBlastPrefab,
        new Vector3(4.77f * transform.right.x, 3.88f) + transform.position, Quaternion.identity);

        KiBlastObj.transform.rotation = transform.rotation;
        KiBlast KiBlastCont = KiBlastObj.GetComponent<KiBlast>();
        KiBlastCont.PlayerCM = this;
    }
    [SerializeField] Vector3 InsTransOff;
    public void InstantTrans()
    {
        animator.SetTrigger("InsTrans");
        SetMana(Mana - InstTransMana);
    }
    public void InstantTransEve()
    {
        transform.position = TargetCM.CenterPos + InsTransOff;
    }
    #endregion
    #region OnHit

    /// <summary>
    /// Is Triggered Automatically by BoxController when a this Player Hits
    /// </summary>
    public void OnHit(_HitBox HB)
    {
        if(HB.StandHRID != 6)StartCoroutine(StunAnim(StunTime));
        PlayerData data = new PlayerData();
        data = GetData();
        data.hitBox = HB;
        BattleManager.Instance.onHitEvent(data);
        if (HB.Effect != "Explosion" && Target.GetComponent<ControlsManager>().CheckCornered() && HBType.CMS != State.StandSpecial)
        {
            ParentAnim.SetTrigger("MoveBack");
        }
    }
    [SerializeField] float StunTime;
    /// <summary>
    /// is Triggered  automatically by BattleManager
    /// </summary>
    /// <param name="HB">HitBox data of the person who hit</param>
    public void OnHitRec(PlayerData data)
    {// Code for the player who recived Hit
        //Damage Player
        if (data.ThisPlayer.tag == tag) return;
        if (Blocking)
        {
            OnHitBlock(); BattleManager.Instance.onBlockEvent(GetData());
            SpecialFXCont(SPFXState.Block);
            return;
        }
        SetHealth(Health - data.hitBox.Damage);
        if (Health <= 0) { animator.SetBool("PlayerDead", true); ParentAnim.SetTrigger("PlayerDead"); return; }

        HBType.canRun = false; HBType.canJump = false; HBType.canFight = false; HBType.canBlock = false;

        AnimHitRec(data);

        BattleManager.Instance.onHitRecEvent(GetData());
    }
    #endregion
    #region HP Mana Aura
    [Header("UI Elements")]
    [SerializeField] Image RedHealth;
    [SerializeField] Image HealthBar;
    [SerializeField] Image ManaBar;
    /// <summary>
    /// Sets Health and Triggers BattleManager.OnPlayerDead() when Health is zero;
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(float health)
    {
        Health = health;
        if (Health <= 0) { BattleManager.Instance.OnPlayerDead(gameObject);return; }

        CoreHealth = health / 100;
        if (!HealthCoreActive) StartCoroutine(HealthCore(1));

    }
    [Range(0, 1)] float CoreHealth;
    bool HealthCoreActive;
    IEnumerator HealthCore(float RedHealthTime)
    {
        HealthCoreActive = true;
        while (HealthBar.fillAmount >= CoreHealth)
        {
            HealthBar.fillAmount -= Time.deltaTime;
            yield return null;
        }
        HealthCoreActive = false;
        if (!RedAnimCoreActive) StartCoroutine(RedAnimCore(RedHealthTime));
    }
    bool RedAnimCoreActive;
    IEnumerator RedAnimCore(float RedHealthTime)
    {
        RedAnimCoreActive = true;
        yield return new WaitForSeconds(RedHealthTime);
        while (RedHealth.fillAmount >= Health / 100)
        {
            if (RedHealth.fillAmount == 0) break;
            RedHealth.fillAmount -= Time.deltaTime;
            if (HealthCoreActive) yield return new WaitUntil(() => !HealthCoreActive);
            yield return null;
        }
        RedAnimCoreActive = false;
    }
    #region ManaControls
    public void SetMana(float mana)
    {
        ManaBar.fillAmount = mana / 100;
        Mana = mana;
    }
    bool ManaChargeTrig = false;
    public void ManaChargeStart()
    {
        ManaChargeTrig = true;
        if (Mana < 100 && HBType.CMS == State.Standing)
        {
            StartCoroutine(ManaChargeUp());
            animator.SetInteger("KICharge", (int)KiCharge.charging);
        }
    }
    public void ManaChargeStop()
    {
        ManaChargeTrig = false;
        AuraCharge(false);
        animator.SetInteger("KICharge", (int)KiCharge.stop);
    }
    IEnumerator ManaChargeUp()
    {
        yield return new WaitForSeconds(0.4f);
        AuraCharge(true);
        while (ManaChargeTrig)
        {
            ManaBar.fillAmount += AuraChargeSpeed * Time.deltaTime;
            Mana = ManaBar.fillAmount * 100;
            if (Mana >= 100) { AuraCharge(false); animator.SetInteger("KICharge", (int)KiCharge.full); break; }
            yield return null;
        }
        AuraCharge(false);
    }
    #endregion
    #endregion
    #region HoldPos
    public void HoldPosition(float time)
    {
        StartCoroutine(HoldPosCore(animator, time));
    }
    public void HoldPosition(Animator anim, float time)
    {
        StartCoroutine(HoldPosCore(anim, time));
    }
    IEnumerator HoldPosCore(Animator anim, float time)
    {
        Animator animator = anim;
        yield return new WaitForFixedUpdate();
        RB.velocity = Vector2.zero;
        RB.gravityScale = 0;
        animator.enabled = false;
        yield return new WaitForSeconds(time);
        animator.enabled = true;
        RB.gravityScale = 7;
    }
    #endregion
    #region StunAnim
    IEnumerator StunAnim(float time, Func<bool> func)
    {
        animator.enabled = false;
        yield return new WaitForSeconds(time);
        yield return new WaitUntil(func);
        animator.enabled = true;
    }
    IEnumerator StunAnim(float time)
    {
        animator.enabled = false;
        yield return new WaitForSeconds(time);
        animator.enabled = true;
    }
    #endregion
    // #endregion

    #region Block
    /// <summary>
    /// Do not Use. its for BlockCorroutine
    /// </summary>
    bool BlockActive;
    /// <summary>
    /// Returns True if Player is Blocking
    /// </summary>
    [HideInInspector] public bool Blocking;
    /// <summary>
    /// Is Controlled by BoxController (ReadOnly)
    /// </summary>
    bool BlockTrig;
    public void BlockStart()
    {
        if (!BlockActive) StartCoroutine(BlockCore());
    }
    IEnumerator BlockCore()
    {
        if (!BlockActive)
        {
            BlockActive = true;
            while (BlockTrig)
            {
                bool cond = ((direction == -1 && HBType.CMS2 == State2.Standing) && EnemyHBType.CMS2 != State2.Crouching) || (HBType.CMS2 == State2.Crouching && EnemyHBType.CMS2 != State2.Jumping);
                animator.SetBool("Block", cond);
                HBType.canRun = !cond;
                HBType.canJump = !cond;
                HBType.canFight = !cond;
                HBType.canCrouch = true;
                Blocking = cond;
                yield return new WaitForSeconds(0.5f);
            }
            animator.SetBool("Block", false);
            BlockActive = false;
            Blocking = false;
        }
    }
    void OnHitBlock()
    {
        Debug.Log("Blocked");
    }
    #endregion
    #region HurtBox
    /// <summary>
    /// is Triggered By BoxController HurtBox Which is Triggered by animation event, It is  Also Triggered within this Script
    /// This also raises an Event OnHBT
    /// </summary>
    public void OnHurtBox(HBT hbType)
    {
        HBType = HBT.NewHBT(hbType);
        PlayerData Data = new PlayerData();
        Data = GetData();
        BattleManager.Instance.OnHurtBoxEvent(Data); // used to update EnemyHBT in Opponent Controls Manager;
    }
    /// <summary>
    /// RayCastes to Check if it Collides
    /// </summary>
    /// <param name="dir">Direction Relative to WorldSpace</param>
    /// <param name="dist">Dist Relative To CenterPos of this Object</param>
    /// <param name="FindName">Look for This name in gameObject</param>
    /// <returns></returns>
    public bool RangeCheck(Vector2 dir, float dist, string FindName)
    {
        RaycastHit2D[] rays = Physics2D.RaycastAll(CenterPos, dir, dist);
        foreach (var ray in rays)
        {
            if (FindName == ray.transform.name) return true;
        }
        return false;
    }
    #endregion

    #region Animations
    [HideInInspector] public Animator animator, ParentAnim;
    Animator SpecialFX;
    Animator AuraFX;
    UnityEngine.Object Obj_KiBlast;
    void AnimStart()
    {
        animator = GetComponent<Animator>();
        ParentAnim = transform.parent.GetComponent<Animator>();

        SpecialFX = transform.Find("SpecialFX").GetComponent<Animator>();
        AuraFX = transform.Find("AuraFX").GetComponent<Animator>();

        Obj_KiBlast = Resources.Load("KiBlast");
    }
    #region HitRec
    public void AnimHitRec(PlayerData data)
    {
        if (data.hitBox.StandHRID == 6)
        {  
            
            AnimSetHitRec(6);
            StunAnim(0,new Func<bool>(()=>Grounded));
            return;
        }
        switch (HBType.CMS2)
        {
            case State2.Standing: AnimSetHitRec(data.hitBox.StandHRID); return;
            case State2.Crouching: AnimSetHitRec(data.hitBox.CrouchHRID); return;
            case State2.Jumping: AnimSetHitRec(data.hitBox.InAirHRID); return;
        }
        void AnimSetHitRec(int ID)
        {
            animator.SetInteger("HitRec", ID);
            animator.SetTrigger("HitRecTrig");

            ParentAnim.SetInteger("Eff", ID);
            ParentAnim.SetTrigger("Trig");
        }
    }
    // bool SpecialHitRecTrig;
    // bool LastSpecialHit;
    // IEnumerator AnimSpecialHitRec()
    // {
    //     animator.SetBool("SpecialHitRec", true);
    //     SpecialHitRecTrig = true;
    //     AnimSetHitRec(6);
    //     Vector3 Dir = ((transform.right.x == 1) ? new Vector3(-1, 1) : new Vector3(1, 1));
    //     float WaitTill = 0.5f;
    //     float timer = 0;
    //     while (LastSpecialHit || timer < WaitTill)
    //     {
    //         if (LastSpecialHit)
    //         {
    //             timer = 0;
    //             RB.AddForce(Dir * 1700);
    //             LastSpecialHit = false;
    //         }
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //     AnimSetHitRec(7);
    //     yield return new WaitForSeconds(0.2f);
    //     SpecialHitRecTrig = false;
    //     animator.SetBool("SpecialHitRec", false);
    //     yield return null;
    // }
    #endregion
    public enum KiCharge { charging, stop, full }
    public void AuraCharge(bool cond)
    {
        AuraFX.SetBool("Charge", cond);
    }
    /// <summary>
    /// Is Triggerd By BattleManager
    /// </summary>
    public void AnimCeleb()
    {
        animator.SetBool("Celebrate", true);
    }
    #region AnimationEvents
    public void MoveTowardsTarget()
    {
        StartCoroutine(MoveTowardsTargetCore());
    }
    IEnumerator MoveTowardsTargetCore()
    {
        float timecount = 0.2f;
        Vector2 dir = TargetCM.CenterPos - CenterPos;

        dir.y = Mathf.Clamp(dir.y, -100, -0.01f);
        //if (dir.y < -0.1f && Mathf.Abs(dir.x)> 0.25f )
        while (timecount > 0 && !Grounded)
        {
            RB.AddForce(dir * 300);
            timecount -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    public void OnAnimFinishedEve(string name)
    {
        switch (name)
        {
            case "DeadFin": BattleManager.Instance.LoserDeadAnimComp = true; break;
            case "CelebFin": BattleManager.Instance.WinnerCelebAnimComp = true; StartCoroutine(DelayCode(()=>animator.enabled = false, 0.25f)) ;break;
        }
        animator.enabled = false;
        ParentAnim.enabled = false;
    }
    IEnumerator DelayCode(Action act, float sec){
        yield return new WaitForSeconds(sec);
        act();
    }
    public void GravityEve(float gravity)
    {
        RB.gravityScale = gravity;
    }
    #endregion
    #region SpecialFX
    enum SPFXState{Empty,Hit,Explosion, SpAtk, Block, SpAtkHitRec}
    void SpecialFXCont(SPFXState state){
        SpecialFX.SetInteger("Spec", (int)state);
        SpecialFX.SetTrigger("SpecTrig");
    }
    #endregion
    #endregion

    #region BoxController
    #region Serialized Objects
    [Space(-1)]
    [Header("BoxController")]
    [SerializeField] BoxCollider2D GrabBox;
    [SerializeField] BoxCollider2D HeadHurtBox;
    [SerializeField] BoxCollider2D AbsHurtBox;
    [SerializeField] BoxCollider2D LegsHurtBox;
    [SerializeField] BoxCollider2D BlockBoxCollider;
    #endregion
    [HideInInspector] public DataBase DB;
    BoxGizmos boxGizmos;
    private void BoxStart()
    {
        DB = (DataBase)Resources.Load(gameObject.name + "DB");
        boxGizmos = GetComponent<BoxGizmos>();
    }

    #region HitBox
    _HitBox HB;
    Vector3 HitBoxOffset;
    Vector3 HitBoxSize;
    public void HitboxEve(string name)
    {
#if UNITY_EDITOR
        if (!DB.HitBoxes.ContainsKey(name)) { Debug.LogError(name + " HitBox notfound"); return; }
#endif
        HB = DB.HitBoxes[name];
        if (HB == null) return;
        HitBoxSize = HB.Hitbox.Size;
        HitBoxOffset = HB.Hitbox.Offset;
        HitBoxOffset.x *= transform.right.x;
        boxGizmos.StartHitCoroutine(HB.Hitbox);
        Collider2D[] HBCol = Physics2D.OverlapBoxAll(transform.position + HitBoxOffset, HB.Hitbox.Size, 0);
        foreach (Collider2D hitbox in HBCol)
        {
            if (hitbox.tag == Target.tag && hitbox.name == "HurtBox")
            {
                OnHit(HB);
                return;
            }
        }
    }
    public void HitRay()
    {

    }
    #endregion
    #region HurtBox
    public _HurtBox Hurtbox;
    /// <summary>
    /// HurtBox Start Event for Animations
    /// </summary>
    /// <param name="name">name of the Hurtbox  to activate</param>
    public void HurtBoxStartEve(string name)
    {
#if UNITY_EDITOR
        if (!DB.HurtBoxes.ContainsKey(name)) { Debug.LogError(name + " HurtBox not Found in " + tag); return; }
#endif
        Hurtbox = DB.HurtBoxes[name];
        OnHurtBox(Hurtbox.HurtBoxType);
        // Head HurtBox
        HeadHurtBox.offset = Hurtbox.Head.Offset;
        HeadHurtBox.size = Hurtbox.Head.Size;
        // Abs HurtBox
        AbsHurtBox.offset = Hurtbox.Abs.Offset;
        AbsHurtBox.size = Hurtbox.Abs.Size;
        // Legs HurtBox
        LegsHurtBox.offset = Hurtbox.Legs.Offset;
        LegsHurtBox.size = Hurtbox.Legs.Size;

        // GrabBox
        GrabBox.offset = Hurtbox.GrabBox.Offset;
        GrabBox.size = Hurtbox.GrabBox.Size;
    }

    #endregion
    #region BlockBoxs
    public void BlockBoxStartEve(string BlockBoxName)
    {
#if UNITY_EDITOR
        if (!DB.BlockBoxes.ContainsKey(BlockBoxName)) { Debug.LogError(BlockBoxName + " BlockBox not Found"); return; }
#endif
        _BlockBox Blockbox = DB.BlockBoxes[BlockBoxName];
        BlockBoxCollider.offset = Blockbox.BlockBox.Offset;
        BlockBoxCollider.size = Blockbox.BlockBox.Size;
        BlockBoxCollider.enabled = true;
    }
    public void BlockBoxEndEve()
    {
        BlockBoxCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Target == null) return;
        if (other.gameObject.tag == Target.tag && other.gameObject.name == "BlockBox")
        {
            BlockTrig = true;
            BlockStart();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (Target == null) return;
        if (other.gameObject.tag == Target.tag && other.gameObject.name == "BlockBox")
        {
            BlockTrig = false;
        }
    }
    #endregion
    [HideInInspector] public bool Grounded;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Ground" && !Grounded) { Grounded = true; animator.SetBool("IsGrounded", true); }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.name == "Ground" && Grounded) { Grounded = false; animator.SetBool("IsGrounded", false); }
    }
    #endregion
    /// <summary>
    /// Sets all Player Data except HitBox;
    /// </summary>
    /// <returns></returns>
    public PlayerData GetData()
    {
        PlayerData data = new PlayerData();
        data.ThisPlayer = gameObject;
        data.Target = Target;
        data.hBT = HBType;
        return data;
    }

}



