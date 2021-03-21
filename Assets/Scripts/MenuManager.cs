using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;

[ExecuteInEditMode]
public class MenuManager : MonoBehaviour
{
    [SerializeField] MainDatabase MDB;
    [Serializable] public enum MenuState { StartMenu, MainMenu, CharSelect, LvlSelect, BattleScreen, Settings, Loading };
    MenuState CurrentMenu;

    private void Start()
    {
        StartMenu.obj.SetActive(false);
        MainMenu.obj.SetActive(false);
        CharSelect.obj.SetActive(false);
        LVLSelect.obj.SetActive(false);
        Settings.obj.SetActive(false);

        LoadMenu(GameManager.Instance.MenuState);
    }
    [SerializeField] Image blackScreen;
    public void LoadMenu(MenuState LoadState){
        StartCoroutine(LoadMenuCore(LoadState));
    }
    IEnumerator LoadMenuCore(MenuState Loadstate){
        blackScreen.color = Color.black;
        blackScreen.gameObject.SetActive(true);
        switch(Loadstate){
            case MenuState.StartMenu: StartMenu.obj.SetActive(true); OnStartMenu(); break;
            case MenuState.MainMenu: MainMenu.obj.SetActive(true); OnMainMenu(); break;
        }
        yield return new WaitForSeconds(0.5f);
        Color alphaCount = blackScreen.color;
        while(alphaCount.a > 0){
            alphaCount.a -= Time.deltaTime;
            blackScreen.color = alphaCount;
            yield return null;
        }
        blackScreen.gameObject.SetActive(false);
    }
    #region StartMenu
    #region InspecterVar
    [Space(15f)]
    [Header("StartMenu Selection")]
    [SerializeField] UIObjects StartMenu;
    [SerializeField] Button InvisibleTouch;
    #endregion
    void OnStartMenu()
    {
        CurrentMenu = MenuState.StartMenu;

        InvisibleTouch.onClick.AddListener(() => { StartCoroutine(FadeTransition(StartMenu, MainMenu, OnMainMenu)); });
    }
    #endregion
    #region MainMenu
    #region InspecterVar
    [Space(15f)]
    [Header("MainMenu Selection")]
    [SerializeField] UIObjects MainMenu;
    [SerializeField] Button ClassicBtn, ArcadeBtn, PvPBtn, SettingsBtn;
    #endregion

    void OnMainMenu()
    {

        CurrentMenu = MenuState.MainMenu;

        ClassicBtn.onClick.AddListener(() => { StartCoroutine(FadeTransition(MainMenu, CharSelect, OnCharecterSelect)); });
        ArcadeBtn.onClick.AddListener(() =>
        {
            
        });
        PvPBtn.onClick.AddListener(() =>
        {
            var img = FindByNameinList<Image>(MainMenu.images, "Message");
            img.gameObject.SetActive(true);
            StartCoroutine(TaskDelay(2f, () => img.gameObject.SetActive(false)));
        });
        SettingsBtn.onClick.AddListener(() => { StartCoroutine(FadeTransition(MainMenu, CharSelect, OnCharecterSelect)); });
    }
    #endregion
    #region CharecterSelect
    #region InspecterVar
    [Space(15f)]
    [Header("Charecter Selection")]
    [SerializeField] UIObjects CharSelect;
    [SerializeField] List<Button> CharButtons;
    [SerializeField] Button AcceptBtn, DeclineBtn;
    [SerializeField] Sprite UnKnownChar;
    [SerializeField] Sprite P1, P2;

    [SerializeField] bool UpdateChar;
    #endregion
    [HideInInspector] public Charecter Player1, Player2;
    Image P1Avatar, P2Avatar;
    Coroutine CurrHighlight;
    /// <summary>
    /// Charecter Selection State
    /// </summary>
    private enum CST { P1Empty, P1Selecting, P1Accepted, P2Empty, P2Selecting, P2Accepted }
    CST cst;
    void OnCharecterSelect()
    {
        cst = CST.P1Empty;
        foreach (var btn in CharButtons)
        {
            btn.onClick.AddListener(delegate { OnCharecterClick(btn.name); });
        }
        ///var charecterBtn = CharSelect.SortTypeByName(CharSelect.button,"");

        P1Avatar = FindByNameinList(CharSelect.images, "P1Avatar");
        P2Avatar = FindByNameinList(CharSelect.images, "P2Avatar");

        CurrHighlight = StartCoroutine(ScaleHighlight(P1Avatar, 0.01f, 0.1f, 0.25f));

        AcceptBtn.onClick.AddListener(
            () =>
            {
                switch (cst)
                {
                    case CST.P1Selecting:
                        StopCoroutine(CurrHighlight);
                        CurrHighlight = StartCoroutine(ScaleHighlight(P2Avatar, 0.01f, 0.1f, 0.25f));
                        cst = CST.P1Accepted;
                        return;
                    case CST.P2Selecting:
                        StopCoroutine(CurrHighlight);
                        StartCoroutine(FadeTransition(CharSelect, LVLSelect, OnLevelSelect));
                        cst = CST.P2Accepted;
                        return;
                }
            }
        );
        DeclineBtn.onClick.AddListener(
            () =>
            {
                switch (cst)
                {
                    case CST.P1Selecting:
                        Player1 = new Charecter();
                        StopCoroutine(CurrHighlight);
                        CurrHighlight = StartCoroutine(ScaleHighlight(P1Avatar, 0.01f, 0.1f, 1));
                        cst = CST.P1Empty;
                        return;
                    case CST.P1Accepted:
                        StopCoroutine(CurrHighlight);
                        cst = CST.P1Selecting;
                        return;
                    case CST.P2Empty:
                        StopCoroutine(CurrHighlight);
                        cst = CST.P1Selecting;
                        return;
                    case CST.P2Selecting:
                        StopCoroutine(CurrHighlight);
                        P1Avatar.sprite = P1;
                        P2Avatar.sprite = P2;
                        CurrHighlight = StartCoroutine(ScaleHighlight(P1Avatar, 0.01f, 0.1f, 1));
                        cst = CST.P1Empty;
                        return;
                }
            }
        );
    }
    public void OnCharecterClick(string Btnname)
    {
        string CharName = Btnname.Remove(Btnname.Length - 3, 3);
        if (CST.P1Empty == cst || CST.P1Selecting == cst)
        {
            cst = CST.P1Selecting;
            Player1 = MDB.Charecters[CharName];
            StopCoroutine(CurrHighlight);
            P1Avatar.transform.localScale = new Vector3(0.2f, 0.2f, 0.9f);
            P1Avatar.sprite = Player1.forms[0].FormFace;
        }
        else if (CST.P2Empty == cst || CST.P2Selecting == cst || CST.P1Accepted == cst)
        {
            cst = CST.P2Selecting;
            Player2 = MDB.Charecters[CharName];
            StopCoroutine(CurrHighlight);
            P2Avatar.transform.localScale = new Vector3(0.2f, 0.2f, 0.9f);
            P2Avatar.sprite = Player2.forms[0].FormFace;
        }
    }
    IEnumerator ScaleHighlight(Image img, float speed, float sizeChange, float waitBefore)
    {
        yield return new WaitForSeconds(waitBefore);
        Transform trans = img.transform;
        float CurrentSize = trans.localScale.x;
        bool invers = false;
        while (true)
        {
            if (invers)
            {
                trans.localScale += Vector3.one * speed;
                if (trans.localScale.x > sizeChange + CurrentSize) invers = false;
            }
            else
            {
                trans.localScale -= Vector3.one * speed;
                if (trans.localScale.x < -sizeChange + CurrentSize) invers = true;
            }
            yield return null;
        }
    }
    #endregion
    #region LevelSelect
    #region InspecterVar
    [Space(15f)]
    [Header("Level Selection")]
    [SerializeField] UIObjects LVLSelect;
    [SerializeField] Button NextLvl, PrevLvl, LvlAcptBtn;
    #endregion
    int NoOfLVL, CurrentLvl = 0;
    Image LvlImg;

    void OnLevelSelect()
    {
        NoOfLVL = MDB.Arenas.Count;
        LvlImg = FindByNameinList<Image>(LVLSelect.images, "LevelImg");
        NextLvl.onClick.AddListener(() =>
        {
            if (CurrentLvl + 1 >= NoOfLVL) { CurrentLvl = NoOfLVL - 1; return; }
            CurrentLvl++;
            LvlImg.sprite = MDB.Arenas[CurrentLvl].sprite;
        });

        PrevLvl.onClick.AddListener(() =>
        {
            if (CurrentLvl <= 0) { CurrentLvl = 0; return; }
            CurrentLvl--;
            LvlImg.sprite = MDB.Arenas[CurrentLvl].sprite;
        });
        LvlAcptBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadScene("BattleScreen");
            LVLSelect.obj.SetActive(false);
        });

    }
    #endregion
    #region Settings
    #region InspecterVar
    [Space(15f)]
    [Header("Settings")]
    [SerializeField] UIObjects Settings;
    #endregion
    void OnSettings()
    {

    }
    #endregion
    #region Miscellaneous
    IEnumerator FadeTransition(UIObjects from, UIObjects To, Action method)
    {
        float FadeCounter = 0;
        float FadeSpeed = 2f;
        while (FadeCounter <= 1.05)
        {
            float alpha = FadeSpeed * Time.deltaTime;
            foreach (var image in from.images)
            {
                Color col = image.color;
                col.a -= alpha;
                image.color = col;
            }
            foreach (var Text in from.texts)
            {
                Color col = Text.color;
                col.a -= alpha;
                Text.color = col;
            }
            FadeCounter += alpha;
            yield return null;
        }
        from.obj.SetActive(false);
        foreach (var image in To.images)
        {
            Color col = image.color;
            col.a = 0;
            image.color = col;
        }
        foreach (var Text in To.texts)
        {
            Color col = Text.color;
            col.a = 0;
            Text.color = col;
        }
        To.obj.SetActive(true);
        while (FadeCounter >= 0)
        {
            float alpha = FadeSpeed * Time.deltaTime;
            foreach (var image in To.images)
            {
                Color col = image.color;
                col.a += alpha;
                image.color = col;
            }
            foreach (var Text in To.texts)
            {
                Color col = Text.color;
                col.a += alpha;
                Text.color = col;
            }
            FadeCounter -= alpha;
            yield return null;
        }
        method();
    }

    IEnumerator TaskDelay(float Sec, Action task)
    {
        yield return new WaitForSeconds(Sec);
        task();
    }

    public List<T> SortTypeByName<T>(List<T> SortObj, string Name, bool Contains)
    where T : MonoBehaviour
    {
        List<T> components = new List<T>();
        foreach (var i in SortObj)
        {
            if (i.name == Name && Contains) components.Add(i);
            else if (i.name != Name && !Contains) components.Add(i);
        }
        return components;
    }
    public List<T> SortTypeByTag<T>(List<T> SortObj, string Tag)
    where T : MonoBehaviour
    {
        List<T> components = new List<T>();
        foreach (var i in SortObj)
        {
            if (i.tag == Tag) components.Add(i);
        }
        return components;
    }
    public T FindByNameinList<T>(List<T> component, string Name)
    where T : MonoBehaviour
    {
        foreach (var i in component)
        {
            if (i.name == Name) return i;
        }
        return null;
    }
    T2 ArrayToDictionary<T, T2>(T[] array)
        where T : MonoBehaviour
        where T2 : SerializableDictionaryBase<string, T>, new()
    {
        T2 dict = new T2();
        foreach (var i in array)
        {
            dict.Add(i.name, i);
        }
        return dict;
    }
    #endregion
    #region Gizmos & AutoSet
#if (UNITY_EDITOR)
    [SerializeField] MenuState UpdateState;
    [SerializeField] bool UpdateObj;
    private void OnDrawGizmosSelected()
    {
        if (UpdateObj)
        {
            switch (UpdateState)
            {
                case MenuState.StartMenu: StartMenu.SetObj(); break;
                case MenuState.MainMenu: MainMenu.SetObj(); break;
                case MenuState.Settings: Settings.SetObj(); break;
                case MenuState.CharSelect: CharSelect.SetObj(); break;
                case MenuState.LvlSelect: LVLSelect.SetObj(); break;

            }
            UpdateObj = false;
        }
        if (UpdateChar)
        {
            var IconImages = SortTypeByTag<Image>(CharSelect.images, "CharecterBtn");
            var charecters = MDB.Charecters.Values.ToList();
            for (int i = 0; i < IconImages.Count; i++)
            {

                if (charecters.Count > i)
                {
                    IconImages[i].sprite = charecters[i].forms[0].FormFace;
                    IconImages[i].name = charecters[i].BaseName + "Btn";

                    Button Btn = IconImages[i].GetComponent<Button>();
                    if (!CharButtons.Contains(Btn)) CharButtons.Add(Btn);
                }
                else { IconImages[i].sprite = UnKnownChar; IconImages[i].name = "UnKnownCharBtn"; }
                IconImages[i].gameObject.SetActive(true);
            }
            UpdateChar = false;
        }
    }
#endif
    #endregion
}

[Serializable]
public class UIObjects
{
    public void SetObj()
    {
        var img = new List<Image>(obj.GetComponentsInChildren<Image>(true));
        var Txt = new List<Text>(obj.GetComponentsInChildren<Text>(true));
        images = new List<Image>();
        texts = new List<Text>();
        foreach (var i in img)
        {
            if (i.color.a != 0) images.Add(i);
        }
        foreach (var i in Txt)
        {
            if (i.color.a != 0) texts.Add(i);
        }
    }

    public GameObject obj;
    public List<Image> images;
    public List<Text> texts;

}