"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { searchInitiativeCollectionsAction } from "@/lib/initiatives/actions"
import type { InitiativeCollectionTypeaheadItemDto } from "@/lib/initiatives/types"

export function LinkedCollectionPicker({
  onSelectAction,
}: {
  onSelectAction: (item: InitiativeCollectionTypeaheadItemDto) => void
}) {
  const [query, setQuery] = useState("")
  const [results, setResults] = useState<InitiativeCollectionTypeaheadItemDto[]>([])
  const [isSearching, setIsSearching] = useState(false)

  const handleSearch = async () => {
    setIsSearching(true)
    const result = await searchInitiativeCollectionsAction(query)
    setIsSearching(false)

    if (result.data) {
      setResults(result.data)
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-2">
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search collections..."
          className="flex-1 rounded-md border bg-background px-3 py-2 text-sm"
          onKeyDown={(e) => {
            if (e.key === "Enter") handleSearch()
          }}
        />
        <Button onClick={handleSearch} disabled={isSearching} type="button">
          Search
        </Button>
      </div>
      {results.length > 0 && (
        <ul className="space-y-2 rounded-md border p-2">
          {results.map((item) => (
            <li key={item.id} className="flex items-center justify-between text-sm">
              <div>
                <div className="font-medium">{item.name}</div>
                {item.description && <div className="text-xs text-muted-foreground">{item.description}</div>}
              </div>
              <Button size="sm" variant="outline" onClick={() => onSelectAction(item)} type="button">
                Select
              </Button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
