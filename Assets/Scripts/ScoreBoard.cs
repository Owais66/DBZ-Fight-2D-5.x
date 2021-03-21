using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class ScoreBoard : MonoBehaviour
{
    [SerializeField] Text HeaderText,ScoreText, TotalTimeText;
    [SerializeField] Image[] Star = new Image[3]; 
    [SerializeField] Button AcptBtn, ReTryBtn;
    public void ShowScoreBoard(){
        foreach(var s in Star) s.enabled = false;
        


        StartCoroutine(SetScoreBoardCore());
    }
    IEnumerator SetScoreBoardCore(){
        
        HeaderText.text = BattleManager.Instance.WinnerObj.tag == "Player"?"Winner":"Loser";
        
        float Score =BattleManager.Instance.Score;
        ScoreText.text = Score.ToString();

        float TimeSec = BattleManager.Instance.EndTime;
        string TimeText = Math.Round(TimeSec/60).ToString()+":"+Math.Round((TimeSec%60)).ToString();
        TotalTimeText.text = TimeText;
        
        transform.localScale = Vector3.zero;

        //ScoreBoard Empasis Transition
        while (transform.localScale.x<1)
        {
            transform.localScale += Vector3.one*0.05f;
            yield return null;
        }
        transform.localScale = Vector3.one;
        
        //Star Animation
        
        yield return new WaitForSeconds(0.2f);
        if(Score>3000) Star[0].enabled = true;
        yield return new WaitForSeconds(0.2f);
        if(Score>6000) Star[1].enabled = true;
        yield return new WaitForSeconds(0.2f);
        if(Score>8500) Star[2].enabled = true;

        //Accept and Retry Button
        AcptBtn.onClick.AddListener(()=>GameManager.Instance.LoadMainMenu(MenuManager.MenuState.MainMenu));
        ReTryBtn.onClick.AddListener(()=>GameManager.Instance.ReloadScene());
    }
}
