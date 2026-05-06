import {
  buildStoreFrontView,
  getInventory,
  normalizeQuery,
} from "@/data/mockData";
import { StoreFront } from "@/components/StoreFront";

const PAGE_SIZE = 40;
export const dynamic = "force-dynamic";

const toSingleValue = (value?: string | string[]) => {
  if (Array.isArray(value)) {
    return value[0] || "";
  }

  return value || "";
};

export default async function SSRPage({
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
  const query = normalizeQuery({
    scenario: toSingleValue(resolvedSearchParams.scenario),
    q: toSingleValue(resolvedSearchParams.q),
    category: toSingleValue(resolvedSearchParams.category),
    sort: toSingleValue(resolvedSearchParams.sort),
    page: toSingleValue(resolvedSearchParams.page),
  });
  const inventory = getInventory(query.scenario);
  const view = buildStoreFrontView(inventory, query, PAGE_SIZE);

  return (
    <StoreFront
      products={view.items}
      query={query}
      totalItems={view.totalItems}
      page={view.page}
      totalPages={view.totalPages}
      summary={view.summary}
      basePath="/ssr"
      isClient={false}
    />
  );
}
