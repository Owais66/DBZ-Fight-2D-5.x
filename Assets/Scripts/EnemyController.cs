using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] bool AIActive;
    #region Componenets
    ControlsManager CM;
    ControlsManager TargetCM;
    #endregion
    public HBT TargetHBT;
    AIDataBase AIDB;
    _AIData AIData;
    private void Start()
    {

        AIDB = (AIDataBase)Resources.Load("EnemyDB");
        CM = GetComponent<ControlsManager>();
        TargetCM = CM.TargetCM;

        BattleManager.Instance.OnHBTEvent += (PlayerData data) => { if (data.ThisPlayer.tag != tag){TargetHBT = data.hBT; UpdateAI(TargetHBT);}};
    }
    private void FixedUpdate()
    {   if(!AIActive) return;
        if (TargetCM == null || AIData == null || TargetHBT == null || EnemyHBTChange == null) return;
         AICont();
    }
    private void Update() {
        if(!AIActive) return;
        if (TargetCM == null || AIData == null || TargetHBT == null || EnemyHBTChange == null) return;
        
        CM.IsCrouched(Crouch);
        
        if ((AITimer > 0 && AIRangeCheck(AIData) && EnemyHBTChange.CMS == TargetHBT.CMS) || AICoreBool) { AITimer -= Time.deltaTime; return; }
        UpdateAI(TargetHBT);
    }
    #region AI
    float AITimer = 0;
    bool Crouch;
    /// <summary>
    /// Do not use. Just For AICont() and AI() Coroutine
    /// </summary>
    HBT EnemyHBTChange;
    float Direction;
    void AICont()
    {
        CM.Run(transform.right.x * Direction);
        // CM.IsCrouched(Crouch);
        // if ((AITimer > 0 && AIRangeCheck(AIData) && EnemyHBTChange.CMS == TargetHBT.CMS) || AICoreBool) { AITimer -= Time.fixedDeltaTime; return; }
        // UpdateAI(TargetHBT);
    }
    void UpdateAI(HBT hBT)
    {
        _AIDataArray DatasArray = AIDB.AIdataDict[hBT.CMS];
        List<_AIData> FilteredData = new List<_AIData>();
        if (DatasArray.AIDatas.Length != 0)
        {
            FilteredData = Filter(DatasArray.AIDatas);
            if (FilteredData != null && FilteredData.Count != 0) AIData = FilteredData[Random.Range(0, FilteredData.Count)];
        }
        _AIData[] NoneData = AIDB.AIdataDict[State.None].AIDatas;
        if (FilteredData == null || FilteredData.Count == 0)
        {
            FilteredData = Filter(NoneData);
            if (FilteredData != null && FilteredData.Count != 0) AIData = FilteredData[Random.Range(0, FilteredData.Count)];
        }
        if (FilteredData == null || FilteredData.Count == 0)
        {
            AIData = NoneData[Random.Range(0, NoneData.Length)];
        }
        AITimer = Random.Range(AIData.MinDuration, AIData.MaxDuration);
        StartCoroutine(AI());


        List<_AIData> Filter(_AIData[] ThisDatas)
        {

            if (ThisDatas.Length == 0) return null;
            List<_AIData> ThisFilteredData = new List<_AIData>();

            //GroundCheck & RangeCheck
            foreach (var data in ThisDatas)
            {
                if (CM.Grounded == data.Grounded && AIRangeCheck(data)) ThisFilteredData.Add(data);
            }
            if (ThisFilteredData == null || ThisFilteredData.Count == 0) return null;

            //Probablity Check & Mana Check
            float Mana = CM.Mana;
            int Probablity = DatasArray.GetMinProbablity(ThisFilteredData);

            for (int i = ThisFilteredData.Count - 1; i >= 0; i--)
            {
                if (ThisFilteredData[i].Probablity < Probablity || ThisFilteredData[i].MinMana > Mana)
                    ThisFilteredData.RemoveAt(i);
            }
            if (ThisFilteredData.Count == 0 || ThisFilteredData == null) return null;
            return ThisFilteredData;
        }
    }
    bool AICoreBool;
    /// <summary>
    /// is Called By onEnemyHBT which is called whenever Targets HBType Changes;
    /// </summary>
    /// <param name="aiData"></param>
    IEnumerator AI()
    {
        if (!AICoreBool)
        {
            AICoreBool = true;
            yield return new WaitForSeconds(0.25f);
            EnemyHBTChange = TargetHBT;

            CM.ManaChargeStop();

            switch (AIData.AIState)
            {
                case State.Crouching: CM.IsCrouched(true); Crouch = true; break;
                case State.CrouchBlock: CM.IsCrouched(true); Crouch = true; break;
                case State.CrouchFight: CM.IsCrouched(true); Crouch = true; yield return new WaitForSeconds(0.2f); Fight(); break;
                case State.Jump: CM.IsCrouched(false); Crouch = false; CM.HBType.canJump = true; CM.Jump(); break;
                case State.inAirFight: Crouch = false; StartCoroutine(InAirFight(2f)); break;
                case State.StandSpecial: Crouch = false; StartCoroutine(Power1Core()); break;
                case State.StandBlock: Crouch = false; break;
                case State.StandFight: Crouch = false; yield return new WaitForSeconds(0.1f); Fight(); break;
                case State.Standing: Crouch = false; Standing(); break;
                case State.InsTrans: CM.InstantTrans(); break;
                case State.KiBlast: CM.onKiBlast(); break;
                case State.KiCharge: CM.ManaChargeStart(); break;
            }
            Direction = AIData.Direction;
            AICoreBool = false;
        }
    }
    /// <summary>
    /// Returns True if Player is  within Range
    /// </summary>
    /// <returns></returns>
    bool AIRangeCheck(_AIData data)
    {
        if (data.MinRange == 0 || !CM.RangeCheck(TargetCM.CenterPos - CM.CenterPos, data.MinRange, TargetCM.name))
        {
            if (data.MaxRange == 0 || CM.RangeCheck(TargetCM.CenterPos - CM.CenterPos, data.MaxRange, TargetCM.name)) return true;
        }
        return false;
    }
    #endregion

    #region Moves
    void Standing()
    {
        CM.IsCrouched(false);
    }
    void Fight()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            CM.onFight();
            return;
        }
        else CM.onFight2();
    }
    bool inAirFights;
    IEnumerator InAirFight(float maxtime)
    {
        if (!inAirFights)
        {
            inAirFights = true;
            //float timecount = maxtime;
            if (CM.Grounded) { CM.Jump(); }

            yield return new WaitForSeconds(0.5f);
            Fight();
            //timecount -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            inAirFights = false;
        }
    }
    IEnumerator Power1Core(){
        Debug.Log("PowerOn");
        CM.ManaChargeStart();
        yield return new WaitForSeconds(0.2f);
        CM.onFight2();
        yield return new WaitForSeconds(0.2f);
        CM.ManaChargeStop();
    }
    #endregion


#if UNITY_EDITOR
    #region Gizmos
    [SerializeField] bool ShowGizmos;
    [SerializeField] float Radius = 0;
    private void OnDrawGizmos()
    {
        Vector3 CenterPos = GetComponent<BoxCollider2D>().offset;
        if (!ShowGizmos) return;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CenterPos, Radius);

        if (AIData != null && (AIData.MaxRange != 0 || AIData.MinRange != 0))
        {
            Gizmos.DrawWireSphere(CenterPos, AIData.MinRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(CenterPos, AIData.MaxRange);
        }
    }
    #endregion
#endif
}


