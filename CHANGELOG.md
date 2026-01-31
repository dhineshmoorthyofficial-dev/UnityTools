# Changelog

All notable changes to this project will be documented in this file.

## [1.0.1] - 2026-01-31

### Added
- **Sort Children Alphabetically**:
    - Implemented **Natural Numeric Sorting** (e.g., "Group 3" now correctly comes before "Group 10").
    - Added tool to Hierarchy context menu for quick access.
    - Added tool to top menu: `Tools > GameDevTools > Sort Children Alphabetically`.
    - Integrated keyboard shortcut: `Alt+Shift+S`.
    - Added user warning when attempting to sort objects with fewer than 2 children.
- **Documentation**:
    - Updated `README.md` with new tool features and access paths.
    - Updated `PROJECT_CONVENTIONS.md` with alphabetical ordering requirements.

## [1.0.0] - 2026-01-30

### Added
- **Workflow & Productivity**:
    - **Feature Aggregator**: Group related scripts and assets by concept.
    - **Macro Actions**: Build and execute sequences of editor actions.
    - **Project Bootstrapper**: Quick setup for new Unity projects.
    - **Task Manager (Synced)**: Real-time collaborative task tracking.
    - **TODO/FIXME Scanner**: Scan project for code comments.
- **Scene & Hierarchy**:
    - **Global Object Pinning**: Pin GameObjects and Assets to the toolbar.
    - **Add Scene to Build**: Quick utility for build settings.
    - **Hierarchy Icons**: Visual indicators and dependency tracking.
    - **Object Comparison Tool**: Deep comparison and syncing between GameObjects.
    - **Object Grouper**: Non-destructive hierarchy organization.
    - **Snapshot Manager**: Capture and restore GameObject states.
- **Asset Utilities**:
    - **Advanced Inspector**: Favorites system and component search.
    - **Asset Sync Tool**: Automatically sync files to external locations.
    - **Hidden Dependency Detector**: Analyze build size and references.
    - **Integrated Terminal**: Built-in multi-tabbed terminal (CMD/PowerShell).
    - **Quick Prefab Creator**: Instant prefab generation from selection.
    - **GSheet Data Viewer**: Real-time Google Sheets integration.
- **Toolbar Extender**: Core feature for history, shortcuts, and platform switching.

### Changed
- Refactored toolbar layout for better stability and pinning support.
- Improved Selection History navigation.
