import { BadgeCheck } from "lucide-react"
import Link from "next/link"
import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import { Badge } from "@/components/ui/badge"
import type { TermFeaturesDto, TermListItemDto } from "@/lib/terms/types"
import { truncateText } from "@/lib/text"

export function TermsListCard({
  term,
  features,
  canOpenProfile = true,
}: {
  term: TermListItemDto
  features?: TermFeaturesDto
  canOpenProfile?: boolean
}) {
  const href = term.url ?? `/terms?id=${term.id}`
  const excerpt = term.bodyText?.trim() ? truncateText(term.bodyText) : null

  return (
    <article className="atlas-snippet-gold-card overflow-hidden">
      <header className="flex items-center justify-between gap-3 border-b border-[var(--atlas-home-border-soft)] px-4 py-2.5">
        <Link href={href} className="atlas-home-card-title flex min-w-0 items-center gap-2">
          <span className="truncate">{term.name}</span>
          {term.isApproved ? (
            <span
              className="inline-flex shrink-0 items-center"
              title="Approved term"
            >
              <BadgeCheck className="h-[1.1em] w-[1.1em] fill-[#3e8ed0] text-white" strokeWidth={2} aria-label="Verified" />
            </span>
          ) : null}
        </Link>
        <div className="flex shrink-0 flex-wrap items-center justify-end gap-1.5">
          <span className="atlas-home-type-pill rounded-full px-3 py-1 text-xs normal-case">
            term
          </span>
          {term.isApproved ? (
            <Badge variant="outline" className="border-emerald-200 bg-emerald-50 text-emerald-700">
              Approved
            </Badge>
          ) : null}
        </div>
      </header>

      <div className="grid gap-4 px-4 py-4 md:grid-cols-[128px_1fr]">
        <div className="flex items-start">
          <picture>
            <source srcSet="/img/report_placeholder_128x128.webp" type="image/webp" />
            <img
              src="/img/report_placeholder_128x128.png"
              alt={`${term.name} thumbnail`}
              className="h-32 w-32 rounded-lg border border-[var(--atlas-home-border-soft)] object-cover"
              loading="lazy"
            />
          </picture>
        </div>
        <Link
          href={href}
          className="flex min-h-24 flex-col justify-between gap-3 text-sm font-medium leading-6 text-[var(--atlas-home-text)] hover:text-[var(--atlas-home-text)]"
        >
          <p>
            {excerpt ? (
              <>
                {excerpt} <span className="text-[var(--atlas-home-link)]">read more</span>
              </>
            ) : (
              <span className="text-[var(--atlas-home-link)]">Open to view details.</span>
            )}
          </p>
        </Link>
      </div>

      <EntityCardFooter
        entityType="term"
        id={term.id}
        title={term.name}
        href={href}
        isStarred={term.isStarred}
        starCount={term.starCount}
        features={features}
        canOpenProfile={canOpenProfile}
        profilePanel={
          <ProfileAnalyticsPanel
            id={term.id}
            type="term"
            userProfilesEnabled={features?.userProfilesEnabled}
          />
        }
      />
    </article>
  )
}
