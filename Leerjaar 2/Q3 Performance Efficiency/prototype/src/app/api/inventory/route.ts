import { NextRequest, NextResponse } from "next/server";
import { getInventory, normalizeQuery } from "@/data/mockData";

export const dynamic = "force-dynamic";
export const revalidate = 0;

export function GET(request: NextRequest) {
  const query = normalizeQuery({
    scenario: request.nextUrl.searchParams.get("scenario") || undefined,
  });

  return NextResponse.json(
    {
      scenario: query.scenario,
      inventory: getInventory(query.scenario),
    },
    {
      headers: {
        "Cache-Control": "no-store",
      },
    },
  );
}
