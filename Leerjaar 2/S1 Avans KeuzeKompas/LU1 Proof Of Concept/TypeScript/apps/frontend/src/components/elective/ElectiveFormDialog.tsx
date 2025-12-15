import { useState, useMemo, useEffect } from "react";
import { toast } from "sonner";
import { electivesApi, userApi } from "@/services/api.service";
import { useElectives } from "@/hooks/useElectives";
import type { Elective } from "@/types/Elective";
import type { TeacherUser } from "@/types/User";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface ElectiveFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSuccess?: () => void;
  mode: "create" | "edit";
  elective?: Elective; // Required when mode is "edit"
}

interface ElectiveCreateDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSuccess?: () => void;
}

export function ElectiveCreateDialog({ open, onOpenChange, onSuccess }: ElectiveCreateDialogProps) {
  return (
    <ElectiveFormDialog
      open={open}
      onOpenChange={onOpenChange}
      onSuccess={onSuccess}
      mode="create"
    />
  );
}

interface ElectiveEditDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  elective: Elective;
  onSuccess?: () => void;
}

export function ElectiveEditDialog({
  open,
  onOpenChange,
  elective,
  onSuccess,
}: ElectiveEditDialogProps) {
  return (
    <ElectiveFormDialog
      open={open}
      onOpenChange={onOpenChange}
      onSuccess={onSuccess}
      mode="edit"
      elective={elective}
    />
  );
}

interface ElectiveFormData {
  code: string;
  name: string;
  description: string;
  provider: string;
  period: string;
  duration: string;
  credits: string;
  language: string;
  location: string;
  level: string;
  tags: string;
  teacherIds: string[];
}

const initialFormData: ElectiveFormData = {
  code: "",
  name: "",
  description: "",
  provider: "",
  period: "",
  duration: "",
  credits: "",
  language: "NL",
  location: "",
  level: "",
  tags: "",
  teacherIds: [],
};

function ElectiveFormDialog({
  open,
  onOpenChange,
  onSuccess,
  mode,
  elective,
}: ElectiveFormDialogProps) {
  const [formData, setFormData] = useState<ElectiveFormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [teachers, setTeachers] = useState<TeacherUser[]>([]);
  const [loadingTeachers, setLoadingTeachers] = useState(false);

  const { electives } = useElectives();

  useEffect(() => {
    if (mode === "edit" && elective && open) {
      setFormData({
        code: elective.code || "",
        name: elective.name || "",
        description: elective.description || "",
        provider: elective.provider || "",
        period: elective.period || "",
        duration: elective.duration || "",
        credits: elective.credits?.toString() || "",
        language: elective.language || "NL",
        location: elective.location || "",
        level: elective.level || "",
        tags: elective.tags?.join(", ") || "",
        teacherIds:
          elective.teachers?.map((t) => t.id).filter((id): id is string => id != null) || [],
      });
    } else if (mode === "create" && open) {
      setFormData(initialFormData);
    }
  }, [elective, open, mode]);

  useEffect(() => {
    const fetchTeachers = async () => {
      setLoadingTeachers(true);
      try {
        const users = await userApi.getAll();
        setTeachers(users.filter((user): user is TeacherUser => user.role === "teacher"));
      } catch (error) {
        console.error("Failed to fetch teachers:", error);
        toast.error("Failed to load teachers");
      } finally {
        setLoadingTeachers(false);
      }
    };

    if (open) fetchTeachers();
  }, [open]);

  const getUniqueValues = useMemo(() => {
    return (key: keyof Elective): string[] => {
      if (!electives) return [];
      const values = electives
        .map((elective) => {
          const value = elective[key];
          return Array.isArray(value) ? undefined : value ? String(value) : undefined;
        })
        .filter(Boolean) as string[];
      return Array.from(new Set(values)).sort();
    };
  }, [electives]);

  const handleChange = (field: keyof ElectiveFormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (
      !formData.code ||
      !formData.name ||
      !formData.description ||
      !formData.provider ||
      !formData.location ||
      !formData.period ||
      !formData.duration ||
      !formData.level ||
      !formData.language
    ) {
      toast.error("Please fill in all required fields");
      return;
    }

    const credits = parseInt(formData.credits);
    if (isNaN(credits) || credits <= 0) {
      toast.error("Credits must be a positive number");
      return;
    }

    if (mode === "edit" && !elective?.id) {
      toast.error("Invalid elective ID");
      return;
    }

    setLoading(true);
    try {
      const baseElectiveData = {
        code: formData.code,
        name: formData.name,
        description: formData.description,
        provider: formData.provider,
        period: formData.period,
        duration: formData.duration,
        credits,
        language: formData.language,
        location: formData.location,
        level: formData.level,
        tags: formData.tags ? formData.tags.split(",").map((tag) => tag.trim()) : undefined,
      };

      if (mode === "create") {
        const createdElective = await electivesApi.create(baseElectiveData);
        if (formData.teacherIds.length > 0 && createdElective.id) {
          try {
            await Promise.all(
              formData.teacherIds.map((teacherId) =>
                electivesApi.assignTeacher(createdElective.id!, teacherId),
              ),
            );
          } catch (assignError) {
            console.error("Failed to assign some teachers:", assignError);
            toast.warning("Elective created but some teachers could not be assigned");
          }
        }
        toast.success("Elective created successfully!");
      } else {
        await electivesApi.update(elective!.id!, baseElectiveData);

        const currentTeacherIds =
          elective!.teachers?.map((t) => t.id).filter((id): id is string => id != null) || [];
        const newTeacherIds = formData.teacherIds;
        const teachersToAdd = newTeacherIds.filter((id) => !currentTeacherIds.includes(id));
        const teachersToRemove = currentTeacherIds.filter((id) => !newTeacherIds.includes(id));

        if (teachersToAdd.length > 0) {
          try {
            await Promise.all(
              teachersToAdd.map((teacherId) =>
                electivesApi.assignTeacher(elective!.id!, teacherId),
              ),
            );
          } catch (assignError) {
            console.error("Failed to assign some teachers:", assignError);
            toast.warning("Elective updated but some teachers could not be assigned");
          }
        }

        if (teachersToRemove.length > 0) {
          try {
            await Promise.all(
              teachersToRemove.map((teacherId) =>
                electivesApi.unassignTeacher(elective!.id!, teacherId),
              ),
            );
          } catch (unassignError) {
            console.error("Failed to unassign some teachers:", unassignError);
            toast.warning("Elective updated but some teachers could not be unassigned");
          }
        }

        toast.success("Elective updated successfully!");
      }

      setFormData(initialFormData);
      onOpenChange(false);
      onSuccess?.();
    } catch (error) {
      console.error(`Failed to ${mode} elective:`, error);
      toast.error(error instanceof Error ? error.message : `Failed to ${mode} elective`);
    } finally {
      setLoading(false);
    }
  };

  const dialogTitle = mode === "create" ? "Create New Elective" : "Edit Elective";
  const dialogDescription =
    mode === "create"
      ? "Add a new elective to the system. All fields are required unless marked optional."
      : "Update the elective information. All fields are required unless marked optional.";
  const submitButtonText = loading
    ? mode === "create"
      ? "Creating..."
      : "Updating..."
    : mode === "create"
      ? "Create Elective"
      : "Update Elective";

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{dialogTitle}</DialogTitle>
          <DialogDescription>{dialogDescription}</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="code">
                Code <span className="text-destructive">*</span>
              </label>
              <Input
                id="code"
                value={formData.code}
                onChange={(e) => handleChange("code", e.target.value)}
                placeholder="e.g., BICT-V1KEU01"
                required
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="credits">
                Credits (EC) <span className="text-destructive">*</span>
              </label>
              <Input
                id="credits"
                type="number"
                min="0"
                value={formData.credits}
                onChange={(e) => handleChange("credits", e.target.value)}
                placeholder="e.g., 5"
                required
              />
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="name">
              Name <span className="text-destructive">*</span>
            </label>
            <Input
              id="name"
              value={formData.name}
              onChange={(e) => handleChange("name", e.target.value)}
              placeholder="e.g., Advanced Web Development"
              required
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="description">
              Description <span className="text-destructive">*</span>
            </label>
            <textarea
              id="description"
              value={formData.description}
              onChange={(e) => handleChange("description", e.target.value)}
              placeholder="Detailed description of the elective..."
              required
              rows={4}
              className="w-full px-3 py-2 text-sm border border-input rounded-md bg-background focus:outline-none focus:ring-2 focus:ring-ring resize-none"
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="provider">
                Provider <span className="text-destructive">*</span>
              </label>
              <Select
                value={formData.provider}
                onValueChange={(value) => handleChange("provider", value)}
              >
                <SelectTrigger id="provider">
                  <SelectValue placeholder="Select a provider" />
                </SelectTrigger>
                <SelectContent>
                  {getUniqueValues("provider").map((provider) => (
                    <SelectItem key={provider} value={provider}>
                      {provider}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="location">
                Location <span className="text-destructive">*</span>
              </label>
              <Select
                value={formData.location}
                onValueChange={(value) => handleChange("location", value)}
              >
                <SelectTrigger id="location">
                  <SelectValue placeholder="Select a location" />
                </SelectTrigger>
                <SelectContent>
                  {getUniqueValues("location").map((location) => (
                    <SelectItem key={location} value={location}>
                      {location}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="period">
                Period <span className="text-destructive">*</span>
              </label>
              <Select
                value={formData.period}
                onValueChange={(value) => handleChange("period", value)}
              >
                <SelectTrigger id="period">
                  <SelectValue placeholder="Select period" />
                </SelectTrigger>
                <SelectContent>
                  {getUniqueValues("period").map((period) => (
                    <SelectItem key={period} value={period}>
                      {period}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="duration">
                Duration <span className="text-destructive">*</span>
              </label>
              <Select
                value={formData.duration}
                onValueChange={(value) => handleChange("duration", value)}
              >
                <SelectTrigger id="duration">
                  <SelectValue placeholder="Select duration" />
                </SelectTrigger>
                <SelectContent>
                  {getUniqueValues("duration").map((duration) => (
                    <SelectItem key={duration} value={duration}>
                      {duration}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium" htmlFor="level">
                Level <span className="text-destructive">*</span>
              </label>
              <Select
                value={formData.level}
                onValueChange={(value) => handleChange("level", value)}
              >
                <SelectTrigger id="level">
                  <SelectValue placeholder="Select level" />
                </SelectTrigger>
                <SelectContent>
                  {getUniqueValues("level").map((level) => (
                    <SelectItem key={level} value={level}>
                      {level}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="language">
              Language <span className="text-destructive">*</span>
            </label>
            <Select
              value={formData.language}
              onValueChange={(value) => handleChange("language", value)}
            >
              <SelectTrigger id="language">
                <SelectValue placeholder="Select a language" />
              </SelectTrigger>
              <SelectContent>
                {mode === "create" ? (
                  getUniqueValues("language").map((language) => (
                    <SelectItem key={language} value={language}>
                      {language}
                    </SelectItem>
                  ))
                ) : (
                  <>
                    <SelectItem value="NL">Nederlands</SelectItem>
                    <SelectItem value="EN">English</SelectItem>
                  </>
                )}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium" htmlFor="tags">
              Tags <span className="text-muted-foreground">(optional, comma-separated)</span>
            </label>
            <Input
              id="tags"
              value={formData.tags}
              onChange={(e) => handleChange("tags", e.target.value)}
              placeholder="e.g., programming, web, frontend"
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">
              {mode === "create" ? "Teachers" : "Assign Teachers"}{" "}
              <span className="text-muted-foreground">(optional)</span>
            </label>
            <div className="border border-input rounded-md p-3 max-h-48 overflow-y-auto bg-background">
              {loadingTeachers ? (
                <p className="text-sm text-muted-foreground">Loading teachers...</p>
              ) : teachers.length === 0 ? (
                <p className="text-sm text-muted-foreground">No teachers available</p>
              ) : (
                <div className="space-y-2">
                  {teachers.map((teacher) => (
                    <label
                      key={teacher.id}
                      className="flex items-center gap-2 cursor-pointer hover:bg-muted/50 p-2 rounded"
                    >
                      <input
                        type="checkbox"
                        checked={formData.teacherIds.includes(teacher.id!)}
                        onChange={(e) => {
                          const checked = e.target.checked;
                          setFormData((prev) => ({
                            ...prev,
                            teacherIds: checked
                              ? [...prev.teacherIds, teacher.id!]
                              : prev.teacherIds.filter((id) => id !== teacher.id),
                          }));
                        }}
                        className="h-4 w-4 rounded border-input"
                      />
                      <span className="text-sm">
                        {teacher.firstName} {teacher.lastName}
                      </span>
                      <span className="text-xs text-muted-foreground ml-auto">{teacher.email}</span>
                    </label>
                  ))}
                </div>
              )}
            </div>
            {formData.teacherIds.length > 0 && (
              <p className="text-xs text-muted-foreground">
                {formData.teacherIds.length} teacher(s) selected
              </p>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={loading}>
              {submitButtonText}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
