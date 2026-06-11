"use client"

import { Pencil, Plus, Trash2 } from "lucide-react"
import { useRouter } from "next/navigation"
import type { ReactNode } from "react"
import { useTransition } from "react"
import { deleteCollectionAction } from "@/app/collections/actions"
import { ActionRail, RailIconLink, RailTooltipButton } from "@/components/interactions/action-rail"
import { EntityFeedbackDialog } from "@/components/interactions/entity-feedback-dialog"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import type { CollectionDetailDto } from "@/lib/collections/types"
import { isInteractionFeatureEnabled } from "@/lib/interactions/features"

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
    <RailTooltipButton
      label="Delete this collection"
      disabled={pending}
      className="size-10 text-destructive hover:text-destructive"
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
    </RailTooltipButton>
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
    <ActionRail label="Collection actions">
      <EntityProfileSheet entityName={collection.name} entityLabel="collection profile">
        {profilePanel}
      </EntityProfileSheet>

      <StarToggleButton
        type="collection"
        id={collection.id}
        initialStarred={collection.isStarred ?? false}
        initialCount={collection.starCount ?? 0}
        iconOnly
      />

      {isInteractionFeatureEnabled(features.sharingEnabled) ? (
        <ShareMailDialog shareName={collection.name} shareUrl={shareUrl} iconOnly />
      ) : null}

      {isInteractionFeatureEnabled(features.feedbackEnabled) ? (
        <EntityFeedbackDialog entityName={collection.name} entityUrl={shareUrl} />
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
    </ActionRail>
  )
}
