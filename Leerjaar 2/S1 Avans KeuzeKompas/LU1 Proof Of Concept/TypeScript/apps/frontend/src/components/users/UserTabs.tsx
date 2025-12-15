interface UserTabsProps {
  activeTab: "students" | "teachers";
  onTabChange: (tab: "students" | "teachers") => void;
  studentCount: number;
  teacherCount: number;
}

export function UserTabs({ activeTab, onTabChange, studentCount, teacherCount }: UserTabsProps) {
  return (
    <div className="flex gap-2 mb-6 border-b">
      <button
        onClick={() => onTabChange("students")}
        className={`px-6 py-3 font-medium transition-colors relative ${
          activeTab === "students" ? "text-primary" : "text-muted-foreground hover:text-foreground"
        }`}
      >
        Students ({studentCount})
        {activeTab === "students" && (
          <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-primary" />
        )}
      </button>
      <button
        onClick={() => onTabChange("teachers")}
        className={`px-6 py-3 font-medium transition-colors relative ${
          activeTab === "teachers" ? "text-primary" : "text-muted-foreground hover:text-foreground"
        }`}
      >
        Teachers ({teacherCount})
        {activeTab === "teachers" && (
          <div className="absolute bottom-0 left-0 right-0 h-0.5 bg-primary" />
        )}
      </button>
    </div>
  );
}
