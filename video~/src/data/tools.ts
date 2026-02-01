export interface Tool {
  name: string;
  description: string;
  icon?: string;
}

export interface ToolCategory {
  title: string;
  subtitle: string;
  tools: Tool[];
  color: string;
}

export const workflowTools: Tool[] = [
  { name: "Feature Aggregator", description: "Organize scripts & assets by feature" },
  { name: "Macro Actions", description: "Automate action sequences" },
  { name: "Project Bootstrapper", description: "Quick project initialization" },
  { name: "Task Manager", description: "Track tasks with priorities" },
  { name: "Task Manager (Synced)", description: "Real-time collaboration" },
  { name: "TODO Scanner", description: "Find code comments" },
  { name: "Toolbar Extender", description: "Enhanced shortcuts" },
];

export const sceneTools: Tool[] = [
  { name: "Global Object Pinning", description: "Pin objects to toolbar" },
  { name: "Add Scene to Build", description: "Quick build settings" },
  { name: "Hierarchy Icons", description: "Visual indicators" },
  { name: "Object Comparison", description: "Deep GameObject comparison" },
  { name: "Object Grouper", description: "Non-destructive organization" },
  { name: "Snapshot Manager", description: "Capture & restore states" },
  { name: "Sort Children", description: "Alphabetical sorting" },
  { name: "Tab Screenshot", description: "Pixel-perfect captures" },
];

export const assetTools: Tool[] = [
  { name: "Advanced Inspector", description: "Enhanced editing" },
  { name: "Asset Sync Tool", description: "External sync with history" },
  { name: "Dependency Detector", description: "Build optimization" },
  { name: "Integrated Terminal", description: "Multi-tab terminal" },
  { name: "Note Dashboard", description: "Centralized scene notes" },
  { name: "Code Editor", description: "Syntax highlighting" },
  { name: "Quick Prefab Creator", description: "Instant prefab creation" },
  { name: "GSheet Viewer", description: "Google Sheets integration" },
];

export const categories: ToolCategory[] = [
  {
    title: "Workflow & Productivity",
    subtitle: "Automate your workflow",
    tools: workflowTools,
    color: "#3F8FD2", // Blue
  },
  {
    title: "Scene & Hierarchy",
    subtitle: "Master your scenes",
    tools: sceneTools,
    color: "#7BC74D", // Green
  },
  {
    title: "Asset Utilities",
    subtitle: "Power up your assets",
    tools: assetTools,
    color: "#D4A74D", // Gold
  },
];

export const totalToolCount = workflowTools.length + sceneTools.length + assetTools.length;
