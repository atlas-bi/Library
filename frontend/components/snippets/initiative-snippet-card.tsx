import Link from "next/link"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader } from "@/components/ui/card"
import type { InitiativeSummaryDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"

export function InitiativeSnippetCard({ initiative }: { initiative: InitiativeSummaryDto }) {
  const id = initiative.id
  const title = initiative.name?.trim() || (id ? `Initiative ${id}` : "Initiative")
  const href = id ? `/initiatives?id=${id}` : undefined
  const excerpt = initiative.description?.trim()
    ? truncateText(initiative.description)
    : "Open to view details."

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
          <Badge variant="secondary">initiative</Badge>
        </div>
      </CardHeader>
      <CardContent className="py-3 text-sm text-muted-foreground">
        {href ? (
          <Link href={href} className="hover:underline">
            {excerpt} <span className="text-link">read more</span>
          </Link>
        ) : (
          <>
            {excerpt} <span className="text-link">read more</span>
          </>
        )}
      </CardContent>
    </Card>
  )
}
