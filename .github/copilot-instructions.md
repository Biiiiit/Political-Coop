# Political-Coop AI Coding Instructions

## Project Overview
Political-Coop is a multiplayer cooperative board game (Unity) where players manage risks and disasters across four sectors (Farming, Industry, Housing, Nature). Uses **Unity Netcode for multiplayer** (board as host, tablets as clients).

## Architecture

### Core Game Flow: Five-Phase System
Game progresses through phases in `Phase.cs` enum: **Lobby → Draw → Play → Vote → Resolve**

- **GameManager** ([Game/GameManager.cs](../Assets/Scripts/Game/GameManager.cs)) is the central singleton managing:
  - Player roles and assignments (1 of 4 roles per player)
  - Turn state and phase transitions
  - Card plays and voting across all players
  - NetworkBehaviour synced via Netcode
- **BoardState** ([Game/BoardState.cs](../Assets/Scripts/Game/BoardState.cs)) tracks: TurnNumber, CrisisLevel, Phase

### Key Subsystems

**Risk/Disaster System** ([RiskDisasterSystem/](../Assets/Scripts/RiskDisasterSystem/))
- **Risk** (ScriptableObject): Base risk unit with `baseProbability`, `escalationRate`, `severity`, linked `disasterTags`
- **RiskManager**: Tracks active risks, calculates triggered risks based on turn (probability increases each turn)
- **RiskLibrary**: Singleton registry—loads all risks from `Resources/Risks/` at startup
- **Assigned risks**: Component on cards; holds list of risks that risk adds when played

**UI Card System**
- **ProjectCardUI** + **Assignedrisks** component: Cards display risk icons in a grid via **ImageMaskController** (UI masking)
- **CardCreationController**: Manages card preview and risk selection workflow
- **RiskSelectionPanel**: Populates risk buttons dynamically from `RiskLibrary.Instance.allRisks`

**Voting System** ([VotingSystem/](../Assets/Scripts/VotingSystem/))
- Simple timer + manager (files minimal—likely WIP)

**Board System** ([Game/](../Assets/Scripts/Game/))
- **BoardController**: Main scene orchestrator
- **BoardUIController**: Synchronized UI rendering
- **PlayerRoleController**: Maps client IDs to roles

## Critical Data Patterns

### Roles (Enum-based)
```csharp
public enum Role { None = 0, Farming = 1, Industry = 2, Housing = 3, Nature = 4 }
```
Roles link players to sector-specific risks. GameManager stores `Dictionary<ulong, Role>`.

### Risk Probability Model
```
triggerChance = baseProbability + (escalationRate * currentTurn)
```
Risks become more likely each turn—not random, but deterministic per seed. Consider for testing.

### Card Flow
1. **Creation**: Select risks from RiskLibrary → store in `Assignedrisks` component
2. **Play**: Player plays card (adds risks to board)
3. **Voting**: Other players vote yes/no
4. **Resolution**: Apply risk effects (cards generate disaster events)

## Development Workflow

### Building & Running
- **Unity scene**: Open `Assets/Scenes/` (primary game scenes not listed in workspace—check ProjectSettings/EditorBuildSettings.asset)
- **Multiplayer testing**: 
  - Build 1 as HOST (NetworkBootstrap.StartAsHost=true)
  - Build N as CLIENTs (StartAsHost=false)
  - Verify Netcode syncing via NetworkBehaviour RPC/var changes

### Testing Structure
- **PlayMode tests**: [Assets/Tests/PlayMode/](../Assets/Tests/PlayMode/) — integration tests with real GameManager
- **EditMode tests**: [Assets/Tests/EditMode/](../Assets/Tests/EditMode/) — unit tests (RiskManager probability, card logic)
- Run via Unity Test Framework in editor

### Assembly Definitions
- **RiskDisasterSystem.asmdef**: Separate assembly for Risk subsystem (isolated unit testing)
- Main **Assembly-CSharp.csproj**: Game logic + UI
- **Tests.csproj**: Uses standard NUnit

## Naming & Code Conventions

1. **UI Components**: Suffix with `UI` (e.g., `RiskIconUI`, `ProjectCardUI`)
2. **Managers**: Singleton pattern with `public static Instance { get; private set; }`
3. **ScriptableObjects**: Used for data (Risk assets)—load via `Resources.LoadAll<T>("FolderName")`
4. **Network Sync**: GameManager extends `NetworkBehaviour`; use `[NetworkSerialize]` for lists, `[Rpc]` for remote calls
5. **Nullable checks**: Always null-check before using (see `ImageMaskController.SetImage()` pattern)

## Common Tasks

### Add a New Risk Type
1. Create Risk.cs ScriptableObject in Editor: `Assets/Resources/Risks/NewRisk.asset`
2. Set `baseProbability`, `escalationRate`, icon, category
3. Automatically loaded by `RiskLibrary.Awake()` from Resources

### Update Card Risk Display
- Modify `ProjectCardUI.DisplayRiskIcons()` to iterate `assignedrisks.GetRisks()`
- Ensure `ImageMaskController.SetImage()` is called per risk icon prefab

### Add New Game Phase Logic
1. Add phase to `Phase` enum
2. Update `GameManager.PhaseTransition()` logic (RPC to all clients)
3. Add UI state in `BoardUIController.UpdatePhaseDisplay()`

### Network Debugging
- Check `NetworkManager.Singleton.IsServer` / `.IsClient` in conditional logic
- Use `Debug.Log()` with `[NetworkBootstrap]` prefix for remote execution traces
- Verify `NetworkBehaviour` ownership via `IsOwner` before RPC writes

## File Structure Quick Reference
- **Game logic**: `Assets/Scripts/Game/` (GameManager, BoardState, Phases)
- **Risk data**: `Assets/Scripts/RiskDisasterSystem/` (Risk, RiskManager, RiskLibrary)
- **UI**: `Assets/Scripts/` (CardCreationController, RiskSelectionPanel, ProjectCardUI)
- **Config/Settings**: `ProjectSettings/`, `Assets/Settings/`, `Assets/Resources/`
- **Prefabs**: `Assets/Prefabs/` (cards, UI panels, risk icons)

## Dependencies & External Tools
- **Unity Netcode for GameObjects** (v1.x)—review `NetworkBootstrap.cs` for initialization patterns
- **TextMesh Pro** (UI text)
- **Unity UI** (Canvas, buttons, masks)
- **Input System** (InputSystem_Actions.inputactions)

---

**Last Updated**: January 2026  
For questions on multiplayer sync, check GameManager RPCs; for UI patterns, see ProjectCardUI + ImageMaskController.
