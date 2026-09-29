#if UNITY_INCLUDE_TESTS
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class CombatRegressionTests
{
    private sealed class FlatReduction : IDamageProcess
    {
        public int priority => 0;
        public void preprocess(ref DamageBlock damage, CharacterStatistics target) => damage.damage -= 3f;
        public void postprocess(in DamageBlock damage, CharacterStatistics target) { }
    }

    [Test]
    public void DamageReductionPrecedesShieldAndHp()
    {
        GameObject actor = new GameObject("Damage target");
        try
        {
            CharacterStatistics stats = actor.AddComponent<CharacterStatistics>();
            stats.hp = stats.Mhp;
            stats.GrantShield(4f);
            stats.assignModifier(new FlatReduction());
            stats.ResolveDamage(new DamageBlock { damage = 10f });

            Assert.That(stats.Shield, Is.EqualTo(0f));
            Assert.That(stats.hp, Is.EqualTo(97f));
        }
        finally { Object.DestroyImmediate(actor); }
    }

    [Test]
    public void WallPoliciesDistinguishMeleeProjectilesAndWallIgnoringSkills()
    {
        Assert.That(LayerMask.NameToLayer("LowWall"), Is.GreaterThanOrEqualTo(0));
        Assert.That(LayerMask.NameToLayer("HighWall"), Is.GreaterThanOrEqualTo(0));
        GameObject low = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject high = GameObject.CreatePrimitive(PrimitiveType.Cube);
        try
        {
            low.layer = LayerMask.NameToLayer("LowWall");
            high.layer = LayerMask.NameToLayer("HighWall");
            low.transform.position = new Vector3(2f, 0f, 0f);
            high.transform.position = new Vector3(4f, 0f, 0f);
            Physics.SyncTransforms();

            Assert.That(ProjectileCollision.Sweep(Vector3.zero, Vector3.right, 0.1f, 6f,
                0, AttackWallPolicy.BlockAllWalls, null, out RaycastHit meleeHit), Is.True);
            Assert.That(meleeHit.collider.gameObject, Is.SameAs(low));
            Assert.That(ProjectileCollision.Sweep(Vector3.zero, Vector3.right, 0.1f, 6f,
                0, AttackWallPolicy.BlockHighWalls, null, out RaycastHit arrowHit), Is.True);
            Assert.That(arrowHit.collider.gameObject, Is.SameAs(high));
            Assert.That(ProjectileCollision.Sweep(Vector3.zero, Vector3.right, 0.1f, 6f,
                0, AttackWallPolicy.IgnoreWalls, null, out _), Is.False);
        }
        finally
        {
            Object.DestroyImmediate(low);
            Object.DestroyImmediate(high);
        }
    }

    [Test]
    public void ZeroPointsSealSkillAndFivePointsCreateLevelFiveRuntimeCopy()
    {
        SkillsetBase set = ScriptableObject.CreateInstance<SkillsetBase>();
        DummySkill definition = ScriptableObject.CreateInstance<DummySkill>();
        try
        {
            set.QSkillSO = definition;
            set.setSkillLevel("Q", 0);
            set.init();
            Assert.That(set.getSkill("Q"), Is.Null);

            set.setSkillLevel("Q", 5);
            set.init();
            Assert.That(set.getSkill("Q"), Is.Not.SameAs(definition));
            Assert.That(set.getSkill("Q").skillLevel, Is.EqualTo(5));
        }
        finally
        {
            if (set.getSkill("Q") != null)
                Object.DestroyImmediate(set.getSkill("Q"));
            Object.DestroyImmediate(set);
            Object.DestroyImmediate(definition);
        }
    }

    [Test]
    public void DamageOnBothSidesInOneAdjudicationTickIsDraw()
    {
        WithMatch((match, player, enemy) =>
        {
            match.QueueDamage(player, new DamageBlock { damage = 150f });
            match.QueueDamage(enemy, new DamageBlock { damage = 150f });
            ResolveTick(match);
            Assert.That(match.Result, Is.EqualTo(GameManager.MatchResult.Draw));
        });
    }

    [Test]
    public void OneDeathGivesSurvivorVictoryRegardlessOfDamageSource()
    {
        WithMatch((match, player, enemy) =>
        {
            match.QueueDamage(enemy, new DamageBlock { damage = 150f });
            ResolveTick(match);
            Assert.That(match.Result, Is.EqualTo(GameManager.MatchResult.PlayerWin));
        });
    }

    [Test]
    public void TimeoutComparesRemainingHpRatios()
    {
        WithMatch((match, player, enemy) =>
        {
            player.Mhp = 100f;
            player.hp = 40f;
            enemy.Mhp = 50f;
            enemy.hp = 30f;
            typeof(GameManager).GetField("timeoutRequested", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(match, true);
            ResolveTick(match);
            Assert.That(match.Result, Is.EqualTo(GameManager.MatchResult.EnemyWin));
        });
    }

    [Test]
    public void DivineBowBarLosesHalfItsWidthHalfwayThroughCharge()
    {
        GameObject actor = new GameObject("Charging Archer");
        GameObject cameraObject = new GameObject("Test camera");
        try
        {
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            cameraObject.AddComponent<Camera>();
            DivineBowChargeBar bar = DivineBowChargeBar.Show(actor.transform, 5f);
            RectTransform fill = (RectTransform)bar.transform.Find("Track/Remaining Charge");
            Assert.That(fill.sizeDelta.x, Is.EqualTo(116f));

            typeof(DivineBowChargeBar).GetField("startedAt", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(bar, Time.time - 2.5f);
            typeof(DivineBowChargeBar).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(bar, null);
            Assert.That(fill.sizeDelta.x, Is.EqualTo(58f).Within(0.5f));
        }
        finally
        {
            Object.DestroyImmediate(actor);
            Object.DestroyImmediate(cameraObject);
        }
    }

    [Test]
    public void TwoCombatantsOwnSeparateSkillInstancesAndOpponentMasks()
    {
        DummySkill definition = ScriptableObject.CreateInstance<DummySkill>();
        SkillsetBase first = ScriptableObject.CreateInstance<SkillsetBase>();
        SkillsetBase second = ScriptableObject.CreateInstance<SkillsetBase>();
        try
        {
            first.QSkillSO = definition;
            second.QSkillSO = definition;
            first.setSkillLevel("Q", 1);
            second.setSkillLevel("Q", 5);
            first.init(CombatTargeting.OpponentMask(true));
            second.init(CombatTargeting.OpponentMask(false));

            Assert.That(first.getSkill("Q"), Is.Not.SameAs(second.getSkill("Q")));
            Assert.That(first.getSkill("Q").skillLevel, Is.EqualTo(1));
            Assert.That(second.getSkill("Q").skillLevel, Is.EqualTo(5));
            Assert.That(first.getSkill("Q").targetMask.value, Is.EqualTo(LayerMask.GetMask("Enemy")));
            Assert.That(second.getSkill("Q").targetMask.value, Is.EqualTo(LayerMask.GetMask("Player")));
        }
        finally
        {
            if (first.getSkill("Q") != null) Object.DestroyImmediate(first.getSkill("Q"));
            if (second.getSkill("Q") != null) Object.DestroyImmediate(second.getSkill("Q"));
            Object.DestroyImmediate(first);
            Object.DestroyImmediate(second);
            Object.DestroyImmediate(definition);
        }
    }

    [Test]
    public void LocalInputSourcesKeepCommandsAndMovementSeparate()
    {
        GameObject firstObject = new GameObject("First controller");
        GameObject secondObject = new GameObject("Second controller");
        try
        {
            CombatInputSource first = firstObject.AddComponent<CombatInputSource>();
            CombatInputSource second = secondObject.AddComponent<CombatInputSource>();
            first.desktopProfile = DesktopCombatProfile.PlayerOne;
            second.desktopProfile = DesktopCombatProfile.PlayerTwo;
            first.SetJoystick(Vector2.right);
            second.SetJoystick(Vector2.left);
            first.Press("Q", Vector3.forward);
            second.Press("Space", Vector3.back);

            Assert.That(first.MoveDirection, Is.EqualTo(Vector3.right));
            Assert.That(second.MoveDirection, Is.EqualTo(Vector3.left));
            Assert.That(first.TryDequeue(out CombatCommand firstCommand), Is.True);
            Assert.That(second.TryDequeue(out CombatCommand secondCommand), Is.True);
            Assert.That(firstCommand.Slot, Is.EqualTo("Q"));
            Assert.That(secondCommand.Slot, Is.EqualTo("Space"));
            Assert.That(first.TryDequeue(out _), Is.False);
            Assert.That(second.TryDequeue(out _), Is.False);
        }
        finally
        {
            Object.DestroyImmediate(firstObject);
            Object.DestroyImmediate(secondObject);
        }
    }

    private static void WithMatch(System.Action<GameManager, CharacterStatistics, CharacterStatistics> check)
    {
        GameObject managerObject = new GameObject("Match manager");
        GameObject playerObject = new GameObject("Player");
        GameObject enemyObject = new GameObject("Enemy");
        try
        {
            GameManager match = managerObject.AddComponent<GameManager>();
            CharacterStatistics player = playerObject.AddComponent<CharacterStatistics>();
            CharacterStatistics enemy = enemyObject.AddComponent<CharacterStatistics>();
            player.hp = player.Mhp;
            enemy.hp = enemy.Mhp;
            match.RegisterPlayer(playerObject);
            match.RegisterEnemy(enemyObject);
            check(match, player, enemy);
        }
        finally
        {
            Object.DestroyImmediate(managerObject);
            Object.DestroyImmediate(playerObject);
            Object.DestroyImmediate(enemyObject);
        }
    }

    private static void ResolveTick(GameManager match)
    {
        typeof(GameManager).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(match, null);
    }
}
#endif
