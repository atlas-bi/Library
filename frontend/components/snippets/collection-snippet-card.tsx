import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { CollectionListItemDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function CollectionSnippetCard({ collection }: { collection: CollectionListItemDto }) {
  const href = `/collections?id=${collection.id}`
  const excerpt = collection.description?.trim()
    ? truncateText(collection.description)
    : "Open to view details."

  return (
    <SnippetMediaCard
      variant="collection"
      title={collection.name}
      href={href}
      tags={["collection"]}
      showCertified
      excerpt={
        <>
          {excerpt} <span className="text-link font-medium">read more</span>
        </>
      }
      footer={
        <EntityCardFooter
          entityType="collection"
          id={collection.id}
          title={collection.name}
          href={href}
          isStarred={collection.isStarred}
          starCount={collection.starCount}
          profilePanel={<ProfileAnalyticsPanel id={collection.id} type="collection" />}
        />
      }
    />
  )
}
