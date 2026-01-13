# GameFlow System - Complete Setup Package

## 🎮 Overview

This is a complete **dual-screen game flow system** for Political-Coop that manages the board (shared display) and tablets (player displays) through automatic scene loading based on game phases.

## ✨ Features

✅ **Additive Scene Loading** - Smooth transitions without reloading everything  
✅ **Phase-Based UI** - Automatically shows the right UI at the right time  
✅ **Dual-Screen Support** - Board and tablets show different content  
✅ **Network-Ready** - Fully integrated with Unity Netcode  
✅ **Automated Setup** - Helper scripts create all UI elements for you  
✅ **Debug Tools** - Keyboard shortcuts and detailed logging  

## 🚀 Quick Start (5 minutes)

### 1. Create GameFlow Scene
```
1. File → New Scene → Save as "GameFlow.unity"
2. Create Empty GameObject → Add "GameFlowSceneSetup" component
3. Right-click component → "Setup Scene"
4. Done! GameFlowManager is ready
```

### 2. Update Screens Scene
```
1. Open Screens.unity
2. Select BoardCanvas → Add "ScreensSceneSetup" component
3. Right-click → "Setup Board UI"
4. Select PlayerCanvas → Add "ScreensSceneSetup" component  
5. Right-click → "Setup Player UI"
6. Wire up button OnClick events (see console logs)
7. Assign references in Inspector
8. Done!
```

### 3. Configure Build Settings
```
Add these scenes in order:
0. GameFlow
1. Screens
2. CardLibraryUI
3. GameScreen
4. VotingScreen
5. GameBoard
```

### 4. Test It
```
1. Open GameFlow.unity
2. Press Play
3. Start as Host
4. Press 'N' key or click "Next Phase"
5. Watch phases transition automatically!
```

## 📚 Documentation

| File | Purpose |
|------|----------|
| **[QUICK_START_GUIDE.md](QUICK_START_GUIDE.md)** | 📘 Step-by-step setup with helper scripts |
| **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** | 📝 What was implemented and why |
| **[Assets/Scripts/GAMEFLOW_SETUP.md](Assets/Scripts/GAMEFLOW_SETUP.md)** | 🔧 Technical deep-dive |

## 📦 What's Included

### Core System Files
- **GameFlowManager.cs** - Main scene orchestrator
- **BoardUIController.cs** - Board display management (updated)
- **PlayerUIController.cs** - Tablet display management (updated)
- **BoardController.cs** - Board game logic (updated)
- **PlayerRoleController.cs** - Player game logic (updated)

### Helper Scripts (Auto-Setup)
- **GameFlowSceneSetup.cs** - Creates GameFlow scene structure
- **ScreensSceneSetup.cs** - Creates UI elements in Screens scene

### Documentation
- **QUICK_START_GUIDE.md** - Get running in 5 minutes
- **IMPLEMENTATION_SUMMARY.md** - Implementation overview
- **GAMEFLOW_SETUP.md** - Technical reference
- **README_GAMEFLOW.md** - This file

## 🎯 Game Flow

```
Lobby → Draw → Play → Vote → Resolve → (back to Draw)
```

### Phase Details

| Phase | Board Shows | Tablets Show | Loaded Scenes |
|-------|------------|--------------|---------------|
| **Lobby** | "Waiting for players..." | "Waiting..." | Screens |
| **Draw** | Waiting screen | 🎴 CardLibrary | Screens + CardLibraryUI |
| **Play** | Waiting screen | 🎴 GameScreen (hand) | Screens + GameScreen |
| **Vote** | 📺 GameBoard | ✅❌ Vote buttons | Screens + VotingScreen + GameBoard |
| **Resolve** | 📺 GameBoard (results) | 🏆 Results | Screens + GameBoard |

## ⌨️ Debug Controls

- **N key** (host): Advance to next phase
- **P key** (client): Play fake card during Play phase

## 🛠️ Architecture

```
GameFlow.unity (Persistent Manager)
  ├─ GameFlowManager
  ├─ NetworkManager
  └─ EventSystem

Screens.unity (Always Loaded)
  ├─ BoardCanvas
  │   ├─ WaitingPanel
  │   └─ NextPhaseButton
  └─ PlayerCanvas
      ├─ VoteButtonsPanel
      └─ ResultsPanel

Phase-Specific Scenes (Load/Unload)
  ├─ CardLibraryUI.unity (Draw)
  ├─ GameScreen.unity (Play)
  ├─ VotingScreen.unity (Vote)
  └─ GameBoard.unity (Vote/Resolve)
```

## 🐛 Troubleshooting

**Scene not loading?**
- Check scene names in GameFlowManager Inspector
- Verify scenes are in Build Settings
- Check Console for "Scene not found" errors

**UI not updating?**
- Verify references assigned in Inspector
- Check Console for "LocalInstance is null"
- Ensure EventSystem exists

**Button not working?**
- Check Button OnClick event is wired up
- Verify target controller exists in scene
- Check Console for click logs

## 📋 Next Steps

After setup is complete:

1. ✅ Test basic phase transitions
2. ✅ Test with multiple clients
3. 🔨 Implement card selection in CardLibraryUI
4. 🔨 Implement card hand in GameScreen
5. 🔨 Enhance voting UI
6. 🔨 Add visual effects and transitions
7. 🔨 Polish waiting screens

## 📌 Branch Information

All changes are in the **`feature-tabletsceneflow`** branch.

## 👥 Credits

Implemented for Political-Coop game with:
- Unity 2022.3+
- Unity Netcode for GameObjects
- TextMeshPro

## 🔗 Quick Links

- [Start Here: Quick Start Guide](QUICK_START_GUIDE.md)
- [What Was Done: Implementation Summary](IMPLEMENTATION_SUMMARY.md)
- [Technical Details: GameFlow Setup](Assets/Scripts/GAMEFLOW_SETUP.md)

---

**Ready to start?** Open **[QUICK_START_GUIDE.md](QUICK_START_GUIDE.md)** and follow the steps!
