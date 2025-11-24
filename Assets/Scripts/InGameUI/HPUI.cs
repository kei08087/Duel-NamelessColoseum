using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    public Slider playerSlide;
    public Slider enemySlide;

    CharacterStatistics playerCStat;
    CharacterStatistics enemyCStat;
    public void OnEnable()
    {
        EventManager.PlayerUIConnection += Connect;
    }

    public void OnDisable()
    {
        EventManager.PlayerUIConnection -= Connect;
    }

    public void Update()
    {
        if(playerCStat != null)
            playerSlide.value=playerCStat.hp;
        if(enemyCStat != null)
            enemySlide.value = enemyCStat.hp;
    }

    void Connect(GameObject obj, bool isPlayer)
    {
        if (isPlayer)
        {
            playerCStat = obj.GetComponent<CharacterStatistics>();
            playerSlide.maxValue = playerCStat.Mhp;
            playerSlide.value = playerCStat.hp;
        }
        else
        {
            enemyCStat = obj.GetComponent<CharacterStatistics>();
            enemySlide.maxValue = enemyCStat.Mhp;
            enemySlide.value = enemyCStat.hp;
        }
    }


}
