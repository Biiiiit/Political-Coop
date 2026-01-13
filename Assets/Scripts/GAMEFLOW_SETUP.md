# GameFlow Scene Architecture - Setup Guide

## Overview

The GameFlow system uses **additive scene loading** to manage the game's phases. The `GameFlowManager` in the GameFlow.unity scene orchestrates loading and unloading UI scenes based on the current game phase.

## Architecture

```
GameFlow.unity (Persistent Manager Scene)
├── Loads additively based on phase:
│   ├── Screens.unity (Always loaded - BoardCanvas + PlayerCanvas base)
│   ├── CardLibraryUI.unity (Loaded during Draw phase)
│   ├── GameScreen.unity (Loaded during Play phase)
│   ├── VotingScreen.unity (Loaded during Vote phase)
│   └── GameBoard.unity (Loaded during Vote/Resolve phases)
```

## Game Phases and Scene Mapping

| Phase | Board Display | Player Display | Loaded Scenes |
|-------|--------------|----------------|---------------|
| **Lobby** | Waiting screen "Waiting for players..." | Waiting message | Screens |
| **Draw** | Waiting screen "Players selecting cards..." | CardLibrary UI | Screens + CardLibraryUI |
| **Play** | Waiting screen "Players playing cards..." | GameScreen UI | Screens + GameScreen |
| **Vote** | GameBoard with played cards | Vote Yes/No buttons | Screens + VotingScreen + GameBoard |
| **Resolve** | GameBoard with results | Results panel | Screens + GameBoard |

## Setup Instructions

### Step 1: Create GameFlow Scene

1. Create a new scene: **Assets/Scenes/GameFlow.unity**
2. Add the following GameObjects:
   - **GameFlowManager** (empty GameObject)
     - Add `GameFlowManager.cs` component
     - Configure scene names in Inspector:
       - Screens Scene Name: `Screens`
       - Card Library Scene Name: `CardLibraryUI`
       - Game Screen Scene Name: `GameScreen`
       - Voting Screen Scene Name: `VotingScreen`
       - Game Board Scene Name: `GameBoard`
   - **NetworkManager** (if not already DontDestroyOnLoad)
   - **EventSystem** (for UI interactions)

### Step 2: Configure Screens.unity

In **Screens.unity**, add these panels to the BoardCanvas:

1. **WaitingPanel** (child of BoardCanvas)
   - Add TextMeshPro component: **WaitingMessageText**
   - This displays waiting messages during Lobby, Draw, and Play phases

2. **NextPhaseButton** (child of BoardCanvas)
   - Button component with `OnClick()` → `BoardUIController.OnNextPhaseButtonClicked()`

Assign references in **BoardUIController**:
- `waitingPanel` → WaitingPanel GameObject
- `waitingMessageText` → WaitingMessageText component
- `nextPhaseButton` → NextPhaseButton component

In **PlayerCanvas**, add these panels:

1. **VoteButtonsPanel** (child of PlayerCanvas)
   - Contains Vote Yes and Vote No buttons
   - Initially inactive

2. **ResultsPanel** (child of PlayerCanvas)
   - Displays results after Resolve phase
   - Initially inactive

Assign references in **PlayerUIController**:
- `voteButtonsPanel` → VoteButtonsPanel GameObject
- `resultsPanel` → ResultsPanel GameObject
- `voteYesButton` → Vote Yes Button
- `voteNoButton` → Vote No Button

### Step 3: Configure Individual Scene UIs

#### CardLibraryUI.unity (Draw Phase)
- Contains card library browsing UI
- Players select cards to add to their hand
- Should have a "Confirm Selection" button

#### GameScreen.unity (Play Phase)
- Shows player's hand of cards
- Has "Play Card" button
- Button should call `PlayerUIController.OnPlayCardButtonClicked()`

#### VotingScreen.unity (Vote Phase)
- Tablet-specific voting interface
- Can show card details being voted on
- Buttons route to `PlayerUIController.OnVoteYesButtonClicked()` / `OnVoteNoButtonClicked()`

#### GameBoard.unity (Vote/Resolve Phases)
- Large shared screen display
- Shows all played cards during Vote
- Shows results (accepted/rejected) during Resolve

### Step 4: Build Settings

Add scenes to **File → Build Settings** in this order:
```
0. GameFlow
1. Screens
2. CardLibraryUI
3. GameScreen
4. VotingScreen
5. GameBoard
```

### Step 5: Testing

1. **Start from GameFlow.unity**
2. GameFlowManager will automatically load Screens.unity
3. Start a multiplayer session (host + clients)
4. Press the "Next Phase" button or press 'N' key (debug) to advance phases
5. Watch scenes load/unload in the Hierarchy during phase transitions

## Code Integration

### How Phase Changes Work

1. **Host clicks Next Phase button** or game logic triggers phase change
2. `GameManager.NextPhaseServerRpc()` is called
3. GameManager transitions to new phase and broadcasts via ClientRpc
4. `BoardController.OnBoardStateUpdated()` is called on host
5. `PlayerRoleController.OnBoardStateUpdated()` is called on clients
6. Both controllers call `GameFlowManager.OnPhaseChanged(newPhase)`
7. GameFlowManager unloads old scenes and loads new scenes
8. GameFlowManager configures UI for the new phase

### Key Methods

**GameFlowManager.cs:**
- `OnPhaseChanged(Phase newPhase)` - Triggered when phase changes
- `OnNextPhaseButtonClicked()` - Forwards to GameManager
- `OnPlayerPlayCard(string cardId)` - Forwards to GameManager
- `OnPlayerVote(string cardId, bool voteYes)` - Forwards to GameManager

**BoardUIController.cs:**
- `ShowWaitingScreen(string message)` - Shows waiting panel
- `HideWaitingScreen()` - Hides waiting panel
- `EnableNextPhase(bool enabled)` - Enables/disables Next Phase button

**PlayerUIController.cs:**
- `HideAllPhaseUI()` - Hides all phase-specific UI
- `ShowVoteButtons(bool show)` - Shows/hides voting buttons
- `ShowResults()` - Shows results panel
- `OnCardsToVoteOn(string cardsCombined)` - Receives cards to vote on

## Debugging

### Debug Logs
All components log phase transitions:
```
[GameFlowManager] Phase changing from Draw to Play
[GameFlowManager] Loading scene: GameScreen
[GameFlowManager] Scene loaded: GameScreen
[GameFlowManager] Configuring Play UI
[GameFlowManager] Phase transition complete: Play
```

### Debug Keys
- **N key** (on board/host): Advance to next phase
- **P key** (on client/tablet): Play a fake card during Play phase

### Common Issues

**Scene not loading:**
- Check scene name matches exactly in GameFlowManager Inspector
- Verify scene is added to Build Settings
- Check console for "Scene not found" warnings

**UI not updating:**
- Verify BoardUIController.LocalInstance and PlayerUIController.LocalInstance are set
- Check that panel references are assigned in Inspector
- Look for "Instance not found" warnings in console

**Phase not advancing:**
- Ensure GameManager.Instance exists
- Check NetworkManager is connected (IsServer/IsClient)
- Verify NextPhaseButton OnClick is wired correctly

## Next Steps

1. **Implement card hand UI** in GameScreen.unity
2. **Implement card selection** in CardLibraryUI.unity
3. **Implement voting display** to show multiple cards
4. **Add visual feedback** for phase transitions
5. **Implement results display** with card effects

## File References

- `Assets/Scripts/GameFlowManager.cs` - Main flow controller
- `Assets/Scripts/Game/BoardUIController.cs` - Board UI management
- `Assets/Scripts/Game/PlayerUIController.cs` - Player UI management
- `Assets/Scripts/Game/BoardController.cs` - Board game logic
- `Assets/Scripts/Game/PlayerRoleController.cs` - Player game logic
- `Assets/Scripts/Game/GameManager.cs` - Core game state management
- `Assets/Scripts/Game/Phase.cs` - Phase enum definition
