# Phase 2.3: map data and water

`MapLayout` stores a cell grid and both spawn positions. `MapGenerater` builds
the selected layout from the existing floor, wall, and spawner prefabs. The
`InGameScene` reference remains `TestArena17`, preserving the 17×17 prototype
for local combat checks. `DesertRuinsTrialScene` uses the separate 31×31
`DesertRuinsTrial31` layout with symmetric cover and water. Open that scene
directly to playtest the larger map. The 31×31 size is
provisional until a human playtest; volcano timings must be recalculated only
after the production size is settled.

Water is a floor cell with a blue surface marker. A fighter whose position is
in a water cell moves at 75% of its final speed, including movement bonuses
and existing slows. The surface has no collider; the floor beneath remains
walkable. Water does not alter projectile or attack collision.

## Review items queued for phase 2.4

- Define the team layer policy for child hurtbox colliders. `AssignSide()` only
  changes the character root today, which works for the current root colliders.
  Do not recursively relayer skill effects or projectiles by accident.
- Separate physical desktop/mobile bindings from logical `CombatCommand` skill
  slots before expanding the mobile input UI; manually verify simultaneous
  player-one and player-two input.
- Move input-provider selection out of `Spawner` so local, AI, and future
  network actors can share character spawning.
- Replace the fixed 0.25-second Play Mode attack wait with a bounded
  condition/event wait, then add an idempotent sequential
  `Clear()` → `OnDisable()`/death status cleanup regression.
- Document and test combat-tick edges: duration rounds up to a tick, defense
  expires before same-tick queued damage, and periodic healing includes the
  duration boundary. Confirm that these descriptions match the skill UI text.
- Keep Divine Bow UI lookup explicit if a character gains multiple charge
  displays. The current Archer has only one.

The review's stun-after-execution observation matches the confirmed combat
rule, so it does not call for a change. Missing external GitHub CI statuses
do not indicate a code failure; local Unity Test Runner results are reported
with each phase until a repository CI workflow is established.
