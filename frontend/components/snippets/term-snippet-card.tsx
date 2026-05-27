import Link from "next/link"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import type { CollectionTermDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"

function termIdFromDto(term: CollectionTermDto): number | null {
  if (typeof term.id === "number") return term.id
  if (typeof term.termId === "number") return term.termId
  return null
}

export function TermSnippetCard({ term }: { term: CollectionTermDto }) {
  const termId = termIdFromDto(term)
  const title = term.name?.trim() || (termId ? `Term ${termId}` : "Term")
  const href = termId ? `/terms?id=${termId}` : undefined
  const excerpt = term.summary?.trim() ? truncateText(term.summary) : "Open to view details."

  const body = (
    <>
      {excerpt} <span className="text-link">read more</span>
    </>
  )

  return (
    <Card className="h-full">
      <CardHeader className="gap-2 border-b py-3">
        <div className="flex flex-wrap items-start justify-between gap-2">
          {href ? (
            <Link href={href} className="font-medium hover:underline">
              {title}
            </Link>
          ) : (
            <span className="font-medium">{title}</span>
          )}
          <Badge variant="secondary">term</Badge>
        </div>
      </CardHeader>
      <CardContent className="py-3 text-sm text-muted-foreground">
        {href ? (
          <Link href={href} className="hover:underline">
            {body}
          </Link>
        ) : (
          body
        )}
      </CardContent>
      <CardFooter className="border-t py-2 text-xs text-muted-foreground">
        {typeof term.rank === "number" ? `Rank ${term.rank}` : null}
      </CardFooter>
    </Card>
  )
}
