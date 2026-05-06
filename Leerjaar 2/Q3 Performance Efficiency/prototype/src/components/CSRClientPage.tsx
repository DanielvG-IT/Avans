"use client";

import {
  startTransition,
  useEffect,
  useMemo,
  useState,
  useDeferredValue,
} from "react";
import {
  buildStoreFrontView,
  type BenchmarkQuery,
  type Product,
} from "@/data/mockData";
import { StoreFront } from "@/components/StoreFront";

const PAGE_SIZE = 40;

export const CSRClientPage = ({
  initialQuery,
}: {
  initialQuery: BenchmarkQuery;
}) => {
  const [query, setQuery] = useState<BenchmarkQuery>(initialQuery);
  const [inventory, setInventory] = useState<Product[] | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const deferredQuery = useDeferredValue(query);

  useEffect(() => {
    let cancelled = false;

    const loadInventory = async () => {
      setIsLoading(true);
      setInventory(null);

      try {
        const response = await fetch(
          `/api/inventory?scenario=${encodeURIComponent(query.scenario)}`,
          {
            cache: "no-store",
          },
        );
        if (!response.ok) {
          throw new Error(`Inventory request failed with ${response.status}`);
        }
        const payload = (await response.json()) as { inventory: Product[] };

        if (!cancelled) {
          startTransition(() => {
            setInventory(payload.inventory);
          });
        }
      } catch (error) {
        if (!cancelled) {
          console.error("[csr] inventory fetch failed", error);
        }
      } finally {
        if (!cancelled) {
          setIsLoading(false);
        }
      }
    };

    void loadInventory();

    return () => {
      cancelled = true;
    };
  }, [query.scenario]);

  const view = useMemo(
    () =>
      inventory
        ? buildStoreFrontView(inventory, deferredQuery, PAGE_SIZE)
        : {
            items: [],
            totalItems: 0,
            page: 1,
            totalPages: 1,
            summary: {
              scenarioLabel: "Loading inventory",
              datasetSize: 0,
              averagePrice: 0,
              premiumCount: 0,
              expressReadyCount: 0,
              merchIndex: 0,
              categoryBreakdown: [],
            },
          },
    [inventory, deferredQuery],
  );

  const handleQueryChange = (updates: Partial<BenchmarkQuery>) => {
    startTransition(() => {
      setQuery((prev) => ({ ...prev, ...updates }));
    });
  };

  return (
    <StoreFront
      products={view.items}
      query={query}
      totalItems={view.totalItems}
      page={view.page}
      totalPages={view.totalPages}
      summary={view.summary}
      onQueryChange={handleQueryChange}
      onPageChange={(nextPage) => handleQueryChange({ page: nextPage })}
      isClient={true}
      isLoading={isLoading}
    />
  );
};
