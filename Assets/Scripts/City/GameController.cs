using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GameController Instance;
    public Animator anim;

    public int GameState;

    private int levelToLoad;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void FadeToLevel(int levelIndex)
    {
        anim.SetTrigger("FadeOut");
        levelToLoad = levelIndex;
    }

    public void LoadScene()
    {

        SceneManager.LoadScene(levelToLoad);
        anim.SetTrigger("FadeIn");
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelToLoad);

        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}
    }
}
