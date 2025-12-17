using UnityEngine;
using TMPro;

public class ApplyResultUI : MonoBehaviour
{
    public TextMeshProUGUI text;



    public void apply(string result)
    {
        switch(result)
        {
            case "win":
                text.text = "You Won!";
                break;
            case "lose":
                text.text = "You Lost";
                break;
            default:
                text.text = "Draw";
                break;
        }
    }
}
