export interface Tool {
  name: string;
  description: string;
  icon?: string;
  image?: string;
}

export interface ToolCategory {
  title: string;
  subtitle: string;
  tools: Tool[];
  color: string;
}

export const workflowTools: Tool[] = [
  {
    name: "Feature Aggregator",
    description: "Organize scripts & assets by feature",
    image: "/screenshots/Capture_Feature_Aggregator_20260201_214936.png"
  },
  {
    name: "Macro Actions",
    description: "Automate action sequences",
    image: "/screenshots/Capture_Macro_Actions_20260201_215102.png"
  },
  {
    name: "Project Bootstrapper",
    description: "Quick project initialization",
    image: "/screenshots/Capture_Bootstrapper_20260201_215246.png"
  },
  {
    name: "Task Manager",
    description: "Track tasks with priorities",
    image: "/screenshots/Capture_Task_Manager_20260201_215319.png"
  },
  {
    name: "Task Manager (Synced)",
    description: "Real-time collaboration",
    image: "/screenshots/Capture_Task_Manager_(Synced)_20260201_215328.png"
  },
  {
    name: "TODO Scanner",
    description: "Find code comments",
    image: "/screenshots/Capture_TODO_Scanner_20260201_215337.png"
  },
  {
    name: "Toolbar Extender",
    description: "Enhanced shortcuts",
    image: "/screenshots/toolbar extender.png"
  },
];

export const sceneTools: Tool[] = [
  {
    name: "Global Object Pinning",
    description: "Pin objects to toolbar",
    image: "/screenshots/pin to toolbar.png"
  },
  {
    name: "Add Scene to Build",
    description: "Quick build settings",
    image: "/screenshots/add to build settings.png"
  },
  {
    name: "Hierarchy Icons",
    description: "Visual indicators",
    image: "/screenshots/hierarchy icons.png"
  },
  {
    name: "Object Comparison",
    description: "Deep GameObject comparison",
    image: "/screenshots/Capture_Object_Comparison_20260201_215201.png"
  },
  {
    name: "Object Grouper",
    description: "Non-destructive organization",
    image: "/screenshots/Capture_Object_Grouper_20260201_215225.png"
  },
  {
    name: "Snapshot Manager",
    description: "Capture & restore states",
    image: "/screenshots/Capture_Snapshot_Tool_20260201_215303.png"
  },
  {
    name: "Sort Children",
    description: "Alphabetical sorting",
    image: "/screenshots/sort children alphabetically.png"
  },
  {
    name: "Tab Screenshot",
    description: "Pixel-perfect captures",
    image: "/screenshots/tab screenshot.png"
  },
];

export const assetTools: Tool[] = [
  {
    name: "Advanced Inspector",
    description: "Enhanced editing",
    image: "/screenshots/Capture_Advanced_Inspector_20260201_213257.png",
  },
  {
    name: "Asset Sync Tool",
    description: "External sync with history",
    image: "/screenshots/Capture_Asset_Sync_20260201_214344.png"
  },
  {
    name: "Dependency Detector",
    description: "Build optimization",
    image: "/screenshots/Capture_Hidden_Dependencies_20260201_215035.png"
  },
  {
    name: "Integrated Terminal",
    description: "Multi-tab terminal",
    image: "/screenshots/Capture_Terminal_20260201_215049.png"
  },
  {
    name: "Note Dashboard",
    description: "Centralized scene notes",
    image: "/screenshots/Capture_Note_Dashboard_20260201_215132.png"
  },
  {
    name: "Code Editor",
    description: "Syntax highlighting",
    image: "/screenshots/Capture_Code_Editor_20260201_214906.png"
  },
  {
    name: "Quick Prefab Creator",
    description: "Instant prefab creation",
    image: "/screenshots/quick prefab creator.png"
  },
  {
    name: "GSheet Viewer",
    description: "Google Sheets integration",
    image: "/screenshots/Capture_GSheet_Data_Viewer_20260201_214951.png"
  },
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
