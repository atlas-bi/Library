"use client"

import {
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
  updateUserFolderAction,
} from "@/app/users/actions"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
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
    return [...stars.items]
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
  }, [folderFilter, stars.items, textFilter, typeFilter])

  const refreshPage = () => {
    window.location.reload()
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
        <h3 className="text-xl font-semibold">
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
      <p className="text-sm text-muted-foreground">
        You don&apos;t have any favorites! Search to get started.
      </p>
    )
  }

  return (
    <div className={cn("space-y-5", isPending && "opacity-70")}>
      {quickFilters.length > 0 ? (
        <div className="flex flex-wrap items-center gap-4">
          <div className="inline-flex items-center gap-2 text-sm font-semibold">
            <Search className="h-4 w-4 text-muted-foreground" />
            Quick Filter
          </div>
          <Input
            value={textFilter}
            onChange={(event) => setTextFilter(event.target.value)}
            placeholder="type to filter..."
            className="max-w-xs"
            aria-label="Filter starred items"
          />
          {quickFilters.map((filter) => (
            <Button
              key={filter.id}
              type="button"
              size="sm"
              variant={typeFilter === filter.id ? "default" : "outline"}
              onClick={() => setTypeFilter((current) => (current === filter.id ? null : filter.id))}
              className="gap-2"
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
              "relative w-full rounded-md border px-4 py-4 text-left transition-colors",
              folderFilter === "all"
                ? "border-primary bg-primary/5 font-semibold"
                : "hover:bg-muted/50",
            )}
          >
            <span className="inline-flex items-center gap-3">
              <FolderOpen className="h-5 w-5" />
              All
            </span>
            <span className="absolute -top-2 -right-2 rounded-full bg-primary px-2 py-0.5 text-xs text-primary-foreground">
              {stars.summary.totalCount}
            </span>
          </button>

          {stars.summary.showUnsortedBucket ? (
            <button
              type="button"
              onClick={() => setFolderFilter("unsorted")}
              className={cn(
                "relative w-full rounded-md border px-4 py-4 text-left transition-colors",
                folderFilter === "unsorted"
                  ? "border-primary bg-primary/5 font-semibold"
                  : "hover:bg-muted/50",
              )}
            >
              <span className="inline-flex items-center gap-3">
                <Folder className="h-5 w-5" />
                Unsorted
              </span>
              <span className="absolute -top-2 -right-2 rounded-full bg-primary px-2 py-0.5 text-xs text-primary-foreground">
                {stars.summary.unsortedCount}
              </span>
            </button>
          ) : null}

          {stars.folders.map((folder) => (
            <div key={folder.id} className="space-y-2">
              <button
                type="button"
                onClick={() => setFolderFilter(folder.id)}
                className={cn(
                  "relative w-full rounded-md border px-4 py-4 text-left transition-colors",
                  folderFilter === folder.id
                    ? "border-primary bg-primary/5 font-semibold"
                    : "hover:bg-muted/50",
                )}
              >
                <span className="inline-flex items-center gap-3">
                  <Folder className="h-5 w-5" />
                  {folder.name?.trim() || `Folder ${folder.id}`}
                </span>
                <span className="absolute -top-2 -right-2 rounded-full bg-primary px-2 py-0.5 text-xs text-primary-foreground">
                  {folder.itemCount}
                </span>
              </button>
              {stars.canEditWorkspace && folder.canManage ? (
                <div className="flex gap-2 px-1">
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
                </div>
              ) : null}
            </div>
          ))}

          {stars.canEditWorkspace && stars.permissions.canCreateFolders ? (
            <div className="space-y-2 rounded-md border p-3">
              <div className="text-sm font-medium">New folder</div>
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
            <div className="space-y-2 rounded-md border p-3">
              <div className="text-sm font-medium">Rename folder</div>
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
            filteredItems.map((item) => (
              <UserFavoriteCard key={`${item.type}-${item.starId}`} item={item} />
            ))
          ) : (
            <p className="text-sm text-muted-foreground">No favorites match the current filters.</p>
          )}
        </div>
      </div>
    </div>
  )
}
