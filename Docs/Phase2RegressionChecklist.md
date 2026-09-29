# Phase 2.0 combat regression baseline

Run `CombatRegressionTests` in Unity's Edit Mode Test Runner. The automated
checks cover reduction → shield → HP, wall policy, sealed skills, same-tick
death, one-survivor victory, timeout HP ratios, and the charge bar midpoint.

Manual play checks for rules tied to frame timing and input:

1. Start `InGameScene` with Warrior. Confirm the match clock starts at 5:00.
2. Move into a wall, release the joystick, and attack. Facing must stay fixed
   until the player gives a new movement or attack direction.
3. Attack while moving. The attack keeps its selected direction. Windup stops
   movement; recovery permits it.
4. Use Double Slash. Observe first hit, 0.3 second gap, second hit, then
   recovery. The skill cooldown starts at activation.
5. Use Rush Slash into each wall type. Movement stops at first wall contact.
6. Start `InGameScene` with Archer. Charge Divine Bow. The ground plate and
   separate bar appear together; the bar shrinks from full to empty over five
   seconds. Both disappear when the arrow fires or the windup is cancelled.
7. Shoot an arrow through a low wall and at a high wall. It passes the low wall
   and stops at the high wall. Melee attacks are blocked by both.
8. Damage each fighter separately and simultaneously. A sole survivor wins;
   two deaths in one adjudication tick draw. At 5:00, higher remaining HP
   percentage wins.

This baseline intentionally does not claim automated coverage for the timed
input cases. Those are kept here until the local two-player and tick stages
provide a deterministic harness.
