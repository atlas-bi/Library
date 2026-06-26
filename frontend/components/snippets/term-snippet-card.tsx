import type { CollectionTermDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

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

  return (
    <SnippetMediaCard
      title={title}
      href={href}
      tags={["term"]}
      excerpt={
        <>
          {excerpt} <span className="text-link font-medium">read more</span>
        </>
      }
      footer={
        typeof term.rank === "number" ? (
          <span className="text-xs text-muted-foreground">Rank {term.rank}</span>
        ) : undefined
      }
    />
  )
}
