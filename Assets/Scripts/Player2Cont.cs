using UnityEngine;

public class Player2Cont : MonoBehaviour
{   
    ControlsManager CM;
    private void Start() {
        CM = GetComponent<ControlsManager>();
    }
    void Update()
    {   
        Controls();
        CM.IsCrouched(DN_Key);
        if (UP_Key) CM.Jump(); 
    }
    public bool RT_Key;
    public bool LT_Key;
    public bool UP_Key;
    public bool DN_Key;
     private void FixedUpdate() {
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
    void Controls()
    {
        RT_Key = Input.GetKey("[6]");
        LT_Key = Input.GetKey("[4]");
        UP_Key = Input.GetKey("[8]");
        DN_Key = Input.GetKey("[5]");
    }
}
