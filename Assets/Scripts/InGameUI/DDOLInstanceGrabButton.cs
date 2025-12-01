using UnityEngine;

public class DDOLInstanceGrabButton : MonoBehaviour
{
    SceneManagering SceneManagering;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManagering = SceneManagering.Instance;
    }

    // Update is called once per frame
    public void restart()
    {
        SceneManagering.GameStart();
    }

    public void toMenu()
    {
        SceneManagering.GameOver();
    }
}
