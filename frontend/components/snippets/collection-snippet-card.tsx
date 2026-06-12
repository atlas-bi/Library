import type { CollectionListItemDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function CollectionSnippetCard({ collection }: { collection: CollectionListItemDto }) {
  const excerpt = collection.description?.trim()
    ? truncateText(collection.description)
    : "Open to view details."

  return (
    <SnippetMediaCard
      variant="collection"
      title={collection.name}
      href={`/collections?id=${collection.id}`}
      tags={["collection"]}
      showCertified
      excerpt={
        <>
          {excerpt} <span className="text-link font-medium">read more</span>
        </>
      }
      footer={
        <div className="flex w-full items-center justify-between text-xs text-muted-foreground">
          {typeof collection.starCount === "number" ? (
            <span>{collection.starCount} stars</span>
          ) : (
            <span />
          )}
          {collection.isStarred ? (
            <span className="font-medium text-amber-600">Starred</span>
          ) : null}
        </div>
      }
    />
  )
}
