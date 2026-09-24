import { describe, expect, it } from "vitest"
import { filterHomeStarCards } from "@/lib/home/stars-filters"
import type { HomeStarCard } from "@/lib/home/types"

const cards: HomeStarCard[] = [
  {
    id: 1,
    href: "/reports?id=1",
    title: "Daily Emergency Department Census",
    itemType: "report",
    folderId: null,
    typeLabel: "Report",
    description: "Census report",
  },
  {
    id: 2,
    href: "/collections?id=2",
    title: "Patient Flow Command Center",
    itemType: "collection",
    folderId: 10,
    typeLabel: "Collection",
    description: "Collection",
  },
  {
    id: 3,
    href: "/initiatives?id=3",
    title: "Improve Patient Flow",
    itemType: "initiative",
    folderId: 10,
    typeLabel: "Initiative",
    description: "Initiative",
  },
]

describe("filterHomeStarCards", () => {
  it("filters by folder", () => {
    const filtered = filterHomeStarCards(cards, {
      folderId: "10",
      typeFilter: null,
      textFilter: "",
    })

    expect(filtered).toHaveLength(2)
  })

  it("filters by quick filter type", () => {
    const filtered = filterHomeStarCards(cards, {
      folderId: "all",
      typeFilter: "report",
      textFilter: "",
    })

    expect(filtered).toHaveLength(1)
    expect(filtered[0]?.title).toBe("Daily Emergency Department Census")
  })

  it("filters by text query", () => {
    const filtered = filterHomeStarCards(cards, {
      folderId: "all",
      typeFilter: null,
      textFilter: "patient flow",
    })

    expect(filtered).toHaveLength(2)
  })
})
