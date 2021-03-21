using UnityEngine;
using System.Collections;
public class CameraController : MonoBehaviour
{
    Vector3 Dist;
    #region  Components
    [SerializeField] GameObject Player, Enemy;
    [SerializeField] float YOffSet;
    ControlsManager PlayerCM, EnemyCM;
    Transform CamTrans;
    #endregion
    public delegate void CamDel(bool _flip);
    public static event CamDel Flip;

    private void Start()
    {
        
        SetObjects();

        BattleManager.Instance.OnHitRecEvent += (PlayerData data) => { StartCoroutine(ShakeCamCore(0.2f, 0.5f)); };
        BattleManager.Instance.OnBlockEvent += (PlayerData data) => { StartCoroutine(ShakeCamCore(0.2f, 0.2f)); };
    }
    void SetObjects(){
        CamTrans = GetComponent<Transform>();

        PlayerCM = Player.GetComponent<ControlsManager>();
        EnemyCM = Enemy.GetComponent<ControlsManager>();
    }
    private void LateUpdate()
    {   
        if(PlayerCM==null || EnemyCM==null){SetObjects(); return; }
        SetCamera();
        Flip_Check();
    }
    Vector3 Campos;
    [SerializeField] float smoothtime;
    void SetCamera()
    {
        if (Player == null || Enemy == null || ShakeActive) return;
        Dist = (EnemyCM.CenterPos - PlayerCM.CenterPos) / 2;
        Dist.z = -10;
        Dist.y += YOffSet;
        Campos = PlayerCM.CenterPos + Dist;
        CamTrans.position = Vector3.Lerp(transform.position, Campos, smoothtime);
    }
    bool flip;
    void Flip_Check()
    {
        if (Dist.x < 0 && flip == false) { flip = true; Flip(flip); }
        else if (Dist.x > 0 && flip == true) { flip = false; Flip(flip); }
    }
    bool ShakeActive;
    [SerializeField] float Magnitude;
    [SerializeField] float ShakeTime;
    IEnumerator ShakeCamCore(float ShakeTime, float Magnitude)
    {
        ShakeActive = true;
        float timeCount = ShakeTime;
        Vector3 CurrentPos = transform.position;
        while (timeCount > 0)
        {
            transform.position = CurrentPos + (Vector3)Random.insideUnitCircle * Magnitude;
            timeCount -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ShakeActive = false;
    }
}