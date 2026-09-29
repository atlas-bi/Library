import type { HomeStarCard } from "@/lib/home/types"

export function filterHomeStarCards(
  cards: HomeStarCard[],
  options: {
    folderId: string
    typeFilter: string | null
    textFilter: string
  },
): HomeStarCard[] {
  const query = options.textFilter.trim().toLowerCase()

  return cards.filter((card) => {
    if (options.typeFilter && card.itemType !== options.typeFilter) {
      return false
    }

    if (options.folderId === "unsorted") {
      if (card.folderId != null) return false
    } else if (options.folderId !== "all") {
      const selectedFolderId = Number(options.folderId)
      if (!Number.isFinite(selectedFolderId) || card.folderId !== selectedFolderId) {
        return false
      }
    }

    if (!query) return true

    return (
      card.title.toLowerCase().includes(query) ||
      card.typeLabel.toLowerCase().includes(query) ||
      card.description.toLowerCase().includes(query)
    )
  })
}

export function countCardsForFolder(cards: HomeStarCard[], folderId: string): number {
  return filterHomeStarCards(cards, { folderId, typeFilter: null, textFilter: "" }).length
}
