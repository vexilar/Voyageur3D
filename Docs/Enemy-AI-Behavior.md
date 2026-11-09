# Enemy AI Behavior

How enemy wander, turn-to-inspect, pursue, and melee attack work together. Sensing lives in `Enemy`; movement and combat are separate components that react to its events.

## Overview

Enemy AI is event-driven and split across small MonoBehaviours on the same GameObject (plus a `NavMeshAgent`). `Enemy` detects the player and raises suspicion / vision events. The other scripts subscribe and start or stop their own coroutines.

| Component | Role |
| --- | --- |
| `Enemy` | Hear / see the player; raise suspicion events |
| `EnemyWander` | Idle patrol inside a radius around spawn |
| `EnemyTurnToInspect` | After hearing (but not seeing), face toward the noise |
| `EnemyPursue` | After seeing, chase within a leash |
| `EnemyMeleeAttack` | While in melee range during pursuit, fire attack events |

Typical consumers (e.g. `NewAgeBearAnimationController`, `NewAgeBearSoundController`) listen to the same events for animation and audio. They do not drive AI decisions.

## Sensing (`Enemy`)

**File:** `Assets/Scripts/Enemy/Enemy.cs`

`Enemy` owns hearing and sight. It does not move the agent. Detection uses a trigger collider on the enemy (player enter / exit).

### Fields

| Field | Default idea | Role |
| --- | --- | --- |
| `PeripheralVision` | `0.2` | Dot-product threshold vs `transform.forward`. `-1` ≈ 360°, `0` ≈ 180° front, higher values narrow the cone |
| `SuspicionDuration` | `10` | Seconds of looking after a hear before `onNoLongerSuspicious` |

### Events

| Event | When |
| --- | --- |
| `onHeardTarget` | Player enters the trigger **and** is outside the vision cone |
| `onSeenTarget` | Player is in range **and** inside the vision cone (once per aggro cycle) |
| `onNoLongerSuspicious` | Suspicion timer finishes without a successful sighting |

### Flow

1. **Player enters trigger** → store target, start `lookForTarget` (poll vision every `0.3s`).
2. If not already facing the player → fire `onHeardTarget` and start `suspicionDecay` (`SuspicionDuration`).
3. If facing the player (or later enters the cone) → fire `onSeenTarget`, set `hasSeenTarget`, cancel suspicion decay.
4. **Player exits trigger** → clear target / range. Looking stops. An in-flight suspicion timer can still finish and fire `onNoLongerSuspicious`.
5. Death sets an internal dead flag so further enter/exit sensing is ignored.

`ResetAggro()` clears `hasSeenTarget` so a future sighting can fire `onSeenTarget` again (called when pursuit ends).

Only objects that pass `isPlayer()` are considered.

## Wander (`EnemyWander`)

**File:** `Assets/Scripts/Enemy/EnemyWander.cs`

Default idle loop: wait, pick a NavMesh point near spawn, walk there, repeat.

### Fields

| Field | Default | Role |
| --- | --- | --- |
| `WanderRadius` | `15` | Sphere around spawn used to sample destinations |
| `WaitDuration` | `5` | Seconds idle between picks |

### Loop

1. Wait `WaitDuration`.
2. Sample `Random.insideUnitSphere * WanderRadius` around the position recorded in `Start`.
3. `NavMesh.SamplePosition` → if valid, `SetDestination`.
4. Poll every `0.5s` until within `0.5` of the destination **or** `20s` timeout.
5. Repeat until death.

### When it stops / starts

| Trigger | Effect |
| --- | --- |
| `onHeardTarget` | Stop wander coroutine, `ResetPath` |
| `onSeenTarget` | Same |
| `onNoLongerSuspicious` | Restart wander |
| `EnemyPursue.onTargetAbandoned` | Restart wander (intended) |
| Death | Stop permanently |

Wander stores spawn once at `Start`. Patrol always centers on that point, not the enemy’s current position after a chase.

## Turn to inspect (`EnemyTurnToInspect`)

**File:** `Assets/Scripts/Enemy/EnemyTurnToInspect.cs`

Response to **hearing** when the enemy has not yet seen the player: pause, then walk a short way “behind” itself so the agent turns around toward the noise direction.

### Fields

| Field | Default | Role |
| --- | --- | --- |
| `TurnToInspectDelay` | `5` | Delay before the turn move |

### Flow

1. `onHeardTarget` → start coroutine.
2. Wait `TurnToInspectDelay`.
3. Destination = current position + `(-forward * 10)`, snapped to NavMesh.
4. `SetDestination` and fire `onTurnAroundToInspect` (for anim / VFX listeners).

### Interruptions

Any of these cancel the turn coroutine and `ResetPath`:

- `onSeenTarget` (vision takes over → pursue)
- `onNoLongerSuspicious`
- `onTargetAbandoned`
- Death

So a hear that becomes a see aborts the inspect turn; pursuit owns the agent instead.

## Pursue (`EnemyPursue`)

**File:** `Assets/Scripts/Enemy/EnemyPursue.cs`

Starts only from `onSeenTarget`. Speeds up the agent and chases until the leash is broken or the enemy dies.

### Fields

| Field | Default | Role |
| --- | --- | --- |
| `DelayPursuit` | `2.5` | Wait before chase (aggro / roar window) |
| `PursuitSpeed` | `8` | Agent speed while chasing |
| `TargetDistance` | `10` | Stop distance; closer than this counts as “reached” |
| `LeashRadius` | `75` | Max distance from **spawn** while chasing |
| `DelayAfterReachingDestination` | `2` | Pause after reaching melee range (attack anim room) |

### Events

| Event | When |
| --- | --- |
| `onPursue` | First time chase movement starts; also when resuming chase after having reached the target |
| `onReachedTarget` | Distance ≤ `TargetDistance`; path cleared |
| `onChaseAfterReached` | Target moved outside `TargetDistance` again after a reach |
| `onTargetAbandoned` | Declared for leash / give-up; see note below |

### Chase loop

1. Wait `DelayPursuit`, set agent speed to `PursuitSpeed`.
2. While inside leash and alive, every `0.3s`:
   - If farther than `TargetDistance` → sample NavMesh near target, `SetDestination`, fire pursue events as needed.
   - If closer → `ResetPath`, fire `onReachedTarget` once, wait `DelayAfterReachingDestination`.
3. Leaving the leash (or exiting the loop while alive) calls `abandonTarget()` → reset aggro, clear path, restore original speed, stop chase coroutine.

Spawn position for the leash is recorded in `Start`, same idea as wander.

### Note: `onTargetAbandoned`

`onTargetAbandoned` is **subscribed to** by wander, turn-to-inspect, melee, and bear presentation scripts, but **`EnemyPursue` never invokes it** today. Leash give-up therefore resets pursuit state without notifying those listeners. Returning to wander after a chase currently relies on other paths (notably `onNoLongerSuspicious` after a hear-only encounter). Wiring `onTargetAbandoned?.Invoke()` from `abandonTarget()` would match the intended design.

## Melee attack (`EnemyMeleeAttack`)

**File:** `Assets/Scripts/Enemy/EnemyMeleeAttack.cs`

Does not deal damage itself. It raises `onMeleeAttack` on a timer while pursuit reports the target as reached. Hit detection / anim / SFX subscribe separately.

### Fields

| Field | Default | Role |
| --- | --- | --- |
| `TimeBetweenAttacks` | `3` | Delay between `onMeleeAttack` raises |

### Flow

1. `onReachedTarget` → start attack loop.
2. Loop: invoke `onMeleeAttack`, wait `TimeBetweenAttacks`, until stopped or dead.
3. Stop on `onNoLongerSuspicious`, `onChaseAfterReached`, `onTargetAbandoned`, or death.

When the player leaves melee range during pursuit, `onChaseAfterReached` stops attacks; if they close again, a new `onReachedTarget` restarts the loop.

## How the pieces interact

```
                    ┌─────────────────────────────────────┐
                    │              Enemy                   │
                    │  trigger enter → hear and/or see    │
                    └──────────────┬──────────────────────┘
           onHeardTarget           │            onSeenTarget
                 │                 │                 │
                 ▼                 │                 ▼
      ┌──────────────────┐         │      ┌──────────────────┐
      │ EnemyWander      │ stop    │      │ EnemyWander      │ stop
      │ EnemyTurnToInspect│ start  │      │ EnemyTurnToInspect│ interrupt
      └────────┬─────────┘         │      └──────────────────┘
               │                   │                 │
               │ (delay + turn)    │                 ▼
               │                   │      ┌──────────────────┐
               │                   │      │ EnemyPursue      │
               │                   │      │ delay → chase    │
               │                   │      └────────┬─────────┘
               │                   │               │
               │                   │     onReachedTarget
               │                   │               ▼
               │                   │      ┌──────────────────┐
               │                   │      │ EnemyMeleeAttack │
               │                   │      │ timed onMeleeAttack
               │                   │      └────────┬─────────┘
               │                   │               │
               │    onNoLongerSuspicious           │ onChaseAfterReached
               │    (suspicion timeout)            │ (target fled melee)
               ▼                   │               ▼
      restart wander               │      stop attacking; may resume chase
      interrupt turn               │
                                   │
                    (intended) onTargetAbandoned → restart wander,
                    stop attack / turn — not currently raised
```

### Scenario walkthroughs

**Idle patrol**  
Wander runs from `Start`. No player in trigger → keep picking points inside `WanderRadius`.

**Hear only**  
Player enters behind / outside the cone → wander stops, turn-to-inspect starts after delay, suspicion timer runs. If never seen before `SuspicionDuration` → `onNoLongerSuspicious` → interrupt turn, restart wander.

**Hear then see**  
Hear stops wander and schedules turn. Entering the vision cone fires `onSeenTarget` → interrupt turn, stop wander, pursue after `DelayPursuit`.

**See immediately**  
Player enters already in the cone → no hear path; pursue starts (wander still stops on `onSeenTarget`).

**In melee**  
Pursue reaches `TargetDistance` → `onReachedTarget` → melee loop. Player backs out → `onChaseAfterReached` stops melee; pursue continues. Close again → attack restarts.

**Death**  
`EnemyHealth.onEnemyDied` stops wander, pursue, turn, and attack on each component.

## Presentation hooks (example)

Bear (and similar) controllers do not own AI state; they mirror events:

| Event | Typical reaction |
| --- | --- |
| `onHeardTarget` | Attention anim / sound |
| `onSeenTarget` | Roar / aggro |
| `onMeleeAttack` | Melee attack trigger |
| `onPursue` | Chase sound |
| Agent velocity | Continuous walk blend |
| Lost-target events | Attention / calm-down |

`EnemyTurnToInspect.onTurnAroundToInspect` is available for a dedicated turn anim; the stock bear controller currently keys attention off hear / lose-target instead.

## Related scripts

| Path | Notes |
| --- | --- |
| `Assets/Scripts/Enemy/Enemy.cs` | Sensing hub |
| `Assets/Scripts/Enemy/EnemyWander.cs` | Patrol |
| `Assets/Scripts/Enemy/EnemyTurnToInspect.cs` | Hear response |
| `Assets/Scripts/Enemy/EnemyPursue.cs` | Chase / leash |
| `Assets/Scripts/Enemy/EnemyMeleeAttack.cs` | Attack cadence |
| `Assets/Scripts/Enemy/EnemyHealth.cs` | Death / hurt (stops AI) |
| `Assets/Enemies/NewAgeBear/Scripts/NewAgeBearAnimationController.cs` | Anim consumer |
| `Assets/Enemies/NewAgeBear/Scripts/NewAgeBearSoundController.cs` | Audio consumer |
