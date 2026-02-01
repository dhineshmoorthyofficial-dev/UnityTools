// Unity-inspired color palette
export const colors = {
  background: "#1E1E1E",
  backgroundDarker: "#191919",
  primary: "#3F8FD2", // Unity blue
  accent: "#7BC74D", // Green for features
  text: "#FFFFFF",
  textMuted: "#B4B4B4",
  border: "#3C3C3C",
  
  // Category colors
  workflow: "#3F8FD2", // Blue
  scene: "#7BC74D", // Green
  assets: "#D4A74D", // Gold/Orange
} as const;

// Typography
export const fonts = {
  heading: "Inter, sans-serif",
  body: "Inter, sans-serif",
  mono: "JetBrains Mono, monospace",
} as const;

// Font weights
export const fontWeights = {
  regular: 400,
  medium: 500,
  semibold: 600,
  bold: 700,
  extrabold: 800,
} as const;

// Animation timing (in frames at 30fps)
export const timing = {
  fps: 30,
  
  // Section durations
  intro: {
    start: 0,
    duration: 150, // 5 seconds
  },
  workflow: {
    start: 150,
    duration: 450, // 15 seconds
  },
  scene: {
    start: 600,
    duration: 450, // 15 seconds
  },
  assets: {
    start: 1050,
    duration: 450, // 15 seconds
  },
  outro: {
    start: 1500,
    duration: 300, // 10 seconds
  },
  
  // Animation durations
  fadeIn: 15, // 0.5 seconds
  fadeOut: 15,
  slideIn: 20,
  staggerDelay: 5, // Delay between each tool card
} as const;

// Spring animation configs
export const springConfig = {
  smooth: {
    damping: 200,
    mass: 0.5,
    stiffness: 100,
  },
  bouncy: {
    damping: 100,
    mass: 0.5,
    stiffness: 200,
  },
} as const;

// Layout constants
export const layout = {
  width: 1920,
  height: 1080,
  padding: 80,
  toolCardWidth: 400,
  toolCardHeight: 80,
  gridGap: 20,
} as const;
