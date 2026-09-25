"use client"

import {
  ArrowDown,
  ArrowUp,
  BookOpen,
  ChartBar,
  Folder,
  FolderOpen,
  Lightbulb,
  Plus,
  Search,
  Trash2,
  UserRound,
  Users,
  Waypoints,
} from "lucide-react"
import { useMemo, useState, useTransition } from "react"
import {
  createUserFolderAction,
  deleteUserFolderAction,
  reorderUserFavoritesAction,
  reorderUserFoldersAction,
  updateUserFavoriteFolderAssignmentAction,
  updateUserFolderAction,
} from "@/app/users/actions"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { UserFavoriteCard } from "@/components/users/user-favorite-card"
import type { UserStars } from "@/lib/users/types"
import { cn } from "@/lib/utils"

type FolderFilter = "all" | "unsorted" | number

function filterIcon(type: string) {
  switch (type) {
    case "report":
      return <ChartBar className="h-4 w-4" />
    case "collection":
      return <Waypoints className="h-4 w-4" />
    case "initiative":
      return <Lightbulb className="h-4 w-4" />
    case "term":
      return <BookOpen className="h-4 w-4" />
    case "user":
      return <UserRound className="h-4 w-4" />
    case "group":
      return <Users className="h-4 w-4" />
    default:
      return <Search className="h-4 w-4" />
  }
}

export function UserStarsWorkspace({ stars }: { stars: UserStars }) {
  const [folders, setFolders] = useState(() =>
    [...stars.folders].sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0)),
  )
  const [items, setItems] = useState(() =>
    [...stars.items].sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0)),
  )
  const [folderFilter, setFolderFilter] = useState<FolderFilter>("all")
  const [typeFilter, setTypeFilter] = useState<string | null>(null)
  const [textFilter, setTextFilter] = useState("")
  const [newFolderName, setNewFolderName] = useState("")
  const [renameFolderId, setRenameFolderId] = useState<number | null>(null)
  const [renameFolderName, setRenameFolderName] = useState("")
  const [isPending, startTransition] = useTransition()

  const quickFilters = useMemo(() => {
    const filters: Array<{ id: string; label: string }> = []
    const state = stars.filters
    if (!state.showQuickFilters) return filters
    if (state.hasReports) filters.push({ id: "report", label: "Reports" })
    if (state.hasCollections) filters.push({ id: "collection", label: "Collections" })
    if (state.hasInitiatives) filters.push({ id: "initiative", label: "Initiatives" })
    if (state.hasTerms) filters.push({ id: "term", label: "Terms" })
    if (state.hasUsers) filters.push({ id: "user", label: "Users" })
    if (state.hasGroups) filters.push({ id: "group", label: "Groups" })
    if (state.hasSearches) filters.push({ id: "search", label: "Searches" })
    return filters
  }, [stars.filters])

  const filteredItems = useMemo(() => {
    const query = textFilter.trim().toLowerCase()
    return [...items]
      .filter((item) => {
        if (folderFilter === "unsorted") return item.folderId == null
        if (typeof folderFilter === "number") return item.folderId === folderFilter
        return true
      })
      .filter((item) => {
        if (!typeFilter) return true
        return item.type?.toLowerCase() === typeFilter
      })
      .filter((item) => {
        if (!query) return true
        const haystack = [
          item.name,
          item.description,
          item.searchString,
          item.typeLabel,
          item.secondaryText,
        ]
          .filter(Boolean)
          .join(" ")
          .toLowerCase()
        return haystack.includes(query)
      })
      .sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0))
  }, [folderFilter, items, textFilter, typeFilter])

  const refreshPage = () => {
    window.location.reload()
  }

  const persistFolderOrder = (nextFolders: typeof folders) => {
    startTransition(async () => {
      const payload = nextFolders.map((folder, index) => ({
        folderId: String(folder.id),
        folderRank: index + 1,
      }))
      const result = await reorderUserFoldersAction(stars.userId, stars.isCurrentUser, payload)
      if (result.ok) refreshPage()
    })
  }

  const persistFavoriteOrder = (nextItems: typeof items) => {
    startTransition(async () => {
      const payload = nextItems.map((item, index) => ({
        favoriteId: String(item.starId),
        favoriteType: item.type ?? "",
        favoriteRank: index + 1,
      }))
      const result = await reorderUserFavoritesAction(stars.userId, stars.isCurrentUser, payload)
      if (result.ok) refreshPage()
    })
  }

  const moveFolder = (folderId: number, direction: -1 | 1) => {
    const index = folders.findIndex((folder) => folder.id === folderId)
    const targetIndex = index + direction
    if (index < 0 || targetIndex < 0 || targetIndex >= folders.length) return
    const nextFolders = [...folders]
    const [folder] = nextFolders.splice(index, 1)
    nextFolders.splice(targetIndex, 0, folder)
    setFolders(nextFolders)
    persistFolderOrder(nextFolders)
  }

  const moveFavorite = (favoriteId: number, direction: -1 | 1) => {
    const scopedItems = filteredItems
    const index = scopedItems.findIndex((item) => item.starId === favoriteId)
    const targetIndex = index + direction
    if (index < 0 || targetIndex < 0 || targetIndex >= scopedItems.length) return

    const current = scopedItems[index]
    const target = scopedItems[targetIndex]
    const sourceIndex = items.findIndex((item) => item.starId === current.starId)
    const destinationIndex = items.findIndex((item) => item.starId === target.starId)
    if (sourceIndex < 0 || destinationIndex < 0) return

    const nextItems = [...items]
    const [moved] = nextItems.splice(sourceIndex, 1)
    nextItems.splice(destinationIndex, 0, moved)
    setItems(nextItems)
    persistFavoriteOrder(nextItems)
  }

  const assignFavoriteFolder = (favoriteId: number, favoriteType: string, folderId: string) => {
    startTransition(async () => {
      const result = await updateUserFavoriteFolderAssignmentAction(
        stars.userId,
        stars.isCurrentUser,
        {
          favoriteId,
          favoriteType,
          folderId: folderId === "unsorted" ? null : Number(folderId),
        },
      )
      if (result.ok) refreshPage()
    })
  }

  const handleCreateFolder = () => {
    const name = newFolderName.trim()
    if (!name) return
    startTransition(async () => {
      const result = await createUserFolderAction(stars.userId, stars.isCurrentUser, { name })
      if (result.ok) refreshPage()
    })
  }

  const handleRenameFolder = () => {
    if (renameFolderId == null) return
    const name = renameFolderName.trim()
    if (!name) return
    startTransition(async () => {
      const result = await updateUserFolderAction(
        stars.userId,
        stars.isCurrentUser,
        renameFolderId,
        {
          name,
        },
      )
      if (result.ok) refreshPage()
    })
  }

  const handleDeleteFolder = (folderId: number) => {
    startTransition(async () => {
      const result = await deleteUserFolderAction(stars.userId, stars.isCurrentUser, folderId)
      if (result.ok) refreshPage()
    })
  }

  if (stars.items.length === 0 && stars.suggestedReports.length > 0) {
    return (
      <div className="space-y-4">
        <h3 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">
          You don&apos;t have any favorites! Here&apos;s some reports you&apos;ve used.
        </h3>
        <div className="grid gap-4 md:grid-cols-2">
          {stars.suggestedReports.map((report) => (
            <UserFavoriteCard
              key={report.id}
              item={{
                starId: report.id,
                type: "report",
                typeLabel: report.type ?? "Report",
                itemId: report.id,
                name: report.name,
                description: report.description,
                url: report.url,
                canReorder: false,
                isStarred: false,
                starCount: 0,
                isCertified: false,
                isApproved: false,
                canOpenProfile: true,
                canShare: false,
                canRequestAccess: false,
                canRun: false,
                opensRunModal: false,
                canEditInEditor: false,
                canManageInEditor: false,
                tags: [],
                relatedCollectionNames: [],
              }}
            />
          ))}
        </div>
      </div>
    )
  }

  if (stars.items.length === 0) {
    return (
      <div className="atlas-home-card min-h-[72px] px-6 py-6 text-sm text-[var(--atlas-home-text)]">
        You don&apos;t have any favorites! Search to get started.
      </div>
    )
  }

  return (
    <div className={cn("space-y-5", isPending && "opacity-70")}>
      {quickFilters.length > 0 ? (
        <div className="my-4 flex flex-wrap items-center gap-4">
          <div className="inline-flex items-center gap-2 text-sm font-semibold text-[var(--atlas-home-text-strong)]">
            <Search className="h-4 w-4 text-[var(--atlas-home-muted)]" strokeWidth={1.8} />
            Quick Filter
          </div>
          <Input
            value={textFilter}
            onChange={(event) => setTextFilter(event.target.value)}
            placeholder="type to filter..."
            className="atlas-home-search-shell h-10 max-w-xs bg-white px-3 text-sm shadow-none outline-none"
            aria-label="Filter starred items"
          />
          {quickFilters.map((filter) => (
            <Button
              key={filter.id}
              type="button"
              size="sm"
              variant={typeFilter === filter.id ? "default" : "outline"}
              onClick={() => setTypeFilter((current) => (current === filter.id ? null : filter.id))}
              className="atlas-home-filter-button gap-2 rounded-full px-4 py-2 text-sm"
            >
              {filterIcon(filter.id)}
              {filter.label}
            </Button>
          ))}
        </div>
      ) : null}

      <div className="grid gap-5 lg:grid-cols-[256px_1fr]">
        <aside className="sticky top-20 space-y-3 self-start">
          <button
            type="button"
            onClick={() => setFolderFilter("all")}
            className={cn(
              "atlas-home-card relative w-full border border-transparent px-4 py-4 text-left transition-colors",
              folderFilter === "all"
                ? "font-bold"
                : "font-medium hover:bg-[var(--atlas-home-surface-muted)]",
            )}
          >
            <span className="inline-flex items-center gap-3 text-[var(--atlas-home-text)]">
              <FolderOpen className="h-5 w-5" strokeWidth={1.8} />
              All
            </span>
            <span className="atlas-home-folder-badge absolute -top-3 -right-3 rounded-full px-2 py-0.5 text-xs">
              {stars.summary.totalCount}
            </span>
          </button>

          {stars.summary.showUnsortedBucket ? (
            <button
              type="button"
              onClick={() => setFolderFilter("unsorted")}
              className={cn(
                "atlas-home-card relative w-full border border-transparent px-4 py-4 text-left transition-colors",
                folderFilter === "unsorted"
                  ? "font-bold"
                  : "font-medium hover:bg-[var(--atlas-home-surface-muted)]",
              )}
            >
              <span className="inline-flex items-center gap-3 text-[var(--atlas-home-text)]">
                <Folder className="h-5 w-5" strokeWidth={1.8} />
                Unsorted
              </span>
              <span className="atlas-home-folder-badge absolute -top-3 -right-3 rounded-full px-2 py-0.5 text-xs">
                {stars.summary.unsortedCount}
              </span>
            </button>
          ) : null}

          {folders.map((folder, index) => (
            <div key={folder.id} className="space-y-2">
              <button
                type="button"
                onClick={() => setFolderFilter(folder.id)}
                className={cn(
                  "atlas-home-card relative w-full border border-transparent px-4 py-4 text-left transition-colors",
                  folderFilter === folder.id
                    ? "font-bold"
                    : "font-medium hover:bg-[var(--atlas-home-surface-muted)]",
                )}
              >
                <span className="inline-flex items-center gap-3 text-[var(--atlas-home-text)]">
                  <Folder className="h-5 w-5" strokeWidth={1.8} />
                  {folder.name?.trim() || `Folder ${folder.id}`}
                </span>
                <span className="atlas-home-folder-badge absolute -top-3 -right-3 rounded-full px-2 py-0.5 text-xs">
                  {folder.itemCount}
                </span>
              </button>
              {stars.canEditWorkspace && folder.canManage ? (
                <div className="flex flex-wrap gap-2 px-1">
                  <Button
                    type="button"
                    size="sm"
                    variant="outline"
                    onClick={() => {
                      setRenameFolderId(folder.id)
                      setRenameFolderName(folder.name?.trim() ?? "")
                    }}
                  >
                    Rename
                  </Button>
                  <Button
                    type="button"
                    size="sm"
                    variant="outline"
                    onClick={() => handleDeleteFolder(folder.id)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                  <Button
                    type="button"
                    size="sm"
                    variant="outline"
                    disabled={index === 0}
                    onClick={() => moveFolder(folder.id, -1)}
                  >
                    <ArrowUp className="h-4 w-4" />
                  </Button>
                  <Button
                    type="button"
                    size="sm"
                    variant="outline"
                    disabled={index === folders.length - 1}
                    onClick={() => moveFolder(folder.id, 1)}
                  >
                    <ArrowDown className="h-4 w-4" />
                  </Button>
                </div>
              ) : null}
            </div>
          ))}

          {stars.canEditWorkspace && stars.permissions.canCreateFolders ? (
            <div className="atlas-home-card space-y-2 p-3">
              <div className="text-sm font-medium text-[var(--atlas-home-text-strong)]">
                New folder
              </div>
              <Input
                value={newFolderName}
                onChange={(event) => setNewFolderName(event.target.value)}
                placeholder="Folder name"
              />
              <Button type="button" size="sm" className="gap-2" onClick={handleCreateFolder}>
                <Plus className="h-4 w-4" />
                Save
              </Button>
            </div>
          ) : null}

          {renameFolderId != null ? (
            <div className="atlas-home-card space-y-2 p-3">
              <div className="text-sm font-medium text-[var(--atlas-home-text-strong)]">
                Rename folder
              </div>
              <Input
                value={renameFolderName}
                onChange={(event) => setRenameFolderName(event.target.value)}
              />
              <Button type="button" size="sm" onClick={handleRenameFolder}>
                Save
              </Button>
            </div>
          ) : null}
        </aside>

        <div className="space-y-4">
          {filteredItems.length > 0 ? (
            filteredItems.map((item, index) => (
              <div key={`${item.type}-${item.starId}`} className="space-y-2">
                {(stars.canEditWorkspace || item.canReorder) && stars.isCurrentUser ? (
                  <div className="flex flex-wrap items-center justify-end gap-2">
                    {stars.permissions.canMoveFavoritesToFolders ? (
                      <Select
                        defaultValue={item.folderId == null ? "unsorted" : String(item.folderId)}
                        onValueChange={(value) =>
                          assignFavoriteFolder(item.starId, item.type ?? "", value)
                        }
                      >
                        <SelectTrigger className="h-9 w-[180px]">
                          <SelectValue placeholder="Move to folder" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="unsorted">Unsorted</SelectItem>
                          {folders.map((folder) => (
                            <SelectItem key={folder.id} value={String(folder.id)}>
                              {folder.name?.trim() || `Folder ${folder.id}`}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    ) : null}
                    {stars.permissions.canReorderFavorites ? (
                      <>
                        <Button
                          type="button"
                          size="sm"
                          variant="outline"
                          disabled={index === 0}
                          onClick={() => moveFavorite(item.starId, -1)}
                        >
                          <ArrowUp className="h-4 w-4" />
                        </Button>
                        <Button
                          type="button"
                          size="sm"
                          variant="outline"
                          disabled={index === filteredItems.length - 1}
                          onClick={() => moveFavorite(item.starId, 1)}
                        >
                          <ArrowDown className="h-4 w-4" />
                        </Button>
                      </>
                    ) : null}
                  </div>
                ) : null}
                <UserFavoriteCard item={item} />
              </div>
            ))
          ) : (
            <div className="atlas-home-card px-6 py-7 text-sm text-[var(--atlas-home-text)]">
              No favorites match the current filters.
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
