# GameFlow Quick Start Guide

## Automated Setup with Helper Scripts

I've created helper scripts to automatically set up the GameFlow architecture. Follow these steps:

---

## Step 1: Create GameFlow Scene

1. **In Unity, create a new scene:**
   - File → New Scene
   - Save as `Assets/Scenes/GameFlow.unity`

2. **Add the setup helper:**
   - Create an empty GameObject (Right-click in Hierarchy → Create Empty)
   - Name it "SceneSetupHelper"
   - Add Component → `GameFlowSceneSetup`

3. **Run automatic setup:**
   - In Inspector, find the `GameFlowSceneSetup` component
   - Right-click on the component header → **"Setup Scene"**
   - OR click the three dots menu → **"Setup Scene"**

4. **What gets created:**
   - ✅ GameFlowManager GameObject with GameFlowManager component
   - ✅ EventSystem (if not present)
   - ✅ Scene names pre-configured

5. **Manual steps:**
   - Ensure **NetworkManager** prefab/GameObject is in the scene
   - If not, drag your NetworkManager prefab into the scene
   - Save the scene (Ctrl+S / Cmd+S)

6. **Clean up (optional):**
   - You can delete the "SceneSetupHelper" GameObject after setup is complete

---

## Step 2: Update Screens Scene

### For BoardCanvas (Host/Board Display):

1. **Open Screens.unity**
2. **Find BoardCanvas in Hierarchy**
3. **Add the setup helper:**
   - Select BoardCanvas
   - Add Component → `ScreensSceneSetup`
4. **Run automatic setup:**
   - Right-click on `ScreensSceneSetup` component → **"Setup Board UI"**
5. **What gets created:**
   - ✅ WaitingPanel with WaitingMessageText
   - ✅ NextPhaseButton
6. **Wire up the button:**
   - Select `NextPhaseButton` in Hierarchy
   - In Inspector, find the `Button` component
   - Under **On Click ()**, click `+`
   - Drag `BoardUIController` GameObject to the object field
   - Select function: `BoardUIController → OnNextPhaseButtonClicked()`
7. **Assign references to BoardUIController:**
   - Select GameObject with `BoardUIController` component
   - Drag created objects to the matching fields:
     - `waitingPanel` → WaitingPanel
     - `waitingMessageText` → WaitingMessageText (under WaitingPanel)
     - `nextPhaseButton` → NextPhaseButton

### For PlayerCanvas (Tablet Display):

1. **Still in Screens.unity**
2. **Find PlayerCanvas in Hierarchy**
3. **Add the setup helper:**
   - Select PlayerCanvas
   - Add Component → `ScreensSceneSetup`
4. **Run automatic setup:**
   - Right-click on `ScreensSceneSetup` component → **"Setup Player UI"**
5. **What gets created:**
   - ✅ VoteButtonsPanel with Yes/No buttons
   - ✅ ResultsPanel
6. **Wire up the buttons:**
   - Select `VoteYesButton` (under VoteButtonsPanel)
     - Button → On Click () → `+`
     - Drag `PlayerUIController` → function: `OnVoteYesButtonClicked()`
   - Select `VoteNoButton` (under VoteButtonsPanel)
     - Button → On Click () → `+`
     - Drag `PlayerUIController` → function: `OnVoteNoButtonClicked()`
7. **Assign references to PlayerUIController:**
   - Select GameObject with `PlayerUIController` component
   - Drag created objects:
     - `voteButtonsPanel` → VoteButtonsPanel
     - `voteYesButton` → VoteYesButton
     - `voteNoButton` → VoteNoButton
     - `resultsPanel` → ResultsPanel

8. **Clean up (optional):**
   - Remove `ScreensSceneSetup` components from both canvases after setup

9. **Save the scene** (Ctrl+S / Cmd+S)

---

## Step 3: Configure Build Settings

1. **File → Build Settings**
2. **Add scenes in this order:**
   - Drag scenes from Project window or click "Add Open Scenes"
   ```
   0. GameFlow
   1. Screens
   2. CardLibraryUI
   3. GameScreen
   4. VotingScreen (create if doesn't exist)
   5. GameBoard
   ```
3. **Note:** VotingScreen can be created as a minimal scene for now, or modify GameFlowManager to skip it

---

## Step 4: Create VotingScreen (Optional)

If you don't have VotingScreen.unity:

**Option A - Create minimal scene:**
1. File → New Scene
2. Save as `Assets/Scenes/VotingScreen.unity`
3. Add a Canvas with a simple TextMeshProUGUI saying "Vote on cards here"
4. Save and add to Build Settings

**Option B - Skip it:**
1. Open `Assets/Scripts/GameFlowManager.cs`
2. In `LoadPhaseScenesCoroutine`, comment out VotingScreen loading in Vote phase:
   ```csharp
   case Phase.Vote:
       // yield return LoadSceneCoroutine(votingScreenSceneName);
       // votingScreenLoaded = true;
       yield return LoadSceneCoroutine(gameBoardSceneName);
       gameBoardLoaded = true;
       ConfigureVoteUI();
       break;
   ```

---

## Step 5: Test the Setup

1. **Open GameFlow.unity**
2. **Press Play**
3. **Check Console for logs:**
   ```
   [GameFlowManager] Initialized
   [GameFlowManager] Loading Screens scene...
   [GameFlowManager] Screens scene loaded
   [GameFlowManager] Phase changing from Lobby to Lobby
   ```
4. **Start as Host:**
   - Your NetworkManager should allow starting as host
   - Click Start Host button
5. **Test phase transitions:**
   - Click **Next Phase** button in bottom-right
   - OR press **N** key
   - Watch Console for phase change logs
   - Watch Hierarchy to see scenes loading/unloading

---

## Expected Behavior

### Phase: Lobby
- Board shows: "Waiting for players to join..."
- Tablets show: "Waiting for game to start..."
- Next Phase button is enabled

### Phase: Draw
- Board shows: "Players are selecting cards from the library..."
- **CardLibraryUI scene loads**
- Tablets show CardLibrary UI
- Next Phase button is enabled

### Phase: Play
- Board shows: "Players are playing their cards..."
- **GameScreen scene loads**, CardLibraryUI unloads
- Tablets show GameScreen (hand)
- Next Phase button is disabled (auto-advances when all played)

### Phase: Vote
- **GameBoard scene loads**, GameScreen unloads
- Board shows GameBoard with played cards
- **VotingScreen scene loads** (optional)
- Tablets show Vote Yes/No buttons
- Next Phase button is disabled (auto-advances when all voted)

### Phase: Resolve
- Board shows GameBoard with results
- Tablets show ResultsPanel
- Next Phase button is enabled
- After clicking Next Phase, returns to Draw for next turn

---

## Troubleshooting

### "Scene not found" errors
- Check scene names match exactly in GameFlowManager Inspector
- Verify all scenes are in Build Settings
- Scene names are case-sensitive

### UI elements not appearing
- Check references are assigned in Inspector
- Verify the UI elements were created (check Hierarchy)
- Check Console for "LocalInstance is null" warnings

### Button clicks not working
- Verify Button OnClick events are wired up correctly
- Check EventSystem exists in scene
- Check button has Button component

### NetworkManager errors
- Ensure NetworkManager exists in GameFlow scene
- Verify NetworkManager is marked DontDestroyOnLoad
- Check NetworkManager configuration

### Scenes not loading/unloading
- Watch Console for scene loading logs
- Use Debug.Log in GameFlowManager to trace execution
- Verify scenes exist and are named correctly

---

## Debug Tools

### Console Logs
All phase transitions log detailed information:
```
[GameFlowManager] Phase changing from Draw to Play
[GameFlowManager] Unloading scene: CardLibraryUI
[GameFlowManager] Scene unloaded: CardLibraryUI
[GameFlowManager] Loading scene: GameScreen
[GameFlowManager] Scene loaded: GameScreen
[GameFlowManager] Configuring Play UI
[BoardUIController] Waiting screen: Players are playing their cards...
[GameFlowManager] Phase transition complete: Play
```

### Debug Keys
- **N key** (host): Advance to next phase
- **P key** (client): Play fake card during Play phase

### Hierarchy Inspection
During play mode, watch the Hierarchy:
- DontDestroyOnLoad section shows persistent objects
- Scenes appear/disappear as they load/unload

---

## Next Steps After Setup

1. ✅ Test basic phase transitions
2. ✅ Test with multiple clients (tablets)
3. 🔨 Implement card selection in CardLibraryUI scene
4. 🔨 Implement card hand display in GameScreen scene
5. 🔨 Enhance voting UI to show card details
6. 🔨 Implement results visualization
7. 🔨 Add animations/transitions between phases
8. 🔨 Polish waiting screens

---

## File References

**Helper Scripts (can be deleted after setup):**
- `Assets/Scripts/GameFlowSceneSetup.cs`
- `Assets/Scripts/ScreensSceneSetup.cs`

**Core System:**
- `Assets/Scripts/GameFlowManager.cs`
- `Assets/Scripts/Game/BoardUIController.cs`
- `Assets/Scripts/Game/PlayerUIController.cs`
- `Assets/Scripts/Game/BoardController.cs`
- `Assets/Scripts/Game/PlayerRoleController.cs`
- `Assets/Scripts/Game/GameManager.cs`

**Documentation:**
- `IMPLEMENTATION_SUMMARY.md` - What was implemented
- `Assets/Scripts/GAMEFLOW_SETUP.md` - Detailed technical docs
- `QUICK_START_GUIDE.md` - This file

---

## Support

If something doesn't work:
1. Check the Console for error messages
2. Verify all steps were completed
3. Check that references in Inspector are assigned
4. Review the detailed logs in Console
5. Refer to GAMEFLOW_SETUP.md for technical details

All files are in the **feature-tabletsceneflow** branch.
