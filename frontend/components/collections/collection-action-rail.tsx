"use client"

import { Pencil, Plus, Trash2 } from "lucide-react"
import { useRouter } from "next/navigation"
import type { ReactNode } from "react"
import { useTransition } from "react"
import { deleteCollectionAction } from "@/app/collections/actions"
import {
  ActionRail,
  ActionRailGroup,
  RailIconLink,
  RailTooltipButton,
} from "@/components/interactions/action-rail"
import { EntityEngagementRailActions } from "@/components/interactions/entity-engagement-rail-actions"
import type { CollectionDetailDto } from "@/lib/collections/types"

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
      className="atlas-action-rail-button text-destructive hover:text-destructive"
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
  const hasAdminActions =
    collection.canCreateCollection || collection.canEditCollection || collection.canDeleteCollection

  return (
    <ActionRail label="Collection actions">
      <ActionRailGroup>
        <EntityEngagementRailActions
          entityType="collection"
          entityId={collection.id}
          entityName={collection.name}
          entityUrl={shareUrl}
          profileLabel="collection profile"
          profilePanel={profilePanel}
          isStarred={collection.isStarred}
          starCount={collection.starCount}
          features={features}
        />
      </ActionRailGroup>

      {hasAdminActions ? (
        <ActionRailGroup separated>
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
        </ActionRailGroup>
      ) : null}
    </ActionRail>
  )
}
