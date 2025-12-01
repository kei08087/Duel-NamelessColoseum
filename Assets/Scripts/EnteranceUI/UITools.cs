using UnityEngine;

public class UITools : MonoBehaviour
{
    public GameObject[] groups;
    public void openGroup(GameObject group)
    {
        group.SetActive(true);
    }

    public void closeGroup(GameObject group)
    {
        group.SetActive(false);
    }

    public void closeAllGroup()
    {
        foreach (GameObject group in groups)
        {
            group.SetActive(false);
        }
    }

    public void exitGame()
    {
# if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


}
