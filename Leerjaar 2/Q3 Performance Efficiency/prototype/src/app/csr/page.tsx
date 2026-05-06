import { normalizeQuery } from "@/data/mockData";
import { CSRClientPage } from "@/components/CSRClientPage";

export const dynamic = "force-dynamic";

const toSingleValue = (value?: string | string[]) => {
  if (Array.isArray(value)) {
    return value[0] || "";
  }

  return value || "";
};

export default async function CSRPage({
  searchParams,
}: {
  searchParams: Promise<{
    scenario?: string | string[];
    q?: string | string[];
    category?: string | string[];
    sort?: string | string[];
    page?: string | string[];
  }>;
}) {
  const resolvedSearchParams = await searchParams;
  const initialQuery = normalizeQuery({
    scenario: toSingleValue(resolvedSearchParams.scenario),
    q: toSingleValue(resolvedSearchParams.q),
    category: toSingleValue(resolvedSearchParams.category),
    sort: toSingleValue(resolvedSearchParams.sort),
    page: toSingleValue(resolvedSearchParams.page),
  });

  return <CSRClientPage initialQuery={initialQuery} />;
}
