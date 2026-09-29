# Phase 2.1: two local combatants

`InGameScene` uses its serialized Warrior skillset for player one and Archer
skillset for player two. `SceneManagering.enemySkillset`, when assigned, overrides
the scene's player-two fallback. If neither is assigned, the match uses a mirror
of player one's selection. Both sides receive independent runtime skillsets.

Editor/desktop controls for a local two-person check:

| Action | Player one | Player two |
| --- | --- | --- |
| Move | WASD or arrows | I, J, K, L |
| Basic attack | Left mouse | 1 |
| RClick, Q, E, LShift, Space, LCtrl | Right mouse, Q, E, Left Shift, Space, Left Ctrl | 2, 3, 4, 5, 6, 7 |

The existing mobile controls remain bound to player one. Player two's desktop
controls are for the local test stage; mobile two-device input belongs to a
later networking stage. The existing HP UI displays both combatants.

Validation: run `CombatRegressionTests` in Edit Mode and `LocalDuelTests` in
Play Mode. The Play Mode check loads `InGameScene`, verifies two playable
characters, separate skillsets, opposing target masks, separate input
profiles, accepted basic attacks on both sides, and HP loss from both attacks.
Then play the scene and confirm each person can move and attack the other.

## Deferred review findings for phase 2.2

- Reuse a persistent charge-bar UI instead of creating a Canvas each charge.
- Check charge-bar placement at multiple mobile aspect ratios and use a clear
  coordinate-space rule for its offset.
- Keep the charge bar alive when `Camera.main` is briefly unavailable, then
  reattach when a camera returns.
- Add a defensive guard against duplicate Divine Bow windup callbacks, even
  though the current cast state machine prevents duplicate starts.
- As tick adjudication is extracted, replace private-field reflection in
  combat tests with stable match/tick behavior entry points.

These are maintainability and lifecycle risks; the review found no immediate
fatal bug. Unity's 2.0 Edit Mode test assembly was compiled and executed in
the preceding stage.
