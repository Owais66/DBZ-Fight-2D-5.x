using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance
    {
        get
        {   
            if (instance == null) instance = new GameObject("GameManager").AddComponent<GameManager>(); //create game manager object if required
            return instance;    
        }
    }
    private static GameManager instance = null;
    #endregion
    GameObject LoadingPrefab ;
    void Awake()
    {   
        //Check if there is an existing instance of this object
        if ((instance) && (instance.GetInstanceID() != GetInstanceID()))
            DestroyImmediate(gameObject); //Delete duplicate
        else
        {
            instance = this; //Make this object the only instance
            DontDestroyOnLoad(gameObject); //Set as do not destroy
        }
    }
    private void Start() {
        LoadingPrefab = (GameObject)Resources.Load("LoadingCanvas");
    }
    #region LoadingScene
    public void LoadScene(string sceneName)
    {     
            GameObject LoadingObj = Instantiate(LoadingPrefab, Vector3.zero, Quaternion.identity);
            LoadingObj.GetComponent<LoadSceneCont>().Load(sceneName);
    }
    public void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// Do not Use. only for MenuManager
    /// </summary>
    public MenuManager.MenuState MenuState = MenuManager.MenuState.StartMenu;
    public void LoadMainMenu(MenuManager.MenuState menuState){
        MenuState = menuState;
        SceneManager.LoadScene("MainMenu");
    }
    
    #endregion
}