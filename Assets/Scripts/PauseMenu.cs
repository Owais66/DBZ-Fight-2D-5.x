using UnityEngine;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{   
    [SerializeField] Button ResumeBtn;
    void Start()
    {
        ResumeBtn.onClick.AddListener(()=>{BattleManager.Instance.ResumeGame(); gameObject.SetActive(false);});
    }
}
