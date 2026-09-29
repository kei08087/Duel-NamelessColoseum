#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityEngine;

public class CombatRegressionTests
{
    [Test]
    public void MapLayoutsPreserveTestArenaAndProvideWaterTrial()
    {
        MapLayout test = Resources.Load<MapLayout>("MapLayouts/TestArena17");
        MapLayout trial = Resources.Load<MapLayout>("MapLayouts/DesertRuinsTrial31");
        Assert.That(test, Is.Not.Null);
        Assert.That(trial, Is.Not.Null);
        Assert.That(test.IsValid(out _), Is.True);
        Assert.That(trial.IsValid(out _), Is.True);
        Assert.That((test.Width, test.Height), Is.EqualTo((17, 17)));
        Assert.That((trial.Width, trial.Height), Is.EqualTo((31, 31)));
        Assert.That(test.TileAt(3, 3), Is.EqualTo('L'));
        Assert.That(test.TileAt(8, 3), Is.EqualTo('F'));
        Assert.That(trial.TileAt(10, 15), Is.EqualTo('W'));
        Assert.That(trial.TileAt(15, 15), Is.EqualTo('F'));
    }

    [Test]
    public void WaterReducesFinalMovementSpeedByOneQuarter()
    {
        GameObject actor = new GameObject("Water speed target");
        try
        {
            CharacterStatistics stats = actor.AddComponent<CharacterStatistics>();
            stats.moveSpeed = 4f;
            stats.AddAgility(0f, 2f);
            Assert.That(stats.CalculateMoveSpeed(false, 2f), Is.EqualTo(5f));
            Assert.That(stats.CalculateMoveSpeed(true, 2f), Is.EqualTo(3.75f));

            MapLayout trial = Resources.Load<MapLayout>("MapLayouts/DesertRuinsTrial31");
            Assert.That(trial.IsWaterAt(new Vector3(10f, 1.75f, 15f)), Is.True);
            Assert.That(trial.IsWaterAt(new Vector3(15f, 1.75f, 15f)), Is.False);
        }
        finally { Object.DestroyImmediate(actor); }
    }

    private sealed class FlatReduction : IDamageProcess
    {
        public int priority => 0;
        public void preprocess(ref DamageBlock damage, CharacterStatistics target) => damage.damage -= 3f;
        public void postprocess(in DamageBlock damage, CharacterStatistics target) { }
    }

    private sealed class FollowUpDamage : IDamageProcess
    {
        private readonly GameManager match;
        private readonly CharacterStatistics other;
        public int priority => 0;

        public FollowUpDamage(GameManager match, CharacterStatistics other)
        {
            this.match = match;
            this.other = other;
        }

        public void preprocess(ref DamageBlock damage, CharacterStatistics target) { }
        public void postprocess(in DamageBlock damage, CharacterStatistics target) =>
            match.QueueDamage(other, new DamageBlock { damage = 5f });
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
            match.AdvanceCombatTick();
            Assert.That(match.Result, Is.EqualTo(GameManager.MatchResult.Draw));
        });
    }

    [Test]
    public void OneDeathGivesSurvivorVictoryRegardlessOfDamageSource()
    {
        WithMatch((match, player, enemy) =>
        {
            match.QueueDamage(enemy, new DamageBlock { damage = 150f });
            match.AdvanceCombatTick();
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
            match.RequestTimeout();
            match.AdvanceCombatTick();
            Assert.That(match.Result, Is.EqualTo(GameManager.MatchResult.EnemyWin));
        });
    }

    [Test]
    public void DamageQueuedDuringResolutionWaitsUntilFollowingTick()
    {
        WithMatch((match, player, enemy) =>
        {
            player.assignModifier(new FollowUpDamage(match, enemy));
            match.QueueDamage(player, new DamageBlock { damage = 1f });
            match.AdvanceCombatTick();
            Assert.That(match.CombatTick, Is.EqualTo(1));
            Assert.That(enemy.hp, Is.EqualTo(enemy.Mhp));

            match.AdvanceCombatTick();
            Assert.That(match.CombatTick, Is.EqualTo(2));
            Assert.That(enemy.hp, Is.EqualTo(enemy.Mhp - 5f));
        });
    }

    [Test]
    public void LethalDamageClearsVictimsStatusesAtTheTickBoundary()
    {
        WithMatch((match, player, enemy) =>
        {
            CombatStatusController statuses = player.gameObject.AddComponent<CombatStatusController>();
            statuses.ApplyTimed(new AgilityStatus(1f, 2f), 5f);
            Assert.That(statuses.ActiveCount, Is.EqualTo(1));
            match.QueueDamage(player, new DamageBlock { damage = 150f });
            match.AdvanceCombatTick();
            Assert.That(match.Result, Is.EqualTo(GameManager.MatchResult.EnemyWin));
            Assert.That(statuses.ActiveCount, Is.Zero);
            Assert.That(player.BasicAttackCooldown(1f), Is.EqualTo(1f).Within(0.001f));
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

            Camera view = cameraObject.GetComponent<Camera>();
            bar.Refresh(view, 2.5f);
            Assert.That(fill.sizeDelta.x, Is.EqualTo(58f).Within(0.5f));
            Assert.That(bar.RemainingFraction, Is.EqualTo(0.5f));

            Vector3 expected = view.WorldToScreenPoint(actor.transform.position - view.transform.up * 0.85f);
            RectTransform track = (RectTransform)bar.transform.Find("Track");
            Assert.That(track.position.x, Is.EqualTo(expected.x).Within(0.1f));
            Assert.That(track.position.y, Is.EqualTo(expected.y).Within(0.1f));

            view.pixelRect = new Rect(0f, 0f, 1080f, 1920f);
            bar.Refresh(view, 2.5f);
            Vector3 portrait = view.WorldToScreenPoint(actor.transform.position - view.transform.up * 0.85f);
            Assert.That(track.position.x, Is.EqualTo(portrait.x).Within(0.1f));
            Assert.That(track.position.y, Is.EqualTo(portrait.y).Within(0.1f));

            bar.Refresh(null, 3f);
            Assert.That(bar.gameObject.activeSelf, Is.True);
            Assert.That(bar.GetComponent<Canvas>().enabled, Is.False);
            bar.Refresh(view, 3f);
            Assert.That(bar.GetComponent<Canvas>().enabled, Is.True);
            bar.Hide();
            Assert.That(DivineBowChargeBar.Show(actor.transform, 5f), Is.SameAs(bar));
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

    [Test]
    public void TimedBuffExpiresOnCombatTickAndCanBeCleared()
    {
        GameObject actor = new GameObject("Buff target");
        try
        {
            CharacterStatistics stats = actor.AddComponent<CharacterStatistics>();
            stats.hp = stats.Mhp;
            CombatStatusController statuses = actor.AddComponent<CombatStatusController>();
            long duration = CombatStatusController.DurationTicks(1f);

            statuses.ApplyTimed(new AgilityStatus(1f, 2f), 1f);
            Assert.That(stats.BasicAttackCooldown(1f), Is.EqualTo(0.5f).Within(0.001f));
            statuses.AdvanceTick(duration - 1);
            Assert.That(statuses.ActiveCount, Is.EqualTo(1));
            statuses.AdvanceTick(duration);
            Assert.That(statuses.ActiveCount, Is.EqualTo(0));
            Assert.That(stats.BasicAttackCooldown(1f), Is.EqualTo(1f).Within(0.001f));

            statuses.ApplyTimed(new WindBlessingStatus(2f, 3f), 5f);
            Assert.That(stats.ProjectileSpeed(6f), Is.EqualTo(8f));
            statuses.Clear();
            Assert.That(stats.ProjectileSpeed(6f), Is.EqualTo(6f));
            Assert.That(stats.BasicAttackRange(4f), Is.EqualTo(4f));
        }
        finally { Object.DestroyImmediate(actor); }
    }

    [Test]
    public void ShieldUpReductionExpiresBeforeFollowingDamage()
    {
        GameObject actor = new GameObject("Defending target");
        try
        {
            CharacterStatistics stats = actor.AddComponent<CharacterStatistics>();
            stats.hp = stats.Mhp;
            CombatStatusController statuses = actor.AddComponent<CombatStatusController>();
            stats.GrantShield(4f);
            statuses.ApplyTimed(new ShieldUpStatus(5f), 1f);
            stats.ResolveDamage(new DamageBlock { damage = 10f });
            Assert.That(stats.Shield, Is.EqualTo(0f));
            Assert.That(stats.hp, Is.EqualTo(99f));
            statuses.AdvanceTick(CombatStatusController.DurationTicks(1f));
            stats.ResolveDamage(new DamageBlock { damage = 10f });
            Assert.That(stats.hp, Is.EqualTo(89f));
        }
        finally { Object.DestroyImmediate(actor); }
    }

    [Test]
    public void HealingPulsesOncePerSecondAndStopsAtExpiration()
    {
        GameObject actor = new GameObject("Healing target");
        try
        {
            CharacterStatistics stats = actor.AddComponent<CharacterStatistics>();
            stats.hp = 50f;
            CombatStatusController statuses = actor.AddComponent<CombatStatusController>();
            long second = CombatStatusController.DurationTicks(1f);
            statuses.ApplyTimed(new PeriodicHealStatus(3f), 2f);
            statuses.AdvanceTick(second - 1);
            Assert.That(stats.hp, Is.EqualTo(50f));
            statuses.AdvanceTick(second);
            Assert.That(stats.hp, Is.EqualTo(53f));
            statuses.AdvanceTick(second * 2);
            Assert.That(stats.hp, Is.EqualTo(56f));
            Assert.That(statuses.ActiveCount, Is.Zero);
            statuses.AdvanceTick(second * 3);
            Assert.That(stats.hp, Is.EqualTo(56f));
        }
        finally { Object.DestroyImmediate(actor); }
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

}
#endif
