//BattleManager
//Singleton and persistent object to manage game state
//For high level control over game
//--------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
public class BattleManager : MonoBehaviour
{

    #region consts
    const string PlayerTag = "Player";
    const string EnemyTag = "Enemy";
    #endregion
    #region Events
    public delegate void PlayerDataDel(PlayerData data);
    public event PlayerDataDel OnHBTEvent;
    public event PlayerDataDel OnHitEvent;
    public event PlayerDataDel OnSpecialHitEvent;
    public event PlayerDataDel OnHitRecEvent;
    public event PlayerDataDel OnSpecialHitRecEvent;
    public event PlayerDataDel OnBlockEvent;
    #endregion

    public static BattleManager Instance;

    #region GameObjects
    public GameObject Player;
    public GameObject Enemy;
    #endregion
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {   
        StartTimer();

        PauseBtn.onClick.AddListener(() => { if (Paused) ResumeGame(); else PauseGame(); });

        #region Score
        OnHitEvent += (PlayerData data) => Score += (int)data.hitBox.Damage * 15;
        #endregion
    }
    #region PauseGame & Resume & Restart & Exit
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Button PauseBtn;
    public bool Paused = false;
    void PauseGame()
    {
        if (Paused) return;
        Paused = true;
        Time.timeScale = 0;
        Player.GetComponent<PlayerController>().PauseBtns(false);
        Player.GetComponent<PlayerController>().enabled = false;
        PauseBtn.GetComponent<Image>().enabled = false;

        PauseMenu.SetActive(true);
    }
    public void ResumeGame()
    {
        if (!Paused) return;
        Paused = false;
        Time.timeScale = 1;
        Player.GetComponent<PlayerController>().enabled = true;
        Player.GetComponent<PlayerController>().PauseBtns(true);
        PauseBtn.GetComponent<Image>().enabled = true;

        PauseMenu.SetActive(false);
    }
    public void RestartMatch()
    {

    }
    public void ExitMatch()
    {

    }
    #endregion
    #region Timer & Score
    /// <summary>
    /// Is Controlled by OnHitEvent in Start;
    /// </summary>
    public int Score = 10000;
    float StartTime;
    public float EndTime { private set; get; }
    void StartTimer()
    {
        StartTime = Time.timeSinceLevelLoad;
    }
    void EndTimer()
    {
        EndTime = Time.timeSinceLevelLoad - StartTime;
    }
    #endregion
    #region Onhits and OnHitRecs
    /// <summary>
    /// Raises an Event OnHitEvent in from Gamemanager, It's used to tell The the Opponent that its hit and Do What  it wants to do
    /// </summary>
    public void onHitEvent(PlayerData data)
    {
        if (OnHitEvent != null) OnHitEvent(data);
    }
    public void onSpecialHitEvent(PlayerData data)
    {
        if (OnSpecialHitEvent != null) OnSpecialHitEvent(data);
    }

    public void onHitRecEvent(PlayerData data)
    {
        if (OnHitRecEvent != null) OnHitRecEvent(data);
    }
    public void onSpecialHitRecEvent(PlayerData data)
    {
        if (OnSpecialHitRecEvent != null) OnSpecialHitRecEvent(data);
    }
    public void onBlockEvent(PlayerData data)
    {
        if (OnBlockEvent != null) OnBlockEvent(data);
    }
    public void OnHurtBoxEvent(PlayerData data)
    {
        if (OnHBTEvent != null) OnHBTEvent(data);
    }
    #endregion
    /// <summary>
    /// is Trigered automatically by ControlsManager
    /// </summary>
    /// <param name="Tagname"></param>
    public GameObject WinnerObj, LoserObj;
    [SerializeField] ScoreBoard scoreBoard;
    public void OnPlayerDead(GameObject Obj)
    {   
        EndTimer();
        
        switch(Obj.tag){
            case "Player": LoserObj = Obj; WinnerObj = Enemy ;break;
            case "Enemy": LoserObj = Obj; WinnerObj = Player; break;
        }
        StartCoroutine(MatchFinishedCore());
    }
    public bool LoserDeadAnimComp;
    public bool WinnerCelebAnimComp;
    IEnumerator MatchFinishedCore(){
        yield return null;
        Time.timeScale = 0.5f;

        WinnerObj.GetComponent<Animator>().enabled = false;
        Player.GetComponent<PlayerController>().enabled = false;
        Enemy.GetComponent<EnemyController>().enabled = false;
        yield return new WaitUntil(()=>LoserDeadAnimComp);
        Time.timeScale = 1;
        WinnerObj.GetComponent<Animator>().enabled =  true;
        WinnerObj.GetComponent<ControlsManager>().AnimCeleb();
        yield return new WaitUntil(()=>WinnerCelebAnimComp);
        scoreBoard.ShowScoreBoard();
    }
}

public struct PlayerData
{
    public GameObject ThisPlayer, Target;
    public HBT hBT;
    public _HitBox hitBox;
}