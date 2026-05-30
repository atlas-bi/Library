import Link from "next/link"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import type { CollectionListItemDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"

export function CollectionSnippetCard({ collection }: { collection: CollectionListItemDto }) {
  const excerpt = collection.description?.trim()
    ? truncateText(collection.description)
    : "Open to view details."

  return (
    <Card className="h-full border-amber-200/60 shadow-sm dark:border-amber-900/40">
      <CardHeader className="gap-2 border-b py-3">
        <div className="flex flex-wrap items-start justify-between gap-2">
          <Link href={`/collections?id=${collection.id}`} className="font-medium hover:underline">
            {collection.name}
          </Link>
          <Badge variant="secondary">collection</Badge>
        </div>
      </CardHeader>
      <CardContent className="py-3">
        <Link
          href={`/collections?id=${collection.id}`}
          className="text-sm text-muted-foreground hover:underline"
        >
          {excerpt} <span className="text-link">read more</span>
        </Link>
      </CardContent>
      <CardFooter className="border-t py-2 text-xs text-muted-foreground">
        {typeof collection.starCount === "number" ? (
          <span>{collection.starCount} stars</span>
        ) : null}
        {collection.isStarred ? <span className="ml-auto font-medium">Starred</span> : null}
      </CardFooter>
    </Card>
  )
}
