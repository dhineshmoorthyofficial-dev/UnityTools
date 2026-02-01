import { Img, interpolate, spring, useCurrentFrame, useVideoConfig } from "remotion";
import { colors } from "../styles/theme";
import { Tool } from "../data/tools";

interface ToolCardProps {
  tool: Tool;
  index: number;
  categoryColor: string;
  startFrame: number;
  isActive?: boolean;
}

export const ToolCard: React.FC<ToolCardProps> = ({
  tool,
  index,
  categoryColor,
  startFrame,
  isActive,
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

  // Active state animation
  const activeScale = spring({
    frame: isActive ? localFrame : 0,
    fps,
    config: {
      damping: 20,
      mass: 0.5,
      stiffness: 200,
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
        transform: `scale(${isActive ? 1.05 : scale}) translateX(${(1 - slideX) * 50}px)`,
        display: "flex",
        alignItems: "center",
        gap: 16,
        padding: "12px 20px",
        backgroundColor: isActive ? `${categoryColor}15` : colors.backgroundDarker,
        borderRadius: 12,
        border: `1px solid ${isActive ? categoryColor : colors.border}`,
        position: "relative",
        overflow: "hidden",
        width: "100%",
        transition: "all 0.3s ease",
        boxShadow: isActive ? `0 10px 30px ${categoryColor}15` : "none",
      }}
    >
      {/* Glow effect */}
      <div
        style={{
          position: "absolute",
          left: 0,
          top: 0,
          bottom: 0,
          width: isActive ? 6 : 4,
          backgroundColor: categoryColor,
          opacity: isActive ? 0.8 : glowOpacity,
          boxShadow: isActive ? `0 0 20px ${categoryColor}` : `0 0 10px ${categoryColor}40`,
        }}
      />

      {/* Icon placeholder */}
      <div
        style={{
          width: 40,
          height: 40,
          borderRadius: 8,
          backgroundColor: `${categoryColor}20`,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          flexShrink: 0,
        }}
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill={categoryColor}>
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
            fontSize: 18,
            fontWeight: 600,
            color: colors.text,
            fontFamily: "Inter, sans-serif",
            margin: 0,
            marginBottom: 2,
          }}
        >
          {tool.name}
        </h3>
        <p
          style={{
            fontSize: 12,
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
