"use client"

import { useMemo, useState } from "react"
import { HomeStarsPanelView } from "@/components/home/home-stars-panel"
import { countCardsForFolder, filterHomeStarCards } from "@/lib/home/stars-filters"
import type { HomeStarsPanel } from "@/lib/home/types"

export function HomeStarsPanelClient({ panel }: { panel: HomeStarsPanel }) {
  const [selectedFolderId, setSelectedFolderId] = useState("all")
  const [selectedTypeFilter, setSelectedTypeFilter] = useState<string | null>(null)
  const [textFilter, setTextFilter] = useState("")

  const filteredCards = useMemo(
    () =>
      filterHomeStarCards(panel.cards, {
        folderId: selectedFolderId,
        typeFilter: selectedTypeFilter,
        textFilter,
      }),
    [panel.cards, selectedFolderId, selectedTypeFilter, textFilter],
  )

  const foldersWithCounts = useMemo(
    () =>
      panel.folders.map((folder) => ({
        ...folder,
        count: countCardsForFolder(panel.cards, folder.id),
      })),
    [panel.cards, panel.folders],
  )

  const displayPanel: HomeStarsPanel = {
    ...panel,
    folders: foldersWithCounts,
    cards: filteredCards,
  }

  return (
    <HomeStarsPanelView
      panel={displayPanel}
      selectedFolderId={selectedFolderId}
      selectedTypeFilter={selectedTypeFilter}
      textFilter={textFilter}
      onFolderChange={setSelectedFolderId}
      onTypeFilterChange={(filterId) => {
        setSelectedTypeFilter((current) => (current === filterId ? null : filterId))
      }}
      onTextFilterChange={setTextFilter}
    />
  )
}
