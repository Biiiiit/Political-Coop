# GameFlow Visual Overview

## 🎭 System Architecture

```
╭───────────────────────────────────────────╮
│       GameFlow.unity (Manager Scene)       │
│                                           │
│  ╭─────────────────────────────────╮  │
│  │    GameFlowManager Component    │  │
│  │  - Loads/Unloads scenes       │  │
│  │  - Manages phase transitions  │  │
│  │  - Configures UI per phase    │  │
│  ╰─────────────────────────────────╯  │
╰───────────────────────────────────────────╯
           │
           │ Loads Additively
           │
           ↓
╭───────────────────────────────────────────╮
│   Screens.unity (Always Loaded Base)    │
│                                           │
│  ╭────────────╮        ╭──────────────╮  │
│  │ BoardCanvas │        │ PlayerCanvas │  │
│  │  (Host)     │        │  (Tablets)   │  │
│  ╰────────────╯        ╰──────────────╯  │
╰───────────────────────────────────────────╯
           │
           │ Phase-Based Loading
           │
     ┌─────┼─────┐
     │     │     │
     ↓     ↓     ↓
╭────────╮ ╭────────╮ ╭────────╮
│CardLib │ │GameScr │ │GameBrd │
│ (Draw) │ │ (Play) │ │(Vote)  │
╰────────╯ ╰────────╯ ╰────────╯
```

## 🔄 Phase Flow Diagram

```
              START GAME
                  │
                  ↓
         ┌────────────────┐
         │  LOBBY PHASE   │
         │  Board: Wait   │
         │  Tablets: Wait │
         └───────┬────────┘
                │ [Next Phase]
                ↓
         ┌───────────────────────╮
    ╭───┤  DRAW PHASE          ├───╮
    │   │  Load: CardLibraryUI │   │
    │   │  Board: "Selecting" │   │
    │   │  Tablets: CardLib   │   │
    │   ╰───────┬─────────────╯   │
    │          │ [Next Phase]       │
    │          ↓                    │
    │   ┌────────────────────┐   │
    │   │  PLAY PHASE        │   │
    │   │  Load: GameScreen  │   │
    │   │  Board: "Playing" │   │  TURN
    │   │  Tablets: Hand     │   │  LOOP
    │   └──────┬─────────────┘   │
    │          │ [All played]       │
    │          ↓                    │
    │   ┌────────────────────┐   │
    │   │  VOTE PHASE        │   │
    │   │  Load: GameBoard   │   │
    │   │  Board: Cards      │   │
    │   │  Tablets: Vote UI  │   │
    │   └──────┬─────────────┘   │
    │          │ [All voted]        │
    │          ↓                    │
    │   ┌────────────────────┐   │
    │   │  RESOLVE PHASE     │   │
    │   │  Keep: GameBoard   │   │
    │   │  Board: Results    │   │
    │   │  Tablets: Results  │   │
    │   └──────┬─────────────┘   │
    │          │ [Next Phase]       │
    │          ↓                    │
    │     Turn++, Crisis++            │
    │          │                    │
    ╰──────────┼──────────────────╯
               │
            [Loop Back]
```

## 🎮 Setup Workflow

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃   1. CREATE GAMEFLOW    ┃
┃                          ┃
┃  • New Scene             ┃
┃  • Add Setup Helper     ┃
┃  • Click "Setup Scene"  ┃
┃  • DONE! ✅              ┃
┗━━━━━━━━━┬━━━━━━━━━━━━━━━┛
         │
         ↓
┏━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃  2. UPDATE SCREENS UI  ┃
┃                          ┃
┃  BoardCanvas:           ┃
┃  • Add Setup Helper     ┃
┃  • "Setup Board UI"    ┃
┃  • Wire button          ┃
┃  • Assign refs         ┃
┃                          ┃
┃  PlayerCanvas:          ┃
┃  • Add Setup Helper     ┃
┃  • "Setup Player UI"   ┃
┃  • Wire buttons        ┃
┃  • Assign refs         ┃
┃  • DONE! ✅              ┃
┗━━━━━━━━━┬━━━━━━━━━━━━━━━┛
         │
         ↓
┏━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃ 3. BUILD SETTINGS     ┃
┃                          ┃
┃  • Add all scenes       ┃
┃  • Correct order       ┃
┃  • DONE! ✅              ┃
┗━━━━━━━━━┬━━━━━━━━━━━━━━━┛
         │
         ↓
┏━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃    4. TEST & PLAY!      ┃
┃                          ┃
┃  • Open GameFlow        ┃
┃  • Press Play          ┃
┃  • Start Host          ┃
┃  • Click Next Phase    ┃
┃  • Watch magic happen! ┃
┃  • DONE! 🎉              ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━┛

     Total Time: ~5 min
```

## 📊 Data Flow

```
        User Action
            │
            ↓
    ┌────────────────┐
    │  GameManager   │ (Server)
    │  Phase Logic   │
    └───────┬────────┘
           │
           │ ClientRpc
           │
    ┌──────┼──────┐
    │             │
    ↓             ↓
┌─────────╮   ╭───────────────┐
│  Board   │   │ PlayerRole      │
│Controller│   │ Controller      │
╰────┬────╯   ╰────┬───────────╯
     │              │
     │ Notify       │ Notify
     │              │
     └─────┬───────┘
           │
           ↓
    ┌────────────────────┐
    │ GameFlowManager  │
    │ OnPhaseChanged() │
    └─────────┬──────────┘
              │
              │ Load/Unload
              │ Configure UI
              │
    ┌─────────┼─────────┐
    │                  │
    ↓                  ↓
┌────────╮        ╭──────────┐
│ Scenes │        │ UI Config │
│ Load   │        │ Show/Hide │
╰────────╯        ╰──────────╯
```

## 🗂️ File Structure

```
Political-Coop/
├── Assets/
│   ├── Scenes/
│   │   ├── GameFlow.unity ⭐ (NEW - Manager)
│   │   ├── Screens.unity ✏️ (Update with UI)
│   │   ├── CardLibraryUI.unity (Draw phase)
│   │   ├── GameScreen.unity (Play phase)
│   │   ├── VotingScreen.unity (Vote phase)
│   │   └── GameBoard.unity (Vote/Resolve)
│   │
│   └── Scripts/
│       ├── GameFlowManager.cs ⭐ (NEW)
│       ├── GameFlowSceneSetup.cs ⭐ (Helper)
│       ├── ScreensSceneSetup.cs ⭐ (Helper)
│       │
│       └── Game/
│           ├── GameManager.cs (Core - unchanged)
│           ├── BoardController.cs ✏️ (Updated)
│           ├── BoardUIController.cs ✏️ (Updated)
│           ├── PlayerRoleController.cs ✏️ (Updated)
│           └── PlayerUIController.cs ✏️ (Updated)
│
├── README_GAMEFLOW.md ⭐ (Main doc)
├── QUICK_START_GUIDE.md ⭐ (Setup guide)
├── IMPLEMENTATION_SUMMARY.md ⭐ (What's done)
├── COMMIT_SUMMARY.md ⭐ (Deliverables)
└── GAMEFLOW_VISUAL_OVERVIEW.md ⭐ (This file)

Legend:
⭐ = New file
✏️ = Modified file
```

## 🛠️ What Gets Created by Helper Scripts

### GameFlowSceneSetup creates:
```
GameFlow.unity
  ├─ GameFlowManager ✅
  ├─ EventSystem ✅
  └─ (You add: NetworkManager)
```

### ScreensSceneSetup creates on BoardCanvas:
```
BoardCanvas
  ├─ WaitingPanel ✅
  │  └─ WaitingMessageText ✅
  └─ NextPhaseButton ✅
     └─ Text ✅
```

### ScreensSceneSetup creates on PlayerCanvas:
```
PlayerCanvas
  ├─ VoteButtonsPanel ✅
  │  ├─ VoteYesButton ✅
  │  │  └─ Text ✅
  │  └─ VoteNoButton ✅
  │     └─ Text ✅
  └─ ResultsPanel ✅
     └─ ResultsText ✅
```

## ⏱️ Time Estimates

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃ Step 1: GameFlow scene    2 min ┃
┃ Step 2: Screens UI        3 min ┃
┃ Step 3: Build Settings    1 min ┃
┃ Step 4: Test             <1 min ┃
┃                                 ┃
┃ TOTAL:               ~5-7 min ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

## 👍 What You Get

```
✅ Complete phase management system
✅ Automatic scene loading/unloading
✅ Dual-screen coordination
✅ Network-ready integration
✅ Helper scripts for setup
✅ Comprehensive documentation
✅ Debug tools & logging
✅ Ready to extend & customize
```

---

**➡️ Ready to start? [Open QUICK_START_GUIDE.md](QUICK_START_GUIDE.md)**
