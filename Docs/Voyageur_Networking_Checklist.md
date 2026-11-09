# Voyageur — Networking Status Checklist

> Living tracker for the migration described in [`Voyageur_Multiplayer_Architecture.md`](./Voyageur_Multiplayer_Architecture.md).
> Update statuses as work lands. Do not mark items done without Editor verification.

**Last audited:** 2026-11-08  
**Current focus:** Phase 0A/0B verified with `NetTestTwoPlayers` (Host + Editor clone Client on Lukes Scene). Next: Phase 1 per-player gameplay.

### Status legend

| Mark | Meaning |
|------|---------|
| `[x]` | Implemented and verified in Editor |
| `[~]` | Partial / stubbed / exists but incomplete or unproven |
| `[ ]` | Not started |
| `[!]` | Blocked or known broken |

---

## Snapshot

| Phase | Goal | Status |
|-------|------|--------|
| **0A** | Networked skeleton (packages, bootstrap, spawn, movement) | **Mostly done** |
| **0B** | Server/client separation + debug tooling | **Mostly done** |
| **1** | Per-player inventory / craft / harvest authority | **Not started** |
| **2** | Authoritative shared world (build / loot / AI / resources) | **Early partial** (building place spawn only) |
| **3** | Local JSON persistence | **Not started** |
| **4** | Hardening / dedicated server / AWS readiness | **Not started** |

### What works today

- NGO + Unity Transport + Multiplayer Play Mode packages installed.
- `NetworkBootstrap` starts **Host** (Editor), **Client** (MPPM / `-voyageur-role`), or **Server**.
- Minimal `NetworkPlayer` with owner camera/audio and server-authoritative movement.
- Scene FPSController disabled when networking listens.
- Building **placement spawn** via `RequestPlaceServerRpc` + `NetworkBuildingCatalog` (distance check only; no inventory cost / validation).

### What does **not** work yet (multiplayer)

- Per-player inventory, hotbar, crafting, harvesting.
- Containers, loot, trees/resources, enemy AI, combat authority.
- Stable persistent IDs or any save/load of world/player state.
- Reconnect, interest management, dedicated-server build validation.

---

## Phase 0A — Networked skeleton

**Done when:** Play in Editor starts Host; a second MPPM client joins and both see each other; no standalone build required.

- [x] Audit Unity version / packages / project structure
- [x] Add **Netcode for GameObjects** (`com.unity.netcode.gameobjects` 2.13.3)
- [x] Add **Unity Transport** (pulled via NGO → `com.unity.transport` 2.6.0)
- [x] Add **Multiplayer Play Mode** (`com.unity.multiplayer.playmode` 2.0.2)
- [x] Central role-aware bootstrap (`NetworkBootstrap`) — Host / Client / Server
- [x] Configurable address + UDP port (default `127.0.0.1:7777`)
- [x] Avoid duplicate auto-start / role resolve for MPPM virtual players (`IsMainEditor` / `-voyageur-role`)
- [x] Minimal networked player prefab (`Assets/Prefabs/Networking/NetworkPlayer.prefab`)
- [x] Network scene(s) with `NetworkManager` (`NetTest.unity`, wired into `Lukes Scene.unity`)
- [x] Server-authoritative movement path (`NetworkPlayerMovement` + ServerRpc intents)
- [x] Owner-only input / camera / audio (`NetworkPlayerOwnerSetup`)
- [x] Visible other players (NetworkObject + NetworkTransform path)
- [x] Spawn points away from terrain origin (`NetworkPlayerSpawnPoints`)
- [x] Disable scene single-player FPS avatar when networked (`DisableSceneFpsWhenNetworked`)
- [x] Two-player MPPM join/observe verified end-to-end (`NetTestTwoPlayers`: Host + clone Client, both see world + spawned players)

**Key files**

- `Assets/Scripts/Networking/NetworkBootstrap.cs`
- `Assets/Scripts/Networking/NetworkRuntimeRole.cs`
- `Assets/Scripts/Networking/NetworkPlayerMovement.cs`
- `Assets/Scripts/Networking/NetworkPlayerOwnerSetup.cs`
- `Assets/Scripts/Networking/NetworkPlayerSpawnPoints.cs`
- `Assets/Scripts/Networking/DisableSceneFpsWhenNetworked.cs`
- `Assets/Prefabs/Networking/NetworkPlayer.prefab`

---

## Phase 0B — Server/client separation and debugging

**Done when:** same network scene works as Host+Client and Server+Client without separate gameplay implementations.

- [x] Authoritative sim uses `IsServer` (not Host-only shortcuts) in networked movement / building spawn
- [x] Role / client / ownership logs (`NetworkBootstrap`, `NetworkPlayerOwnerSetup`)
- [x] Explicit mode overrides (debug OnGUI Host/Client/Server + CLI `-voyageur-role`)
- [~] Server-only mode smoke-tested with a connecting client *(bootstrap supports it; confirm in Editor)*
- [ ] Dedicated checklist of Host-masked bugs regularly exercised with a remote client
- [ ] Automated or scripted smoke tests for join/leave, two-player spawn, movement visibility

---

## Phase 1 — Per-player gameplay

**Done when:** server-owned inventory / crafting / harvesting behaves consistently for both players with no cross-player leakage.

### Spawn / ownership

- [x] Network player spawn / despawn via NGO player prefab
- [x] Input ownership for movement / camera
- [ ] Health / vitals as per-player authoritative state (not globals)

### Inventory / hotbar / crafting

- [ ] Migrate `PlayerInventory` / hotbar off process-global assumptions
- [ ] Migrate `PlayerItems` / `GameState` player runtime state to per-player server state
- [ ] Crafting intent → ServerRpc validation → authoritative apply
- [ ] Hotbar / equip ownership correct for local owner only
- [ ] Two players: distinct inventories, no leakage

### Harvesting

- [ ] `TreeHarvestController` / tool harvest as client **intent**, server validates damage/loot
- [ ] Server grants items; clients never invent inventory deltas
- [ ] Terrain tree / harvestable state strategy decided (stable IDs vs networked prefabs)

### Validation tests

- [ ] Join / leave with inventory intact for remaining player
- [ ] Two-player inventory isolation
- [ ] Server rejects invalid craft / harvest claims

**Still single-player (inspect / migrate):**

- `Assets/Scripts/Globals/Events.cs`, `GameState.cs`
- `Assets/Scripts/Player/PlayerItems.cs`
- `Assets/Scripts/PlayerUI/PlayerInventory.cs` (+ crafting / hotbar UI)
- `Assets/Scripts/Gathering/TreeHarvestController.cs`

---

## Phase 2 — Authoritative shared world

**Done when:** either player can mutate a shared world, both see correct results, server remains authoritative.

### Building *(started early)*

- [x] Client placement intent → `NetworkPlayerBuilding.RequestPlaceServerRpc`
- [x] Server spawns networked placables (`NetworkObject` on Wall / Door / etc.)
- [x] Prefab catalog (`NetworkBuildingCatalog`)
- [x] Owner building-rig camera bind (`NetworkBuildingRigBinder`)
- [~] Distance validation only — **no material cost, recipe rules, or inventory deduction**
- [ ] Building damage / repair / removal authority
- [ ] Stable `persistentId` on buildings (not `NetworkObjectId`)

### World systems

- [ ] Containers (open / transfer authoritative)
- [ ] Dropped loot
- [ ] Resources / terrain-tree harvest state (deltas, not full terrain sync)
- [x] Enemy AI server-only simulation + pose replication (`NetworkEnemyAuthority` + `NetworkTransform`)
- [x] Combat outcomes server-authoritative (damage ServerRpc + synced health/death)
- [x] Enemy FX replicated (attention/roar/melee/hurt/death ClientRpcs + client ragdoll)
- [x] NetworkPlayer on Player layer so DetectPlayer aggro works
- [ ] Stable persistent IDs for saveable world objects

---

## Phase 3 — Local persistence

**Done when:** stop/restart Play and world/player changes reload from local files within documented guarantees.

- [ ] Versioned DTOs (`WorldSaveData`, `PlayerSaveData`, `BuildingSaveData`, …)
- [ ] `IWorldPersistence` interface
- [ ] `WorldPersistenceService` (server-only coordinator)
- [ ] `JsonWorldPersistence` under `Application.persistentDataPath`
- [ ] Optional `InMemoryWorldPersistence` for tests
- [ ] Load world before accepting players; checkpoint / flush on shutdown
- [ ] Safe file replace + schema version + error handling
- [ ] Document acceptable crash recovery window
- [ ] **Do not** implement `ApiWorldPersistence` / Postgres / Redis yet

---

## Phase 4 — Hardening and deployment readiness

**Done when:** standalone server runs the same simulation and load/save model without a second gameplay implementation.

- [ ] Reconnect / rejoin
- [ ] Admin / debug operations
- [ ] Broader RPC validation hardening
- [ ] Relevance / performance pass targeting ~12–24 players
- [ ] Local headless Linux Dedicated Server build smoke test
- [ ] EC2 / AWS deploy *(later, separate)*
- [ ] `ApiWorldPersistence` + Postgres *(only when needed)*
- [ ] Redis *(only for a concrete cross-process need)*

---

## Explicit non-goals (do not start yet)

Per architecture §1 / §6 — leave unchecked on purpose:

- [ ] AWS deploy / CI/CD for multiplayer servers
- [ ] PostgreSQL schema / Node persistence API
- [ ] Redis
- [ ] Matchmaking / join codes / accounts / cross-world transfers / sharding
- [ ] Full anti-cheat / complex interest management without profiling

---

## Suggested next increments

1. **Re-verify 0A/0B:** Editor Host + one MPPM Client — spawn, move, see each other; then Server + Client once.
2. **Phase 1 spike:** per-player inventory ownership + one harvest or craft ServerRpc round-trip.
3. **Finish building authority:** deduct materials / validate placement rules on server (close the Phase 2 building gap).
4. Only then expand to containers / AI / trees and Phase 3 JSON saves.

---

## How to update this file

When finishing a milestone:

1. Flip `[ ]` → `[x]` (or `[~]` if partial).
2. Bump **Last audited** date and **Current focus**.
3. Note any Editor test steps that actually passed (scene name, Host/Client/Server).
4. Keep claims honest — code present ≠ verified multiplayer behavior.
