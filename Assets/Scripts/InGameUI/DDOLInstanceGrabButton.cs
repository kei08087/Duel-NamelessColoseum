using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOLInstanceGrabButton : MonoBehaviour
{
    public void restart()
    {
        if (SceneManagering.Instance != null)
            SceneManagering.Instance.GameStart();
        else
            SceneManager.LoadScene("InGameScene");
    }

    public void toMenu()
    {
        if (SceneManagering.Instance != null)
            SceneManagering.Instance.GameOver();
        else
            SceneManager.LoadScene("EnteranceScene");
    }
}
