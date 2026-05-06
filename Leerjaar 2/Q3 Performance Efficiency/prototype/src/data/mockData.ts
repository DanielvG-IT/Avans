export interface Product {
  id: number;
  name: string;
  category: string;
  price: number;
  description: string;
  imageColor: string;
}

export type Scenario = "static" | "dynamic" | "massive";
export type SortKey = "name-asc" | "price-asc" | "price-desc";

export interface BenchmarkQuery {
  scenario: Scenario;
  q: string;
  category: string;
  sort: SortKey;
  page: number;
}

export interface DecoratedProduct extends Product {
  accentColor: string;
  inventoryLabel: string;
  shippingLabel: string;
  priceWithTax: string;
  merchScore: number;
  reviewCount: number;
}

export interface CategoryBreakdown {
  name: string;
  count: number;
  share: number;
  accentColor: string;
}

export interface StoreSummary {
  scenarioLabel: string;
  datasetSize: number;
  averagePrice: number;
  premiumCount: number;
  expressReadyCount: number;
  merchIndex: number;
  categoryBreakdown: CategoryBreakdown[];
}

export interface StoreFrontView {
  items: DecoratedProduct[];
  totalItems: number;
  page: number;
  totalPages: number;
  summary: StoreSummary;
}

export const categories = [
  "All",
  "Electronics",
  "Lifestyle",
  "Design",
  "Audio",
  "Home",
];
const colors = ["#f3f4f6", "#e5e7eb", "#fee2e2", "#fef3c7", "#dcfce7"];

const SCENARIO_SIZES: Record<Scenario, number> = {
  static: 72,
  dynamic: 6000,
  massive: 24000,
};

const scenarioLabels: Record<Scenario, string> = {
  static: "Editorial capsule",
  dynamic: "Live catalog",
  massive: "Marketplace sweep",
};

const DEFAULT_QUERY: BenchmarkQuery = {
  scenario: "dynamic",
  q: "",
  category: "All",
  sort: "name-asc",
  page: 1,
};

const compareBySort = (a: Product, b: Product, sort: SortKey): number => {
  if (sort === "price-asc") {
    return a.price - b.price;
  }

  if (sort === "price-desc") {
    return b.price - a.price;
  }

  return a.name.localeCompare(b.name);
};

const generateInventory = (length: number): Product[] => {
  return Array.from({ length }, (_, i) => ({
    id: i,
    name: `${categories[(i % (categories.length - 1)) + 1]} Item ${i + 1}`,
    category: categories[(i % (categories.length - 1)) + 1],
    price: ((i * 17) % 500) + 50,
    description: "Minimalist design meeting functional excellence.",
    imageColor: colors[i % colors.length],
  }));
};

const scenarioData: Record<Scenario, Product[]> = {
  static: generateInventory(SCENARIO_SIZES.static),
  dynamic: generateInventory(SCENARIO_SIZES.dynamic),
  massive: generateInventory(SCENARIO_SIZES.massive),
};

export const getInventory = (scenario: Scenario): Product[] => {
  return scenarioData[scenario];
};

export const normalizeQuery = (
  raw?: Partial<Record<string, string>>,
): BenchmarkQuery => {
  const scenario = raw?.scenario;
  const sort = raw?.sort;
  const page = Number(raw?.page || DEFAULT_QUERY.page);
  const category = raw?.category || DEFAULT_QUERY.category;

  const safeScenario: Scenario =
    scenario === "static" || scenario === "dynamic" || scenario === "massive"
      ? scenario
      : DEFAULT_QUERY.scenario;

  const safeSort: SortKey =
    sort === "name-asc" || sort === "price-asc" || sort === "price-desc"
      ? sort
      : DEFAULT_QUERY.sort;

  return {
    scenario: safeScenario,
    q: raw?.q || DEFAULT_QUERY.q,
    category: categories.includes(category) ? category : DEFAULT_QUERY.category,
    sort: safeSort,
    page:
      Number.isFinite(page) && page > 0 ? Math.floor(page) : DEFAULT_QUERY.page,
  };
};

export const filterAndSortProducts = (
  products: Product[],
  query: BenchmarkQuery,
): Product[] => {
  const q = query.q.toLowerCase();

  return products
    .filter((product) => {
      const matchesQuery = product.name.toLowerCase().includes(q);
      const matchesCategory =
        query.category === "All" || product.category === query.category;
      return matchesQuery && matchesCategory;
    })
    .sort((a, b) => compareBySort(a, b, query.sort));
};

export const paginateProducts = (
  products: Product[],
  page: number,
  pageSize: number,
) => {
  const safePageSize = Math.max(1, pageSize);
  const totalPages = Math.max(1, Math.ceil(products.length / safePageSize));
  const safePage = Math.min(Math.max(page, 1), totalPages);
  const start = (safePage - 1) * safePageSize;

  return {
    page: safePage,
    totalPages,
    items: products.slice(start, start + safePageSize),
  };
};

const decorateProduct = (
  product: Product,
  scenario: Scenario,
  position: number,
): DecoratedProduct => {
  const scenarioOffset =
    scenario === "static" ? 11 : scenario === "dynamic" ? 23 : 37;
  const merchScore =
    ((product.price * (position + 3) + scenarioOffset * 17) % 1000) / 10;
  const reviewCount = ((product.id + 1) * (scenarioOffset + 5)) % 480;
  const inventorySeed = (product.id * 19 + scenarioOffset + position) % 34;
  const inventoryLabel =
    inventorySeed < 6
      ? "Low stock"
      : inventorySeed < 20
        ? "Ready to ship"
        : "Warehouse replenishment";
  const shippingLabel =
    inventorySeed % 3 === 0 ? "Express eligible" : "Standard dispatch";
  const priceWithTax = `€${(product.price * 1.21).toFixed(2)}`;

  return {
    ...product,
    accentColor: colors[(product.id + scenarioOffset) % colors.length],
    inventoryLabel,
    shippingLabel,
    priceWithTax,
    merchScore: Math.round(merchScore * 10) / 10,
    reviewCount,
  };
};

const buildSummary = (
  products: Product[],
  scenario: Scenario,
): StoreSummary => {
  const counts = new Map<string, number>();
  let totalPrice = 0;
  let premiumCount = 0;
  let expressReadyCount = 0;
  let merchAccumulator = 0;

  for (let i = 0; i < products.length; i += 1) {
    const product = products[i];
    counts.set(product.category, (counts.get(product.category) || 0) + 1);
    totalPrice += product.price;

    if (product.price >= 320) {
      premiumCount += 1;
    }

    if (((product.id * 7) + i + scenario.length) % 3 === 0) {
      expressReadyCount += 1;
    }

    merchAccumulator += ((product.price * (i + 5)) % 97) + (product.id % 13);
  }

  const total = Math.max(products.length, 1);
  const categoryBreakdown = Array.from(counts.entries())
    .map(([name, count], index) => ({
      name,
      count,
      share: Math.round((count / total) * 1000) / 10,
      accentColor: colors[index % colors.length],
    }))
    .sort((a, b) => b.count - a.count);

  return {
    scenarioLabel: scenarioLabels[scenario],
    datasetSize: products.length,
    averagePrice: Math.round((totalPrice / total) * 100) / 100,
    premiumCount,
    expressReadyCount,
    merchIndex: Math.round((merchAccumulator / total) * 10) / 10,
    categoryBreakdown,
  };
};

export const buildStoreFrontView = (
  inventory: Product[],
  query: BenchmarkQuery,
  pageSize: number,
): StoreFrontView => {
  const filtered = filterAndSortProducts(inventory, query);
  const paged = paginateProducts(filtered, query.page, pageSize);

  return {
    items: paged.items.map((product, index) =>
      decorateProduct(product, query.scenario, index),
    ),
    totalItems: filtered.length,
    page: paged.page,
    totalPages: paged.totalPages,
    summary: buildSummary(filtered, query.scenario),
  };
};
