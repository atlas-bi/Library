import Link from "next/link"
import { Button } from "@/components/ui/button"

export type ScopedSiteAnalyticsCtaProps = {
  scope: "user" | "group"
  id: number
  className?: string
}

export function buildScopedAnalyticsHref(scope: "user" | "group", id: number): string {
  const param = scope === "user" ? "userId" : "groupId"
  return `/analytics?${param}=${id}`
}

export function ScopedSiteAnalyticsCta({ scope, id, className }: ScopedSiteAnalyticsCtaProps) {
  const href = buildScopedAnalyticsHref(scope, id)
  const subject = scope === "user" ? "this user" : "this group"

  return (
    <div className={className ?? "space-y-4"}>
      <p className="text-sm text-[var(--atlas-home-muted)]">
        Site usage for {subject} is available on the Analytics dashboard (scoped to their activity).
      </p>
      <Button asChild variant="outline">
        <Link href={href}>Open scoped analytics</Link>
      </Button>
    </div>
  )
}