# Phase 2.4: input, Divine Bow, and terrain status

Divine Bow can now turn toward the movement joystick or desktop movement keys
throughout windup while translation stays blocked. Pressing the skill button
again after at least one second fires early. Damage is 50% at one second and
increases linearly to 100% at five seconds. The cooldown still starts when
the arrow is actually launched. The existing ground plate and five-second
charge bar remain visible until firing or windup cancellation.

Water detection still reads the current map cell, but it now refreshes one
`SlowStatus(0.25)` for one second. Repeated checks do not stack modifiers.
Leaving water lets that timed status expire through the common combat status
controller; death, match end, and disable clear it through the same lifecycle.

The 2.3 review follow-ups scheduled for this phase are handled as follows:
child collider owners receive their fighter's target layer; physical desktop
bindings are separate from logical skill slots; `GameManager` configures the
local input profile after spawning; Play Mode attack tests wait for damage
with a timeout; sequential status cleanup has a regression test; and Divine
Bow finds its own named charge bar. Existing tests already cover the combat
tick expiration, defense, and periodic-heal boundaries. Mobile control layout
and two-person simultaneous input still require a device/manual play pass.

## 2.3 review accepted for phase 2.5

- Unify world-position-to-cell conversion for water lookup and spawn
  validation. Both currently use different rounding rules at half-cell
  coordinates; explicitly define the boundary behavior before changing maps.
- Validate row count and each row width with a clear error for the first row,
  rather than deriving all width validation from row zero.
- Define safe same-frame map rebuild semantics before introducing runtime map
  switching. `Destroy()` currently removes old map objects at frame end.
- Verify representative high wall, low wall, water, and both spawn coordinates
  in the instantiated 31×31 scene, in addition to data-level checks.
- Profile the 31×31 scene on the target mobile device. If 961 or more cell
  GameObjects become a bottleneck, combine repeated floor and wall geometry
  or use a tile/mesh batching approach.

Per-frame water lookup is constant-time and involves only two fighters, so
the review does not justify changing it now. The shared material is already
disposed by the map generator. These are not outstanding defects.
