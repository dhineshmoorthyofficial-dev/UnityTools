import { AbsoluteFill, Img, interpolate, spring, useCurrentFrame, useVideoConfig, staticFile } from "remotion";
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

  // Entry/Exit animations
  const opacity = interpolate(localFrame, [0, 20, duration - 20, duration], [0, 1, 1, 0], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  // Calculation for active tool
  // We want to skip intro/outro time, say 30 frames at start/end
  const usefulDuration = duration - 60;
  const toolCycleDuration = Math.max(30, Math.floor(usefulDuration / category.tools.length));
  const activeIndex = Math.min(
    category.tools.length - 1,
    Math.max(0, Math.floor((localFrame - 30) / toolCycleDuration))
  );

  const activeTool = category.tools[activeIndex];

  return (
    <AbsoluteFill
      style={{
        backgroundColor: colors.background,
        opacity,
        padding: 60,
        display: "flex",
        flexDirection: "row",
        gap: 40,
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
          zIndex: 0,
        }}
      />

      {/* Left Column: Category Header & Tool List */}
      <div style={{ flex: 1.2, display: "flex", flexDirection: "column", zIndex: 1 }}>
        {/* Category header */}
        <div
          style={{
            display: "flex",
            alignItems: "center",
            gap: 24,
            marginBottom: 40,
          }}
        >
          <div
            style={{
              width: 8,
              height: 60,
              backgroundColor: category.color,
              borderRadius: 4,
              boxShadow: `0 0 30px ${category.color}80`,
            }}
          />
          <div>
            <h2
              style={{
                fontSize: 48,
                fontWeight: 700,
                color: colors.text,
                fontFamily: "Inter, sans-serif",
                margin: 0,
              }}
            >
              {category.title}
            </h2>
            <p
              style={{
                fontSize: 20,
                color: colors.textMuted,
                fontFamily: "Inter, sans-serif",
                margin: 0,
              }}
            >
              {category.subtitle}
            </p>
          </div>
        </div>

        {/* Tool List */}
        <div
          style={{
            display: "flex",
            flexDirection: "column",
            gap: 12,
            overflow: "hidden",
          }}
        >
          {category.tools.map((tool, index) => (
            <ToolCard
              key={tool.name}
              tool={tool}
              index={index}
              categoryColor={category.color}
              startFrame={30}
              isActive={index === activeIndex}
            />
          ))}
        </div>
      </div>

      {/* Right Column: Screenshot Showcase */}
      <div
        style={{
          flex: 1,
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          zIndex: 1,
        }}
      >
        <div
          style={{
            width: "100%",
            aspectRatio: "16/10",
            backgroundColor: colors.backgroundDarker,
            borderRadius: 20,
            border: `2px solid ${category.color}40`,
            overflow: "hidden",
            boxShadow: `0 20px 50px rgba(0,0,0,0.5)`,
            position: "relative",
          }}
        >
          {activeTool.image ? (
            <Img
              src={staticFile(activeTool.image)}
              style={{
                width: "100%",
                height: "100%",
                objectFit: "cover",
              }}
            />
          ) : (
            <div
              style={{
                width: "100%",
                height: "100%",
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                color: colors.textMuted,
                gap: 20,
              }}
            >
              <div
                style={{
                  width: 120,
                  height: 120,
                  borderRadius: 20,
                  backgroundColor: `${category.color}10`,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <svg width="60" height="60" viewBox="0 0 24 24" fill={category.color}>
                  <rect x="3" y="3" width="7" height="7" rx="1" />
                  <rect x="14" y="3" width="7" height="7" rx="1" />
                  <rect x="3" y="14" width="7" height="7" rx="1" />
                  <rect x="14" y="14" width="7" height="7" rx="1" />
                </svg>
              </div>
              <span style={{ fontSize: 24, fontWeight: 500 }}>
                Feature Snapshot Incoming
              </span>
            </div>
          )}

          {/* Overlay info */}
          <div
            style={{
              position: "absolute",
              bottom: 0,
              left: 0,
              right: 0,
              padding: "20px 30px",
              background: "linear-gradient(transparent, rgba(0,0,0,0.8))",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "flex-end",
            }}
          >
            <div>
              <div style={{ color: category.color, fontSize: 14, fontWeight: 700, textTransform: "uppercase", marginBottom: 4 }}>
                Showcasing
              </div>
              <div style={{ color: "white", fontSize: 28, fontWeight: 700 }}>
                {activeTool.name}
              </div>
            </div>
          </div>
        </div>
      </div>
    </AbsoluteFill>
  );
};
