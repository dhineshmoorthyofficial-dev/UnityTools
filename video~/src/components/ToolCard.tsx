import { interpolate, spring, useCurrentFrame, useVideoConfig } from "remotion";
import { colors } from "../styles/theme";
import { Tool } from "../data/tools";

interface ToolCardProps {
  tool: Tool;
  index: number;
  categoryColor: string;
  startFrame: number;
}

export const ToolCard: React.FC<ToolCardProps> = ({
  tool,
  index,
  categoryColor,
  startFrame,
}) => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const delay = index * 4; // Stagger delay between cards
  const localFrame = frame - startFrame - delay;

  // Entry animation
  const opacity = interpolate(localFrame, [0, 12], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const scale = spring({
    frame: localFrame,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 150,
    },
  });

  const slideX = spring({
    frame: localFrame,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 120,
    },
  });

  // Glow effect
  const glowOpacity = interpolate(localFrame, [0, 15, 30], [0, 0.6, 0.3], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  return (
    <div
      style={{
        opacity,
        transform: `scale(${scale}) translateX(${(1 - slideX) * 50}px)`,
        display: "flex",
        alignItems: "center",
        gap: 16,
        padding: "16px 24px",
        backgroundColor: colors.backgroundDarker,
        borderRadius: 12,
        border: `1px solid ${colors.border}`,
        position: "relative",
        overflow: "hidden",
        minWidth: 380,
      }}
    >
      {/* Glow effect */}
      <div
        style={{
          position: "absolute",
          left: 0,
          top: 0,
          bottom: 0,
          width: 4,
          backgroundColor: categoryColor,
          opacity: glowOpacity,
          boxShadow: `0 0 20px ${categoryColor}`,
        }}
      />

      {/* Icon placeholder */}
      <div
        style={{
          width: 44,
          height: 44,
          borderRadius: 10,
          backgroundColor: `${categoryColor}20`,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          flexShrink: 0,
        }}
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill={categoryColor}>
          <rect x="3" y="3" width="7" height="7" rx="1" />
          <rect x="14" y="3" width="7" height="7" rx="1" />
          <rect x="3" y="14" width="7" height="7" rx="1" />
          <rect x="14" y="14" width="7" height="7" rx="1" />
        </svg>
      </div>

      {/* Content */}
      <div style={{ flex: 1 }}>
        <h3
          style={{
            fontSize: 20,
            fontWeight: 600,
            color: colors.text,
            fontFamily: "Inter, sans-serif",
            margin: 0,
            marginBottom: 4,
          }}
        >
          {tool.name}
        </h3>
        <p
          style={{
            fontSize: 14,
            fontWeight: 400,
            color: colors.textMuted,
            fontFamily: "Inter, sans-serif",
            margin: 0,
          }}
        >
          {tool.description}
        </p>
      </div>
    </div>
  );
};
