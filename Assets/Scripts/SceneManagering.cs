using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagering : MonoBehaviour
{
    public static SceneManagering Instance { get; private set; }
    public SkillsetBase playerSkillset;
    public SkillsetBase enemySkillset;



    void Awake()
    {
        if(Instance != null&&Instance!=this)
            Destroy(Instance);
        Instance = this;
    }


    public void GameStart()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("EnteranceScene");
    }

}
