import { describe, expect, it } from "vitest"
import { mapUserStarsPayloadToPanel, normalizeHomeStarItemType } from "@/lib/home/stars-mapper"

describe("normalizeHomeStarItemType", () => {
  it("prefers the API type over display labels like SSRS", () => {
    expect(normalizeHomeStarItemType("report", "SSRS")).toBe("report")
  })

  it("falls back to collection and initiative labels", () => {
    expect(normalizeHomeStarItemType(null, "Collection")).toBe("collection")
    expect(normalizeHomeStarItemType(null, "Initiative")).toBe("initiative")
  })
})

describe("mapUserStarsPayloadToPanel", () => {
  it("maps demo-admin-like stars with Razor folder counts and shared objects", () => {
    const panel = mapUserStarsPayloadToPanel(
      {
        summary: {
          totalCount: 14,
          unsortedCount: 0,
          hasFolders: true,
          showUnsortedBucket: false,
        },
        filters: {
          hasReports: true,
          hasCollections: true,
          hasInitiatives: true,
          hasTerms: true,
          hasGroups: true,
          hasUsers: false,
          hasSearches: false,
          showQuickFilters: true,
        },
        folders: [
          { id: 1, name: "Daily Operations", itemCount: 7 },
          { id: 2, name: "Executive Review", itemCount: 7 },
        ],
        items: [
          {
            starId: 1,
            type: "report",
            typeLabel: "SSRS",
            itemId: 16,
            name: "Daily Emergency Department Census",
            url: "/reports?id=16",
            rank: 1,
          },
          {
            starId: 2,
            type: "initiative",
            typeLabel: "initiative",
            itemId: 3,
            name: "Improve Patient Flow",
            url: "/initiatives?id=3",
            rank: 2,
          },
          {
            starId: 3,
            type: "collection",
            typeLabel: "collection",
            itemId: 4,
            name: "Patient Flow Command Center",
            url: "/collections?id=4",
            rank: 3,
          },
        ],
        suggestedReports: [],
      },
      [
        {
          id: 99,
          name: "Inpatient Length of Stay",
          href: "/reports?id=2",
          sharedFrom: "Maya Patel",
          shareDate: "8/11/2026",
        },
      ],
    )

    expect(panel.kind).toBe("stars")
    expect(panel.folders[0]).toEqual({ id: "all", label: "All", count: 14 })
    expect(panel.folders.some((folder) => folder.id === "unsorted")).toBe(false)
    expect(panel.filters.map((filter) => filter.id)).toEqual([
      "report",
      "collection",
      "initiative",
      "term",
      "group",
    ])
    expect(panel.cards.map((card) => card.title)).toEqual([
      "Daily Emergency Department Census",
      "Improve Patient Flow",
      "Patient Flow Command Center",
    ])
    expect(panel.cards[0]?.itemType).toBe("report")
    expect(panel.sharedWithMe).toHaveLength(1)
  })

  it("keeps card count aligned with summary total for starred items", () => {
    const items = Array.from({ length: 14 }, (_, index) => ({
      starId: index + 1,
      type: index % 3 === 0 ? "report" : index % 3 === 1 ? "collection" : "term",
      typeLabel: "Item",
      itemId: index + 100,
      name: `Favorite ${index + 1}`,
      url: `/reports?id=${index + 100}`,
      rank: index + 1,
    }))

    const panel = mapUserStarsPayloadToPanel({
      summary: { totalCount: 14, unsortedCount: 0, hasFolders: true, showUnsortedBucket: false },
      filters: {
        hasReports: true,
        hasCollections: true,
        hasTerms: true,
        hasInitiatives: false,
        hasUsers: false,
        hasGroups: false,
        hasSearches: false,
      },
      folders: [
        { id: 1, name: "Daily Operations", itemCount: 7 },
        { id: 2, name: "Executive Review", itemCount: 7 },
      ],
      items,
      suggestedReports: [],
    })

    expect(panel.cards).toHaveLength(14)
    expect(panel.folders[0]?.count).toBe(14)
  })

  it("hides type quick filters when the API reports a single favorite type", () => {
    const panel = mapUserStarsPayloadToPanel({
      summary: { totalCount: 1, unsortedCount: 0 },
      filters: {
        hasReports: true,
        hasCollections: false,
        hasInitiatives: false,
        hasTerms: false,
        hasUsers: false,
        hasGroups: false,
        hasSearches: false,
        showQuickFilters: false,
      },
      folders: [],
      items: [
        {
          starId: 1,
          type: "report",
          typeLabel: "Report",
          itemId: 1,
          name: "Executive Dashboard",
          url: "/reports?id=1",
        },
      ],
      suggestedReports: [],
    })

    expect(panel.filters).toEqual([])
  })

  it("shows the unsorted bucket only when the API enables it", () => {
    const panel = mapUserStarsPayloadToPanel({
      summary: {
        totalCount: 3,
        unsortedCount: 1,
        hasFolders: true,
        showUnsortedBucket: true,
      },
      filters: {
        hasReports: true,
        hasCollections: false,
        hasInitiatives: false,
        hasTerms: false,
        hasUsers: false,
        hasGroups: false,
        hasSearches: false,
        showQuickFilters: false,
      },
      folders: [{ id: 5, name: "Pinned", itemCount: 2 }],
      items: [],
      suggestedReports: [],
    })

    expect(panel.folders).toEqual([
      { id: "all", label: "All", count: 3 },
      { id: "unsorted", label: "Unsorted", count: 1 },
      { id: "5", label: "Pinned", count: 2 },
    ])
  })
})
