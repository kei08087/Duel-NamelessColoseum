# Phase 2.2: adjudication tick and status lifecycle

`GameManager.AdvanceCombatTick()` is called once per Unity `FixedUpdate`.
`CombatAdjudicator` owns queued hits and the tick number. Each tick advances
timed effects, resolves the damage queue snapshot, then decides death or
timeout. Hits added while resolving a snapshot wait for the following tick.
Two deaths in one tick draw; a sole survivor wins; timeout compares remaining
HP percentages. `RequestTimeout()` records a timeout for the next tick, so
damage and death on that tick still take priority.

`CombatStatusController` owns timed effects and stun on each fighter. Archer
Agility and Wind Blessing, Warrior Shield Up and Smite slow, and Warrior
Recover Health now share this lifecycle. Durations are rounded up to combat
ticks. Expiration, death, match end, and character disable remove modifiers
once. Healing pulses every second, including at the duration boundary.
Stun cancels a skill only while it is in windup; a skill already executing
continues. Buff expiration occurs before pending damage is resolved on that
tick. Damage reduction still precedes shield consumption and HP loss.

The Divine Bow bar now reuses one Canvas per Archer. Hiding it disables the
existing object. Its position is projected from a point 0.85 world metres
below the fighter in camera space, avoiding pixel offset calculations tied to
CanvasScaler. A temporarily absent main camera hides the Canvas and a later
camera restores it while charging. Repeated windup starts reuse the same bar.

Tests: `CombatRegressionTests` in Edit Mode and `LocalDuelTests` in Play Mode.
The suite covers tick boundaries, status expiration and cleanup, combat
outcomes, mutual attacks, windup stun, and charge-bar reuse. The bar position
is checked against the camera projection in landscape and portrait viewports.
Final visual spacing still needs a device check. Movement and skill coroutine
steps remain frame-based; this stage establishes an authoritative **damage,
status, and outcome** tick, not a fully deterministic network simulation.
