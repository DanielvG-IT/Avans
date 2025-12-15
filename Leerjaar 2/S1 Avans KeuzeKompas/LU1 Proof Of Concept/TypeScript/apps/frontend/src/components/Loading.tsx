import React from "react";
import keuzekompasLogo from "@/assets/keuzekompas.svg";

interface LoadingProps {
  isFullScreen?: boolean;
  message?: string;
  showLogo?: boolean;
  size?: "small" | "medium" | "large";
  className?: string;
}

const Loading: React.FC<LoadingProps> = ({
  isFullScreen = false,
  message = "Loading…",
  showLogo = true,
  size = "medium",
  className = "",
}) => {
  const sizeStyles = {
    small: { logoWidth: 60, spinnerSize: 50, fontSize: 16 },
    medium: { logoWidth: 120, spinnerSize: 100, fontSize: 24 },
    large: { logoWidth: 180, spinnerSize: 150, fontSize: 32 },
  };

  const { logoWidth, spinnerSize, fontSize } = sizeStyles[size];

  const containerStyle: React.CSSProperties = {
    ...(isFullScreen
      ? {
          height: "100vh",
          position: "relative",
          overflow: "hidden",
        }
      : {
          position: "relative",
          overflow: "hidden",
          padding: "20px",
        }),
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    flexDirection: "column",
    gap: 24,
    background: isFullScreen ? "linear-gradient(135deg, #667eea 0%, #764ba2 100%)" : "transparent",
    fontFamily: "'Inter', sans-serif",
  };

  const particlesStyle: React.CSSProperties = {
    position: "absolute",
    top: 0,
    left: 0,
    width: "100%",
    height: "100%",
    backgroundImage: "radial-gradient(circle, rgba(255,255,255,0.1) 1px, transparent 1px)",
    backgroundSize: "20px 20px",
    animation: "float 10s ease-in-out infinite",
  };

  return (
    <div role="status" aria-live="polite" className={className} style={containerStyle}>
      {isFullScreen && <div style={particlesStyle} />}

      {showLogo && (
        <img
          src={keuzekompasLogo}
          alt="Avans Keuzekompas Logo"
          style={{
            width: logoWidth,
            height: "auto",
            filter: "drop-shadow(0px 8px 16px rgba(0, 0, 0, 0.3))",
            animation: "bounce 2s ease-in-out infinite",
          }}
        />
      )}

      <svg
        width={spinnerSize}
        height={spinnerSize}
        viewBox="0 0 64 64"
        xmlns="http://www.w3.org/2000/svg"
        aria-hidden="true"
        style={{
          filter: "drop-shadow(0px 4px 8px rgba(0, 0, 0, 0.2))",
        }}
      >
        <circle cx="32" cy="32" r="28" stroke="rgba(255,255,255,0.3)" strokeWidth="6" fill="none" />
        <circle
          cx="32"
          cy="32"
          r="28"
          stroke="#ffffff"
          strokeWidth="6"
          strokeLinecap="round"
          fill="none"
          strokeDasharray="80 176"
        >
          <animateTransform
            attributeName="transform"
            type="rotate"
            from="0 32 32"
            to="360 32 32"
            dur="1s"
            repeatCount="indefinite"
          />
        </circle>
      </svg>

      <div
        className="text-foreground dark:text-white"
        style={{
          fontSize,
          fontWeight: 700,
          textShadow: isFullScreen ? "0px 4px 8px rgba(0, 0, 0, 0.3)" : "none",
          animation: "pulse 1.5s ease-in-out infinite",
        }}
      >
        {message}
      </div>

      {/* Inline CSS animations */}
      <style>
        {`
          @keyframes bounce {
            0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
            40% { transform: translateY(-10px); }
            60% { transform: translateY(-5px); }
          }
          @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.5; }
          }
          @keyframes float {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-10px); }
          }
        `}
      </style>
    </div>
  );
};

export default Loading;
