import { AbsoluteFill, interpolate, useCurrentFrame, spring, useVideoConfig } from "remotion";
import { colors } from "../styles/theme";
import { totalToolCount } from "../data/tools";

export const IntroSequence: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Logo animation
  const logoScale = spring({
    frame,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  const logoRotation = spring({
    frame: frame - 5,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 80,
    },
  });

  // Title animation
  const titleOpacity = interpolate(frame, [20, 40], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const titleY = spring({
    frame: frame - 20,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  // Tagline animation
  const taglineOpacity = interpolate(frame, [45, 65], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const taglineY = spring({
    frame: frame - 45,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  // Tool count animation
  const countOpacity = interpolate(frame, [70, 90], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const countScale = spring({
    frame: frame - 70,
    fps,
    config: {
      damping: 100,
      mass: 0.5,
      stiffness: 200,
    },
  });

  // Exit animation
  const exitOpacity = interpolate(frame, [120, 150], [1, 0], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  return (
    <AbsoluteFill
      style={{
        backgroundColor: colors.background,
        justifyContent: "center",
        alignItems: "center",
        opacity: exitOpacity,
      }}
    >
      {/* Background gradient effect */}
      <div
        style={{
          position: "absolute",
          width: "100%",
          height: "100%",
          background: `radial-gradient(ellipse at center, ${colors.primary}15 0%, transparent 70%)`,
        }}
      />

      {/* Unity-style cube logo */}
      <div
        style={{
          transform: `scale(${logoScale}) rotate(${logoRotation * 360}deg)`,
          marginBottom: 40,
        }}
      >
        <svg width="120" height="120" viewBox="0 0 120 120">
          {/* Cube shape */}
          <polygon
            points="60,10 110,35 110,85 60,110 10,85 10,35"
            fill={colors.primary}
            opacity={0.9}
          />
          <polygon
            points="60,10 110,35 60,60 10,35"
            fill={colors.primary}
            opacity={1}
          />
          <polygon
            points="60,60 110,35 110,85 60,110"
            fill={colors.primary}
            opacity={0.7}
          />
          <polygon
            points="60,60 10,35 10,85 60,110"
            fill={colors.primary}
            opacity={0.5}
          />
          {/* Center highlight */}
          <circle cx="60" cy="55" r="15" fill="white" opacity={0.3} />
        </svg>
      </div>

      {/* Title */}
      <div
        style={{
          opacity: titleOpacity,
          transform: `translateY(${(1 - titleY) * 30}px)`,
          textAlign: "center",
        }}
      >
        <h1
          style={{
            fontSize: 72,
            fontWeight: 800,
            color: colors.text,
            fontFamily: "Inter, sans-serif",
            margin: 0,
            letterSpacing: "-2px",
          }}
        >
          Unity Productivity Tools
        </h1>
      </div>

      {/* Tagline */}
      <div
        style={{
          opacity: taglineOpacity,
          transform: `translateY(${(1 - taglineY) * 20}px)`,
          marginTop: 20,
        }}
      >
        <p
          style={{
            fontSize: 32,
            fontWeight: 500,
            color: colors.textMuted,
            fontFamily: "Inter, sans-serif",
            margin: 0,
          }}
        >
          Supercharge Your Unity Workflow
        </p>
      </div>

      {/* Tool count badge */}
      <div
        style={{
          opacity: countOpacity,
          transform: `scale(${countScale})`,
          marginTop: 50,
          display: "flex",
          alignItems: "center",
          gap: 12,
        }}
      >
        <div
          style={{
            backgroundColor: colors.accent,
            borderRadius: 30,
            padding: "12px 32px",
            display: "flex",
            alignItems: "center",
            gap: 10,
          }}
        >
          <span
            style={{
              fontSize: 36,
              fontWeight: 700,
              color: colors.backgroundDarker,
              fontFamily: "Inter, sans-serif",
            }}
          >
            {totalToolCount}+
          </span>
          <span
            style={{
              fontSize: 24,
              fontWeight: 600,
              color: colors.backgroundDarker,
              fontFamily: "Inter, sans-serif",
            }}
          >
            Tools
          </span>
        </div>
      </div>
    </AbsoluteFill>
  );
};
