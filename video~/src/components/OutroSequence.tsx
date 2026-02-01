import { AbsoluteFill, interpolate, spring, useCurrentFrame, useVideoConfig } from "remotion";
import { colors } from "../styles/theme";
import { totalToolCount } from "../data/tools";

interface OutroSequenceProps {
  startFrame: number;
}

export const OutroSequence: React.FC<OutroSequenceProps> = ({ startFrame }) => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const localFrame = frame - startFrame;

  // Entry animation
  const entryOpacity = interpolate(localFrame, [0, 20], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  // Logo animation
  const logoScale = spring({
    frame: localFrame,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  // CTA animation
  const ctaOpacity = interpolate(localFrame, [20, 40], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const ctaY = spring({
    frame: localFrame - 20,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  // GitHub URL animation
  const urlOpacity = interpolate(localFrame, [40, 60], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const urlScale = spring({
    frame: localFrame - 40,
    fps,
    config: {
      damping: 100,
      mass: 0.5,
      stiffness: 200,
    },
  });

  // Installation badge animation
  const installOpacity = interpolate(localFrame, [60, 80], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const installY = spring({
    frame: localFrame - 60,
    fps,
    config: {
      damping: 200,
      mass: 0.5,
      stiffness: 100,
    },
  });

  // Version badge animation
  const versionOpacity = interpolate(localFrame, [80, 100], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  return (
    <AbsoluteFill
      style={{
        backgroundColor: colors.background,
        justifyContent: "center",
        alignItems: "center",
        opacity: entryOpacity,
      }}
    >
      {/* Background effects */}
      <div
        style={{
          position: "absolute",
          width: "100%",
          height: "100%",
          background: `
            radial-gradient(ellipse at top left, ${colors.workflow}15 0%, transparent 40%),
            radial-gradient(ellipse at top right, ${colors.scene}15 0%, transparent 40%),
            radial-gradient(ellipse at bottom center, ${colors.assets}15 0%, transparent 40%)
          `,
        }}
      />

      {/* Small logo */}
      <div
        style={{
          transform: `scale(${logoScale})`,
          marginBottom: 30,
        }}
      >
        <svg width="80" height="80" viewBox="0 0 120 120">
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
          <circle cx="60" cy="55" r="12" fill="white" opacity={0.3} />
        </svg>
      </div>

      {/* CTA text */}
      <div
        style={{
          opacity: ctaOpacity,
          transform: `translateY(${(1 - ctaY) * 30}px)`,
          textAlign: "center",
          marginBottom: 40,
        }}
      >
        <h2
          style={{
            fontSize: 52,
            fontWeight: 700,
            color: colors.text,
            fontFamily: "Inter, sans-serif",
            margin: 0,
          }}
        >
          Start Building Better
        </h2>
        <p
          style={{
            fontSize: 28,
            fontWeight: 500,
            color: colors.textMuted,
            fontFamily: "Inter, sans-serif",
            margin: 0,
            marginTop: 12,
          }}
        >
          {totalToolCount}+ tools to supercharge your Unity workflow
        </p>
      </div>

      {/* GitHub URL */}
      <div
        style={{
          opacity: urlOpacity,
          transform: `scale(${urlScale})`,
          marginBottom: 30,
        }}
      >
        <div
          style={{
            backgroundColor: colors.backgroundDarker,
            border: `2px solid ${colors.border}`,
            borderRadius: 16,
            padding: "18px 40px",
            display: "flex",
            alignItems: "center",
            gap: 16,
          }}
        >
          {/* GitHub icon */}
          <svg width="32" height="32" viewBox="0 0 24 24" fill={colors.text}>
            <path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0024 12c0-6.63-5.37-12-12-12z" />
          </svg>
          <span
            style={{
              fontSize: 24,
              fontWeight: 500,
              color: colors.text,
              fontFamily: "JetBrains Mono, monospace",
            }}
          >
            github.com/your-repo/unity-tools
          </span>
        </div>
      </div>

      {/* Installation method */}
      <div
        style={{
          opacity: installOpacity,
          transform: `translateY(${(1 - installY) * 20}px)`,
          display: "flex",
          alignItems: "center",
          gap: 20,
        }}
      >
        <div
          style={{
            backgroundColor: `${colors.accent}20`,
            border: `1px solid ${colors.accent}`,
            borderRadius: 10,
            padding: "12px 24px",
          }}
        >
          <span
            style={{
              fontSize: 18,
              fontWeight: 600,
              color: colors.accent,
              fontFamily: "Inter, sans-serif",
            }}
          >
            Install via Unity Package Manager
          </span>
        </div>

        {/* Version badge */}
        <div
          style={{
            opacity: versionOpacity,
            backgroundColor: colors.backgroundDarker,
            border: `1px solid ${colors.border}`,
            borderRadius: 10,
            padding: "12px 20px",
          }}
        >
          <span
            style={{
              fontSize: 18,
              fontWeight: 600,
              color: colors.textMuted,
              fontFamily: "JetBrains Mono, monospace",
            }}
          >
            v1.0.2
          </span>
        </div>
      </div>

      {/* Author credit */}
      <div
        style={{
          position: "absolute",
          bottom: 40,
          opacity: versionOpacity,
        }}
      >
        <span
          style={{
            fontSize: 16,
            fontWeight: 400,
            color: colors.textMuted,
            fontFamily: "Inter, sans-serif",
          }}
        >
          by Dhinesh Moorthy
        </span>
      </div>
    </AbsoluteFill>
  );
};
