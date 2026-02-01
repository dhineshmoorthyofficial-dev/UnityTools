import { AbsoluteFill, interpolate, spring, useCurrentFrame, useVideoConfig } from "remotion";
import { colors } from "../styles/theme";
import { ToolCategory } from "../data/tools";
import { ToolCard } from "./ToolCard";

interface CategorySectionProps {
  category: ToolCategory;
  startFrame: number;
  duration: number;
}

export const CategorySection: React.FC<CategorySectionProps> = ({
  category,
  startFrame,
  duration,
}) => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const localFrame = frame - startFrame;

  // Entry animation
  const entryOpacity = interpolate(localFrame, [0, 20], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  // Exit animation
  const exitOpacity = interpolate(
    localFrame,
    [duration - 30, duration],
    [1, 0],
    {
      extrapolateLeft: "clamp",
      extrapolateRight: "clamp",
    }
  );

  const opacity = Math.min(entryOpacity, exitOpacity);

  // Title animation
  const titleY = spring({
    frame: localFrame,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  const titleScale = spring({
    frame: localFrame,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 120,
    },
  });

  // Badge animation
  const badgeOpacity = interpolate(localFrame, [15, 30], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const badgeScale = spring({
    frame: localFrame - 15,
    fps,
    config: {
      damping: 100,
      mass: 0.5,
      stiffness: 200,
    },
  });

  // Split tools into two columns
  const midPoint = Math.ceil(category.tools.length / 2);
  const leftColumn = category.tools.slice(0, midPoint);
  const rightColumn = category.tools.slice(midPoint);

  return (
    <AbsoluteFill
      style={{
        backgroundColor: colors.background,
        opacity,
        padding: 60,
      }}
    >
      {/* Background gradient */}
      <div
        style={{
          position: "absolute",
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          background: `radial-gradient(ellipse at top left, ${category.color}10 0%, transparent 50%)`,
        }}
      />

      {/* Category header */}
      <div
        style={{
          display: "flex",
          alignItems: "center",
          gap: 24,
          marginBottom: 50,
          transform: `translateY(${(1 - titleY) * 40}px) scale(${titleScale})`,
        }}
      >
        {/* Color indicator */}
        <div
          style={{
            width: 8,
            height: 80,
            backgroundColor: category.color,
            borderRadius: 4,
            boxShadow: `0 0 30px ${category.color}80`,
          }}
        />

        <div>
          <h2
            style={{
              fontSize: 56,
              fontWeight: 700,
              color: colors.text,
              fontFamily: "Inter, sans-serif",
              margin: 0,
              letterSpacing: "-1px",
            }}
          >
            {category.title}
          </h2>
          <p
            style={{
              fontSize: 24,
              fontWeight: 500,
              color: colors.textMuted,
              fontFamily: "Inter, sans-serif",
              margin: 0,
              marginTop: 8,
            }}
          >
            {category.subtitle}
          </p>
        </div>

        {/* Tool count badge */}
        <div
          style={{
            marginLeft: "auto",
            opacity: badgeOpacity,
            transform: `scale(${badgeScale})`,
          }}
        >
          <div
            style={{
              backgroundColor: `${category.color}20`,
              border: `2px solid ${category.color}`,
              borderRadius: 20,
              padding: "10px 24px",
            }}
          >
            <span
              style={{
                fontSize: 24,
                fontWeight: 700,
                color: category.color,
                fontFamily: "Inter, sans-serif",
              }}
            >
              {category.tools.length} tools
            </span>
          </div>
        </div>
      </div>

      {/* Tools grid - two columns */}
      <div
        style={{
          display: "flex",
          gap: 30,
          flex: 1,
        }}
      >
        {/* Left column */}
        <div
          style={{
            flex: 1,
            display: "flex",
            flexDirection: "column",
            gap: 16,
          }}
        >
          {leftColumn.map((tool, index) => (
            <ToolCard
              key={tool.name}
              tool={tool}
              index={index}
              categoryColor={category.color}
              startFrame={30}
            />
          ))}
        </div>

        {/* Right column */}
        <div
          style={{
            flex: 1,
            display: "flex",
            flexDirection: "column",
            gap: 16,
          }}
        >
          {rightColumn.map((tool, index) => (
            <ToolCard
              key={tool.name}
              tool={tool}
              index={index + midPoint}
              categoryColor={category.color}
              startFrame={30}
            />
          ))}
        </div>
      </div>
    </AbsoluteFill>
  );
};
