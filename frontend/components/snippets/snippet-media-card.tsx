import { Award, ImageIcon } from "lucide-react"
import Link from "next/link"
import type { ReactNode } from "react"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { cn } from "@/lib/utils"

export function SnippetThumbnail({ alt = "Preview" }: { alt?: string }) {
  return (
    <div
      className="flex size-32 shrink-0 items-center justify-center rounded-md border bg-muted/50"
      aria-hidden
    >
      <ImageIcon className="size-10 text-muted-foreground/60" />
      <span className="sr-only">{alt}</span>
    </div>
  )
}

export function SnippetMediaCard({
  title,
  href,
  tags,
  excerpt,
  footer,
  showCertified = false,
  variant = "default",
}: {
  title: string
  href?: string
  tags: string[]
  excerpt: ReactNode
  footer?: ReactNode
  showCertified?: boolean
  variant?: "default" | "collection"
}) {
  const titleNode = href ? (
    <Link href={href} className="atlas-home-card-title hover:underline">
      {title}
      {showCertified ? (
        <Award className="ml-1.5 inline size-4 text-info" aria-label="Certified" />
      ) : null}
    </Link>
  ) : (
    <span className="atlas-home-card-title">
      {title}
      {showCertified ? (
        <Award className="ml-1.5 inline size-4 text-info" aria-label="Certified" />
      ) : null}
    </span>
  )

  return (
    <Card
      className={cn(
        "h-full overflow-hidden transition-shadow hover:shadow-md",
        variant === "collection" &&
          "border-amber-200/70 shadow-sm ring-1 ring-amber-100/80 dark:border-amber-900/50 dark:ring-amber-950/50",
      )}
    >
      <CardHeader className="gap-2 border-b bg-muted/20 py-3">
        <div className="flex flex-wrap items-start justify-between gap-2">
          <div className="min-w-0 flex-1">{titleNode}</div>
          <div className="flex flex-wrap justify-end gap-1">
            {tags.map((tag) => (
              <Badge key={tag} variant="secondary" className="text-xs uppercase tracking-wide">
                {tag}
              </Badge>
            ))}
          </div>
        </div>
      </CardHeader>
      <CardContent className="py-3">
        <div className="flex gap-4">
          <SnippetThumbnail />
          <div className="min-w-0 flex-1 text-sm leading-relaxed text-muted-foreground">
            {href ? (
              <Link href={href} className="block hover:text-foreground">
                {excerpt}
              </Link>
            ) : (
              excerpt
            )}
          </div>
        </div>
      </CardContent>
      {footer ? <CardFooter className="border-t bg-muted/10 py-2">{footer}</CardFooter> : null}
    </Card>
  )
}
