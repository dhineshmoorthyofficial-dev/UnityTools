# Menu Management Skill

This skill provides instructions for maintaining and optimizing the `Tools/GameDevTools` menu system in the Unity project.

## Menu Hierarchy & Ordering Logic

To maintain a clean and professional editor experience, all menu items under `Tools/GameDevTools` must follow a strict ordering and naming convention.

### 1. The Dashboard (Pinned Top)
- **Menu Path**: `Tools/GameDevTools/Unified Dashboard`
- **Order Value**: `0`
- **Rationale**: This is the primary hub for all tools and must always be the first item.

### 2. The Middle Tools (Alphabetical Block)
- **Order Range**: `100 - 999`
- **Spacing**: Use **10-unit increments** (e.g., 100, 110, 120).
- **Sorting**: Maintain alphabetical order based on the display name.
- **The Rule of 10**: Unity automatically draws a horizontal separator line if the gap between two adjacent `MenuItem` order values is **11 or more**. To keep the list continuous, ensure gaps never exceed 10.

### 3. The Welcome Window (Pinned Bottom)
- **Menu Path**: `Tools/GameDevTools/Welcome`
- **Order Value**: `1000`
- **Rationale**: The welcome/help window should always be the last item in the list.

## Implementation Guidelines

When adding a new tool:
1. Identify its alphabetical position among existing middle tools.
2. Assign a `MenuItem` order value that fits the sequence (e.g., if it fits between 150 and 160, use 155 and shift others, or re-space the whole block).
3. Update `CHANGELOG.md` and `README.md` to reflect the change.
4. Update the `GameDevToolsWelcomeWindow.cs` to include the new tool in the appropriate category.

## Example
```csharp
[MenuItem("Tools/GameDevTools/Awesome New Tool", false, 150)]
public static void ShowWindow() { ... }
```
