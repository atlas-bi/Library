import { BadgeCheck, BarChart2, Share, Star } from "lucide-react"
import Link from "next/link"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { TermFeaturesDto, TermListItemDto } from "@/lib/terms/types"
import { truncateText } from "@/lib/text"
import { isInteractionFeatureEnabled } from "@/lib/interactions/features"

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
  const sharingEnabled = isInteractionFeatureEnabled(features?.sharingEnabled)

  return (
    <div className="atlas-home-card flex flex-col overflow-hidden border border-[var(--atlas-home-border-soft)]">
      <div className="flex items-center justify-between px-4 py-2 border-b border-[var(--atlas-home-border-soft)]">
        <Link href={href} className="atlas-home-card-title flex min-w-0 items-center gap-2">
          <span className="truncate">{term.name}</span>
          {term.isApproved ? (
            <span className="inline-flex shrink-0 items-center text-[#3e8ed0]">
              <BadgeCheck className="size-[1.1em] fill-[#3e8ed0] text-white" strokeWidth={2} />
            </span>
          ) : null}
        </Link>
        <div className="flex shrink-0 flex-wrap items-center gap-2 text-sm">
          <div className="flex items-center">
            <span className="bg-[#f5f5f5] text-[#4a4a4a] px-3 py-[0.15rem] text-[0.75rem] leading-tight rounded">
              term
            </span>
          </div>
          {term.isApproved ? (
            <div className="flex items-center">
              <span className="bg-[#48c78e] text-white px-3 py-[0.15rem] text-[0.75rem] leading-tight rounded">
                Approved
              </span>
            </div>
          ) : null}
        </div>
      </div>

      <div className="p-4 flex-1">
        <div className="flex flex-row items-stretch gap-4">
          <div className="shrink-0 flex items-start">
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
          <div className="flex flex-col flex-1 min-w-0 min-h-[96px]">
            <Link
              href={href}
          className="text-sm font-medium leading-6 text-[var(--atlas-home-text)] hover:text-[var(--atlas-home-text)]"
            >
              <p>
                {excerpt ? (
                  <>
                {excerpt} <span className="text-[var(--atlas-home-link)] hover:underline">read more</span>
                  </>
                ) : (
              <span className="text-[var(--atlas-home-link)] hover:underline">Open to view details.</span>
                )}
              </p>
            </Link>
          </div>
        </div>
      </div>

      <footer className="grid grid-cols-2 border-t border-[var(--atlas-home-border-soft)] bg-[#fafafa]">
        <div className="flex items-center justify-center border-r border-[var(--atlas-home-border-soft)] p-0">
          <StarToggleButton
            type="term"
            id={term.id}
            initialStarred={term.isStarred ?? false}
            initialCount={term.starCount ?? 0}
            variant="card-footer"
          />
        </div>
        <div className="flex items-center justify-center text-[var(--atlas-home-muted)] divide-x divide-[var(--atlas-home-border-soft)]">
          {canOpenProfile && (
            <EntityProfileSheet entityName={term.name} entityLabel="term profile" variant="footer">
              <ProfileAnalyticsPanel
                id={term.id}
                type="term"
                userProfilesEnabled={features?.userProfilesEnabled}
              />
            </EntityProfileSheet>
          )}
          {sharingEnabled && (
            <ShareMailDialog shareName={term.name} shareUrl={href} iconOnly variant="footer" />
          )}
        </div>
      </footer>
    </div>
  )
}

