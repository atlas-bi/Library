import type { InitiativeSummaryDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function InitiativeSnippetCard({ initiative }: { initiative: InitiativeSummaryDto }) {
  const id = initiative.id
  const title = initiative.name?.trim() || (id ? `Initiative ${id}` : "Initiative")
  const href = id ? `/initiatives?id=${id}` : undefined
  const excerpt = initiative.description?.trim()
    ? truncateText(initiative.description)
    : "Open to view details."

  return (
    <SnippetMediaCard
      title={title}
      href={href}
      tags={["initiative"]}
      showCertified
      excerpt={
        <>
          {excerpt} <span className="text-link font-medium">read more</span>
        </>
      }
    />
  )
}
