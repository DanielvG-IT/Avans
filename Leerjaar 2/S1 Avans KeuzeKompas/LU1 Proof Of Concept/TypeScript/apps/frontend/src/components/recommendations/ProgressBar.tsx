// Progress bar component

interface ProgressBarProps {
  percentage: number;
}

export default function ProgressBar({ percentage }: ProgressBarProps) {
  return (
    <div className="w-full bg-muted/20 rounded-full h-2 overflow-hidden">
      <div
        className="h-2 rounded-full bg-primary transition-all"
        style={{ width: `${percentage}%` }}
      />
    </div>
  );
}
