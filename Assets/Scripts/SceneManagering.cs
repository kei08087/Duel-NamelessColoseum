using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagering : MonoBehaviour
{
    public static SceneManagering Instance { get; private set; }
    public SkillsetBase playerSkillset;
    public SkillsetBase enemySkillset;



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void GameStart()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void GameOver()
    {
        // The entrance scene owns its own SceneManagering and UI button references.
        // Release this persistent instance before loading that scene again.
        Instance = null;
        Destroy(gameObject);
        SceneManager.LoadScene("EnteranceScene");
    }

}
