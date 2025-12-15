import { Outlet } from "react-router-dom";
import { ThemeToggleButton } from "@/components/ThemeToggle";

export const AuthLayout = () => {
  return (
    <main
      className="relative w-[100dvw] h-[100dvh] flex items-center justify-center p-6 bg-[#323333ff] dark:bg-[#1a1a1aff] bg-cover bg-center transition-colors"
      style={{
        backgroundImage:
          'url("https://www.avans.nl/binaries/_ht_1675236669544/ultrawideXL1/content/gallery/nextweb/nieuws/2023/02/openxbuitenaanzicht.jpg")',
        backgroundBlendMode: "overlay",
      }}
    >
      {/* Theme toggle in top right corner */}
      <div className="absolute top-4 right-4 z-10">
        <ThemeToggleButton />
      </div>
      <Outlet />
    </main>
  );
};
