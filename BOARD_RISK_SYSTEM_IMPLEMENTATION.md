# Board Risk System Implementation Summary

## Files Modified

### 1. **HexTile.cs** 
**Location:** `Assets/Scripts/Game/HexTile.cs`

**Changes:**
- Added `List<Risk> activeRisks` to store risks on each tile
- Added methods:
  - `AddRisk(Risk)` - adds a risk to the tile
  - `RemoveRisk(Risk)` - removes a specific risk
  - `GetRisks()` - returns read-only list of risks
  - `ClearRisks()` - removes all risks
  - `GetRiskCount()` - returns number of risks
  - `HasRisks()` - checks if tile has any risks

**Purpose:** Foundation for storing risk data on individual board cells.

---

### 2. **GameManager.cs**
**Location:** `Assets/Scripts/Game/GameManager.cs`

**Changes:**
- Modified `ResolveCardsAndEndTurn()` method to call `ApplyCardEffectsToBoard()` for accepted cards
- Added `ApplyCardEffectsToBoard(PlayedCard)` method (placeholder for integration point)
- Added check for `BoardRiskManager.Instance` availability

**Purpose:** Integrate card resolution with board risk system - when cards are accepted during voting, their risks are now applied to the board.

---

## Files Created

### 1. **BoardCellRiskDisplay.cs**
**Location:** `Assets/Scripts/Game/BoardCellRiskDisplay.cs`

**Functionality:**
- Displays risk icons on hex tiles
- Applies visual highlight when risks are present
- Manages risk icon positioning around tiles
- Updates display based on risk severity
- Limit visible icons (default: 3)

**Key Methods:**
- `UpdateRiskDisplay()` - refreshes visual representation
- `ApplyRiskHighlight()` - tints tile based on risk severity
- `CreateRiskIcons()` - instantiates icon prefabs

**Integration:** Attach to each HexTile GameObject for visual feedback.

---

### 2. **BoardRiskManager.cs**
**Location:** `Assets/Scripts/Game/BoardRiskManager.cs`

**Functionality:**
- Centralized singleton for managing all risks on the board
- Maintains `Dictionary<int, List<Risk>>` mapping cells to risks
- Syncs with `BoardCellRiskDisplay` for visual updates
- Provides utility methods for risk queries

**Key Methods:**
- `AddRiskToCell(int hexId, Risk)` - place risk on cell
- `RemoveRiskFromCell(int hexId, Risk)` - remove specific risk
- `GetRisksOnCell(int hexId)` - query risks on cell
- `GetCellsWithRisk(Risk)` - find all cells with risk
- `GetCellsWithAnyRisk()` - find all active cells
- `ClearCell(int hexId)` / `ClearAllCells()` - remove risks
- `GetRandomCellId()` / `GetRandomCells(int count)` - random placement

**Integration:** Automatically initializes at start, finds all HexTiles via `FindObjectsOfType()`.

---

### 3. **CardResolutionResolver.cs**
**Location:** `Assets/Scripts/Game/CardResolutionResolver.cs`

**Functionality:**
- Determines placement strategy for card risks on the board
- Supports multiple placement strategies (Random, RoleSpecific, PlayerChoice)
- Handles risk-to-cell mapping logic

**Key Methods:**
- `ResolveCardToBoard(Assignedrisks, Role)` - main entry point for applying card risks
- `DeterminePlacementCells(Role, Risk)` - determine target cells based on strategy
- `GetRoleSpecificCells(Role, int)` - role-based placement (placeholder)

**Placement Strategies:**
- **Random:** Places risks on random cells
- **RoleSpecific:** Places risks on cells related to player role (ready for expansion)
- **PlayerChoice:** Framework for user-selected placement

**Integration:** Called from `GameManager.ApplyCardEffectsToBoard()`.

---

### 4. **BoardRiskSystemDemo.cs**
**Location:** `Assets/Scripts/Game/BoardRiskSystemDemo.cs`

**Functionality:**
- Test/demo script for the risk system
- Allows runtime testing without playing through full game

**Keyboard Controls:**
- **R Key:** Add random risks to random board cells
- **C Key:** Clear all risks from board
- **D Key:** Display summary of current board state

**Integration:** Attach to any GameObject to enable testing.

---

## System Architecture

```
GameManager (card resolution)
    ↓
ApplyCardEffectsToBoard()
    ↓
CardResolutionResolver.ResolveCardToBoard()
    ↓
BoardRiskManager.AddRiskToCell()
    ↓
HexTile.AddRisk()
    ↓
BoardCellRiskDisplay.UpdateRiskDisplay()
    ↓
[Board tiles now show risk visuals and data]
```

---

## Setup Instructions

### 1. **Auto-Setup (Minimal)**
- `BoardRiskManager` initializes automatically
- Finds all `HexTile` components in scene
- No manual configuration required

### 2. **Visual Display Setup**
Add `BoardCellRiskDisplay` component to each HexTile:
1. Select a HexTile in hierarchy
2. Add Component → `BoardCellRiskDisplay`
3. Assign `riskIconPrefab` (UI Image prefab with RiskIconUI component)
4. (Optional) Configure `maxVisibleRisks`, highlight color, etc.

### 3. **Risk Placement Strategy**
Configure in `CardResolutionResolver`:
- Set `strategy` (Random, RoleSpecific, etc.)
- Set `risksPerCard` (how many cells receive each card's risk)

### 4. **Demo Testing**
Attach `BoardRiskSystemDemo` to any GameObject and use keyboard controls:
- Press **R** in play mode to add test risks
- Press **D** to see current board state
- Press **C** to clear all risks

---

## Design Decisions Made

### **Risk Storage**
- Risks stored on both `HexTile` and `BoardRiskManager` for redundancy
- Synchronized updates to ensure consistency

### **Placement Strategy**
- Default: **Random** (simplest, works for any board layout)
- **RoleSpecific** ready for expansion (e.g., Farming → top tiles)
- **PlayerChoice** framework prepared for future UI

### **Visual Feedback**
- Tile color tinting based on risk severity
- Up to 3 risk icons displayed per tile
- Icons arranged in circle around tile center

### **Network Sync**
- Foundation ready for Netcode integration
- `BoardRiskManager` singleton can be extended with RPCs
- Currently local-only (no multiplayer sync yet)

---

## Known Limitations & TODOs

### **Not Yet Implemented**
1. Network synchronization via Netcode RPCs
2. Hex adjacency detection (for cascade effects)
3. Role-specific cell mapping (needs board layout definition)
4. Player-choice placement UI
5. Risk triggering & disaster resolution
6. Persistent risk state across saves

### **Placeholder Methods**
- `GameManager.ApplyCardEffectsToBoard()` - needs card object reference
- `CardResolutionResolver.GetRoleSpecificCells()` - needs role→cells mapping
- `CardResolutionResolver.GetAdjacentCells()` - needs hex math implementation

---

## Next Steps

### **Phase 3: Risk Placement UI** (Optional)
- Create `RiskPlacementUI.cs` for manual risk placement
- Add drag-and-drop to board interface

### **Phase 5: Risk Visualization**
- Enhance `BoardCellRiskDisplay` with animations
- Add particle effects for high-severity risks
- Implement tooltips showing full risk details

### **Phase 6: Disaster System**
- Implement `DisasterResolver.cs` for cascading effects
- Add risk triggering logic to `RiskManager`
- Handle board state changes (cell destruction, etc.)

### **Network Sync**
- Add `[Rpc]` methods to `BoardRiskManager`
- Implement `[NetworkSerialize]` for risk data
- Test with multi-client setup

---

## Testing Checklist

- [ ] `BoardRiskManager` initializes without errors
- [ ] `HexTiles` display risk icons when risks added
- [ ] Demo controls (R, C, D keys) work correctly
- [ ] `GetRisksOnCell()` returns correct risks
- [ ] Color highlighting applies based on severity
- [ ] Multiple risks per cell display properly
- [ ] `ClearAllCells()` removes all visual indicators
- [ ] No console warnings on startup
