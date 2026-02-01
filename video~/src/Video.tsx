import { AbsoluteFill, Sequence } from "remotion";
import { IntroSequence } from "./components/IntroSequence";
import { CategorySection } from "./components/CategorySection";
import { OutroSequence } from "./components/OutroSequence";
import { categories } from "./data/tools";
import { colors, timing } from "./styles/theme";

// Timing configuration (in frames at 30fps)
const TIMING = timing;

export const Video: React.FC = () => {
  return (
    <AbsoluteFill style={{ backgroundColor: colors.background }}>
      {/* Intro Sequence */}
      <Sequence from={TIMING.intro.start} durationInFrames={TIMING.intro.duration}>
        <IntroSequence />
      </Sequence>

      {/* Workflow & Productivity */}
      <Sequence from={TIMING.workflow.start} durationInFrames={TIMING.workflow.duration}>
        <CategorySection
          category={categories[0]}
          startFrame={0}
          duration={TIMING.workflow.duration}
        />
      </Sequence>

      {/* Scene & Hierarchy */}
      <Sequence from={TIMING.scene.start} durationInFrames={TIMING.scene.duration}>
        <CategorySection
          category={categories[1]}
          startFrame={0}
          duration={TIMING.scene.duration}
        />
      </Sequence>

      {/* Asset Utilities */}
      <Sequence from={TIMING.assets.start} durationInFrames={TIMING.assets.duration}>
        <CategorySection
          category={categories[2]}
          startFrame={0}
          duration={TIMING.assets.duration}
        />
      </Sequence>


      {/* Outro Sequence */}
      <Sequence from={TIMING.outro.start} durationInFrames={TIMING.outro.duration}>
        <OutroSequence startFrame={0} />
      </Sequence>
    </AbsoluteFill>
  );
};
