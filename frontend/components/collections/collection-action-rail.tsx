"use client"

import { Pencil, Plus, Trash2 } from "lucide-react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import type { ReactNode } from "react"
import { useTransition } from "react"
import { deleteCollectionAction } from "@/app/collections/actions"
import { CollectionFeedbackDialog } from "@/components/collections/collection-feedback-dialog"
import { CollectionProfileSheet } from "@/components/collections/collection-profile-sheet"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import { Button } from "@/components/ui/button"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"
import type { CollectionDetailDto } from "@/lib/collections/types"

function RailIconLink({
  href,
  label,
  children,
}: {
  href: string
  label: string
  children: ReactNode
}) {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button asChild variant="ghost" size="icon" className="size-10">
          <Link href={href}>{children}</Link>
        </Button>
      </TooltipTrigger>
      <TooltipContent side="right">{label}</TooltipContent>
    </Tooltip>
  )
}

function RailDeleteButton({
  collectionId,
  collectionName,
}: {
  collectionId: number
  collectionName: string
}) {
  const router = useRouter()
  const [pending, startTransition] = useTransition()

  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="size-10 text-destructive hover:text-destructive"
          disabled={pending}
          onClick={() => {
            const label = collectionName.trim() || `collection ${collectionId}`
            if (!window.confirm(`Delete ${label}? This cannot be undone.`)) return
            startTransition(() => {
              void (async () => {
                const result = await deleteCollectionAction(collectionId)
                if (!result?.error) router.refresh()
              })()
            })
          }}
        >
          <Trash2 className="size-5" />
          <span className="sr-only">Delete collection</span>
        </Button>
      </TooltipTrigger>
      <TooltipContent side="right">Delete this collection</TooltipContent>
    </Tooltip>
  )
}

export function CollectionActionRail({
  collection,
  profilePanel,
}: {
  collection: CollectionDetailDto
  profilePanel: ReactNode
}) {
  const features = collection.features ?? {}
  const shareUrl = `/collections?id=${collection.id}`

  return (
    <TooltipProvider>
      <aside
        aria-label="Collection actions"
        className="sticky top-8 flex w-14 shrink-0 flex-col items-center gap-1 rounded-lg border bg-card p-2"
      >
        <CollectionProfileSheet collectionName={collection.name}>
          {profilePanel}
        </CollectionProfileSheet>

        <StarToggleButton
          type="collection"
          id={collection.id}
          initialStarred={collection.isStarred ?? false}
          initialCount={collection.starCount ?? 0}
          iconOnly
        />

        {features.sharingEnabled !== false ? (
          <ShareMailDialog shareName={collection.name} shareUrl={shareUrl} iconOnly />
        ) : null}

        {features.feedbackEnabled !== false ? (
          <CollectionFeedbackDialog collectionName={collection.name} collectionUrl={shareUrl} />
        ) : null}

        {collection.canCreateCollection ? (
          <RailIconLink href="/collections/new" label="Create new collection">
            <Plus className="size-5" />
            <span className="sr-only">Create new collection</span>
          </RailIconLink>
        ) : null}

        {collection.canEditCollection ? (
          <RailIconLink href={`/collections/edit?id=${collection.id}`} label="Open Atlas editor">
            <Pencil className="size-5" />
            <span className="sr-only">Edit collection</span>
          </RailIconLink>
        ) : null}

        {collection.canDeleteCollection ? (
          <RailDeleteButton collectionId={collection.id} collectionName={collection.name} />
        ) : null}
      </aside>
    </TooltipProvider>
  )
}
