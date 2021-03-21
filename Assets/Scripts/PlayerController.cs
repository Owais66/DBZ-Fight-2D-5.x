using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region UnityActions for Buttons
    public UnityAction onPunchClick, onKickClick, onPowerClick, onChargeDown, onChargeUp;
    #endregion
    [SerializeField] Button Punch_Button, Kick_Button, Charge_Button, Power_Button;
    bool ChargeKey;
    [SerializeField] FixedJoystick joystick;
    ControlsManager CM;
    SpriteRenderer sprite;
    private void Start()
    {
        CM = GetComponent<ControlsManager>();
        onPunchClick += () => CM.onFight();
        onKickClick += () => CM.onFight2();
        onPowerClick += () => { CM.onPower();};

        Punch_Button.onClick.AddListener(onPunchClick);
        Kick_Button.onClick.AddListener(onKickClick);
        Power_Button.onClick.AddListener(onPowerClick);

        SetButtonEvent(Charge_Button.gameObject, EventTriggerType.PointerDown, (e) => { CM.ManaChargeStart(); });
        SetButtonEvent(Charge_Button.gameObject, EventTriggerType.PointerUp, (e) => { CM.ManaChargeStop(); });

        // Set UI EventTrigger
        void SetButtonEvent(GameObject Button, EventTriggerType eventType, UnityAction<BaseEventData> listener)
        {
            EventTrigger trigger = Button.GetComponent<EventTrigger>();
            var e = new EventTrigger.Entry();
            e.eventID = eventType;
            e.callback.AddListener(listener);
            trigger.triggers.Add(e);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        DesktopControls();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        MobileCont();
#endif
        CM.IsCrouched(DN_Key);
        if (UP_Key) CM.Jump();
    }
    private void FixedUpdate()
    {
        Run();
    }
    void Run()
    {
        if (RT_Key)
        {
            CM.Run(1);
        }
        else if (LT_Key) { CM.Run(-1); }
        else CM.Run(0);
    }
    // Inputs
    #region Button Names Consts
    public const string SQ_Key = "S";
    public const string TR_Key = "T";
    public const string O_Key = "O";
    public const string X_Key = "X";
    #endregion

    #region Keys bool 
    public bool RT_Key;
    public bool LT_Key;
    public bool UP_Key;
    public bool DN_Key;

    #endregion
    /// <summary>
    /// is Used  for Desktop Controls; Controls DirectionKeys and Buttons
    /// </summary>
    void DesktopControls()
    {
        RT_Key = Input.GetKey(KeyCode.D);
        LT_Key = Input.GetKey(KeyCode.A);
        UP_Key = Input.GetKey(KeyCode.W);
        DN_Key = Input.GetKey(KeyCode.S);

        #region  Combo Keys

        //Mana Charge
        if (Input.GetButtonDown(O_Key)) CM.ManaChargeStart();
        if (Input.GetButtonUp(O_Key)) CM.ManaChargeStop();

        //Fight
        if (Input.GetButtonDown(SQ_Key)) CM.onFight();
        if (Input.GetButtonDown(TR_Key)) CM.onFight2();
        #endregion
    }
    #region Mobile
    //#if UNITY_ANDROID


    /// <summary>
    /// Is used  for Mobile Joystick
    /// </summary>
    void MobileCont()
    {
        RT_Key = joystick.Direction.x == 1;
        LT_Key = joystick.Direction.x == -1;
        UP_Key = joystick.Direction.y == 1;
        DN_Key = joystick.Direction.y == -1;

        // Combo Keys in start function
    }
    public void PauseBtns(bool Resume)
    {

        // Punch_Button.GetComponent<Image>().enabled = Resume;
        // Kick_Button.GetComponent<Image>().enabled = Resume;
        // Charge_Button.GetComponent<Image>().enabled = Resume;
        // Power_Button.GetComponent<Image>().enabled = Resume;
        // // joystick.GetComponent<Image>().enabled = Resume;
    }
    #endregion

}
