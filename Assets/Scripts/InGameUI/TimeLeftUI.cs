using UnityEngine;
using TMPro;

public class TimeLeftUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.gameEnd)
        {
            text.text = GameManager.Instance.gameTime.ToString();
        }
    }
}
