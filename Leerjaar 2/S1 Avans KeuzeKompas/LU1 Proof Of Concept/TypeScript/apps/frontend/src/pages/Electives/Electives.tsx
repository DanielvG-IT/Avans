import { ElectiveCreateDialog } from "@/components/elective/ElectiveFormDialog";
import ElectiveFilter from "@/components/elective/ElectiveFilter";
import { RoleProtected } from "@/components/auth/RoleProtected";
import ElectiveCard from "@/components/elective/ElectiveCard";
import { Search, ArrowUpDown, Plus, ChevronLeft, ChevronRight } from "lucide-react";
import { useElectives } from "@/hooks/useElectives";
import React, { useEffect, useState } from "react";
import type { Elective } from "@/types/Elective";
import ErrorState from "@/components/ErrorState";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  SelectTrigger,
  SelectContent,
  SelectValue,
  SelectItem,
  Select,
} from "@/components/ui/select";

export default function ElectivePage(): React.ReactNode {
  // Use the custom hook for fetching electives
  const { electives: electivesData, loading, error, refetch } = useElectives();

  // UI state
  const [electives, setElectives] = useState<Elective[]>([]);
  const [filtered, setFiltered] = useState<Elective[]>([]);
  const [query, setQuery] = useState("");
  const [sortBy, setSortBy] = useState("name");
  const [createDialogOpen, setCreateDialogOpen] = useState(false);

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);

  // Filter states
  const [filters, setFilters] = useState({
    language: "all",
    provider: "all",
    level: "all",
    location: "all",
    credits: "all",
    period: "all",
  });

  // Update local state when data is fetched
  useEffect(() => {
    if (electivesData) {
      setElectives(electivesData);
    }
  }, [electivesData]);

  // Filtering & sorting logic
  useEffect(() => {
    let result = [...electives];

    // 🔍 Search
    if (query) {
      result = result.filter((m) =>
        [m.name, m.description, m.provider, m.code].some((f) =>
          f.toLowerCase().includes(query.toLowerCase()),
        ),
      );
    }

    // 🎯 Filters
    (Object.entries(filters) as [keyof typeof filters, string][]).forEach(([key, value]) => {
      if (value !== "all") {
        const electiveKey = key as keyof Elective;
        result = result.filter((m) => {
          const prop = m[electiveKey];
          if (prop === undefined || prop === null) return false;
          if (Array.isArray(prop)) {
            // If the elective property is an array, check if any element matches the filter value
            return prop.some((p) => String(p).toLowerCase() === value.toLowerCase());
          }
          return String(prop).toLowerCase() === value.toLowerCase();
        });
      }
    });

    // 🔢 Sort
    result.sort((a, b) => {
      if (sortBy === "name") return a.name.localeCompare(b.name);
      if (sortBy === "credits") return b.credits - a.credits;
      return 0;
    });

    setFiltered(result);
    setCurrentPage(1); // Reset to first page when filters change
  }, [electives, query, sortBy, filters]);

  // Calculate pagination
  const totalPages = Math.ceil(filtered.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const paginatedElectives = filtered.slice(startIndex, endIndex);

  if (loading) return <ErrorState loading={true} />;
  if (error) return <ErrorState error={error} />;
  if (electives.length === 0) return <ErrorState itemsCount={0} />;

  // Helper for unique dropdown values
  const uniqueValues = (key: keyof Elective): { value: string; label: React.ReactNode }[] => {
    const values = electives
      .map((m) => {
        const value = m[key];
        if (Array.isArray(value)) return undefined; // ignore arrays like tags
        if (value === undefined || value === null) return undefined;
        return String(value);
      })
      .filter((v): v is string => !!v);

    const unique = Array.from(new Set(values));

    return [
      { value: "all", label: "All" },
      ...unique.map((v) => ({
        value: v,
        label: v,
      })),
    ];
  };

  return (
    <div className="flex flex-col lg:flex-row max-w-7xl mx-auto p-3 sm:p-4 md:p-6 gap-4 md:gap-6 bg-background min-h-screen">
      {/* --- FILTER SIDEBAR --- */}
      <aside className="lg:w-64 w-full lg:sticky lg:top-20 self-start lg:h-[calc(100vh-6rem)] overflow-y-auto bg-card border border-border rounded-xl shadow-sm p-3 sm:p-4">
        <h2 className="text-base sm:text-lg font-semibold mb-3 sm:mb-4 text-card-foreground">Filters</h2>
        <div className="flex flex-col gap-3 sm:gap-4">
          <ElectiveFilter
            options={uniqueValues("language")}
            onChange={(value) => setFilters((prev) => ({ ...prev, language: value }))}
            defaultValue={filters.language}
            placeholder="Language"
          />

          <ElectiveFilter
            options={uniqueValues("provider")}
            onChange={(value) => setFilters((prev) => ({ ...prev, provider: value }))}
            defaultValue={filters.provider}
            placeholder="Provider"
          />

          <ElectiveFilter
            options={uniqueValues("level")}
            onChange={(value) => setFilters((prev) => ({ ...prev, level: value }))}
            defaultValue={filters.level}
            placeholder="Level"
          />

          <ElectiveFilter
            options={uniqueValues("location")}
            onChange={(value) => setFilters((prev) => ({ ...prev, location: value }))}
            defaultValue={filters.location}
            placeholder="Location"
          />

          <ElectiveFilter
            options={uniqueValues("credits")}
            onChange={(value) => setFilters((prev) => ({ ...prev, credits: value }))}
            defaultValue={filters.credits}
            placeholder="Credits"
          />

          <ElectiveFilter
            options={uniqueValues("period")}
            onChange={(value) => setFilters((prev) => ({ ...prev, period: value }))}
            defaultValue={filters.period}
            placeholder="Period"
          />
        </div>
      </aside>

      {/* --- MAIN CONTENT --- */}
      <main className="flex-1 flex flex-col gap-4 sm:gap-6 min-w-0">
        {/* Header with Create Button for Admins */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
          <h1 className="text-xl sm:text-2xl font-bold text-foreground">Electives</h1>
          <RoleProtected allowedRoles="admin">
            <Button onClick={() => setCreateDialogOpen(true)} className="gap-2 w-full sm:w-auto">
              <Plus className="h-4 w-4" />
              <span className="sm:inline">Create Elective</span>
            </Button>
          </RoleProtected>
        </div>

        {/* Search + Sort + Items per page */}
        <div className="flex flex-col gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground pointer-events-none" />
            <Input
              placeholder="Search electives..."
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              className="pl-9"
            />
          </div>

          <div className="flex flex-col xs:flex-row gap-2">
            <Select onValueChange={(value) => setItemsPerPage(Number(value))} defaultValue="10">
              <SelectTrigger className="w-full xs:w-[120px]">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="5">5 / page</SelectItem>
                <SelectItem value="10">10 / page</SelectItem>
                <SelectItem value="20">20 / page</SelectItem>
                <SelectItem value="50">50 / page</SelectItem>
              </SelectContent>
            </Select>

            <Select onValueChange={setSortBy} defaultValue="name">
              <SelectTrigger className="w-full xs:w-[160px]">
                <ArrowUpDown className="mr-2 h-4 w-4" />
                <SelectValue placeholder="Sort by" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="name">Name (A–Z)</SelectItem>
                <SelectItem value="credits">Credits (High–Low)</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>

        {/* Results Info */}
        {filtered.length > 0 && (
          <div className="text-xs sm:text-sm text-muted-foreground">
            Showing {startIndex + 1}–{Math.min(endIndex, filtered.length)} of {filtered.length}{" "}
            electives
          </div>
        )}

        {/* Results Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4 sm:gap-6">
          {paginatedElectives.map((m) => (
            <ElectiveCard key={m.id} elective={m} />
          ))}
        </div>

        {filtered.length === 0 && (
          <div className="text-center text-muted-foreground mt-10 sm:mt-20 p-6 sm:p-8 bg-card border border-border rounded-lg">
            <p className="text-base sm:text-lg font-medium text-card-foreground">No electives found</p>
            <p className="text-sm mt-2">Try adjusting your filters or search query.</p>
          </div>
        )}

        {/* Pagination Controls */}
        {filtered.length > 0 && totalPages > 1 && (
          <div className="flex flex-col xs:flex-row items-center justify-center gap-3 mt-6">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
              disabled={currentPage === 1}
              className="w-full xs:w-auto"
            >
              <ChevronLeft className="h-4 w-4" />
              <span className="ml-1">Previous</span>
            </Button>

            <div className="flex items-center gap-1 flex-wrap justify-center">
              {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => {
                // Show first page, last page, current page, and pages around current
                if (
                  page === 1 ||
                  page === totalPages ||
                  (page >= currentPage - 1 && page <= currentPage + 1)
                ) {
                  return (
                    <Button
                      key={page}
                      variant={currentPage === page ? "default" : "outline"}
                      size="sm"
                      onClick={() => setCurrentPage(page)}
                      className="w-9 h-9"
                    >
                      {page}
                    </Button>
                  );
                } else if (page === currentPage - 2 || page === currentPage + 2) {
                  return (
                    <span key={page} className="px-2 text-muted-foreground">
                      ...
                    </span>
                  );
                }
                return null;
              })}
            </div>

            <Button
              variant="outline"
              size="sm"
              onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))}
              disabled={currentPage === totalPages}
              className="w-full xs:w-auto"
            >
              <span className="mr-1">Next</span>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        )}
      </main>

      {/* Create Elective Dialog */}
      <ElectiveCreateDialog
        open={createDialogOpen}
        onOpenChange={setCreateDialogOpen}
        onSuccess={refetch}
      />
    </div>
  );
}
