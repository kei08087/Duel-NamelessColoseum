using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LocalDuelTests
{
    [UnityTest]
    public IEnumerator TrialMapBuildsAndWaterSlowsACombatant()
    {
        SceneManager.LoadScene("DesertRuinsTrialScene");
        yield return null;
        yield return null;

        MapGenerater generator = Object.FindFirstObjectByType<MapGenerater>();
        MapLayout trial = Resources.Load<MapLayout>("MapLayouts/DesertRuinsTrial31");
        Assert.That(generator, Is.Not.Null);
        Assert.That(generator.Layout, Is.SameAs(trial));

        CharacterStatistics fighter = GameManager.Instance.player.GetComponent<CharacterStatistics>();
        fighter.transform.position = new Vector3(15f, 1.75f, 15f);
        yield return null;
        float drySpeed = fighter.instanceSpeed;
        fighter.transform.position = new Vector3(10f, 1.75f, 15f);
        yield return null;
        Assert.That(fighter.instanceSpeed, Is.EqualTo(drySpeed * 0.75f).Within(0.001f));
        Assert.That(generator.transform.childCount, Is.GreaterThan(31 * 31));
    }

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

    [UnityTest]
    public IEnumerator StunCancelsDivineBowWindupAndReusesItsChargeBar()
    {
        SceneManager.LoadScene("InGameScene");
        yield return null;
        yield return null;

        GameObject archer = GameManager.Instance.enemy;
        CastController caster = archer.GetComponent<CastController>();
        Assert.That(caster.TryCast(new CombatCommand("Space", Vector3.zero)), Is.True);
        Assert.That(caster.Phase, Is.EqualTo(CastPhase.Windup));
        DivineBowChargeBar bar = archer.GetComponentInChildren<DivineBowChargeBar>(true);
        Assert.That(bar, Is.Not.Null);
        Assert.That(bar.gameObject.activeSelf, Is.True);

        caster.ApplyStun(0.25f);
        Assert.That(caster.Phase, Is.EqualTo(CastPhase.Ready));
        Assert.That(bar.gameObject.activeSelf, Is.False);
        Assert.That(caster.coolEnd.ContainsKey("Space"), Is.False);

        yield return new WaitForSeconds(0.3f);
        Assert.That(caster.TryCast(new CombatCommand("Space", Vector3.zero)), Is.True);
        Assert.That(archer.GetComponentInChildren<DivineBowChargeBar>(true), Is.SameAs(bar));
    }
}
