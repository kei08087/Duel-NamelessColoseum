using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LocalDuelTests
{
    [UnityTest]
    public IEnumerator InGameSceneSpawnsTwoPlayableCombatants()
    {
        SceneManager.LoadScene("InGameScene");
        yield return null;
        yield return null;

        GameManager match = GameManager.Instance;
        Assert.That(match, Is.Not.Null);
        Assert.That(match.player, Is.Not.Null);
        Assert.That(match.enemy, Is.Not.Null);

        CharacterStatistics player = match.player.GetComponent<CharacterStatistics>();
        CharacterStatistics enemy = match.enemy.GetComponent<CharacterStatistics>();
        Assert.That(player.skillSet, Is.Not.Null);
        Assert.That(enemy.skillSet, Is.Not.Null);
        Assert.That(player.skillSet.type, Is.EqualTo(CharacterEnum.Warrior));
        Assert.That(enemy.skillSet.type, Is.EqualTo(CharacterEnum.Archer));
        Assert.That(player.skillSet, Is.Not.SameAs(enemy.skillSet));
        Assert.That(player.skillSet.LeftClick, Is.Not.SameAs(enemy.skillSet.LeftClick));
        Assert.That(match.player.layer, Is.EqualTo(CombatTargeting.SideLayer(true)));
        Assert.That(match.enemy.layer, Is.EqualTo(CombatTargeting.SideLayer(false)));
        Assert.That(player.skillSet.LeftClick.targetMask.value,
            Is.EqualTo(CombatTargeting.OpponentMask(true).value));
        Assert.That(enemy.skillSet.LeftClick.targetMask.value,
            Is.EqualTo(CombatTargeting.OpponentMask(false).value));

        CombatInputSource firstInput = match.player.GetComponent<CombatInputSource>();
        CombatInputSource secondInput = match.enemy.GetComponent<CombatInputSource>();
        Assert.That(firstInput.desktopProfile, Is.EqualTo(DesktopCombatProfile.PlayerOne));
        Assert.That(secondInput.desktopProfile, Is.EqualTo(DesktopCombatProfile.PlayerTwo));

        match.player.transform.position = new Vector3(8f, 1.75f, 8f);
        match.enemy.transform.position = new Vector3(8f, 1.75f, 8.8f);
        match.player.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        match.enemy.transform.rotation = Quaternion.LookRotation(Vector3.back);
        Physics.SyncTransforms();
        float playerHp = player.hp;
        float enemyHp = enemy.hp;
        Assert.That(match.player.GetComponent<CastController>().TryCast(
            new CombatCommand("LClick", Vector3.zero)), Is.True);
        Assert.That(match.enemy.GetComponent<CastController>().TryCast(
            new CombatCommand("LClick", Vector3.zero)), Is.True);
        yield return new WaitForSeconds(0.25f);
        Assert.That(player.hp, Is.LessThan(playerHp));
        Assert.That(enemy.hp, Is.LessThan(enemyHp));
    }
}
