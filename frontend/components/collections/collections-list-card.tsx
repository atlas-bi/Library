import { BadgeCheck } from "lucide-react"
import Link from "next/link"
import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { CollectionListItemDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"

export function CollectionsListCard({ collection }: { collection: CollectionListItemDto }) {
  const href = `/collections?id=${collection.id}`
  const excerpt = collection.description?.trim() ? truncateText(collection.description) : null

  return (
    <article className="atlas-snippet-gold-card overflow-hidden">
      <header className="flex items-center justify-between gap-3 border-b border-[var(--atlas-home-border-soft)] px-4 py-2.5">
        <Link href={href} className="atlas-home-card-title flex min-w-0 items-center gap-2">
          <span className="truncate">{collection.name}</span>
          <span
            className="inline-flex shrink-0 items-center text-[var(--atlas-home-link)]"
            title="Certified collection"
          >
            <BadgeCheck className="h-4 w-4 fill-current" strokeWidth={1.8} />
          </span>
        </Link>
        <span className="atlas-home-type-pill rounded-full px-3 py-1 text-xs normal-case">
          collection
        </span>
      </header>

      <div className="grid gap-4 px-4 py-4 md:grid-cols-[128px_1fr]">
        <div className="flex items-start">
          {/* biome-ignore lint/performance/noImgElement: proxied backend placeholder matches C# _Snippet.cshtml */}
          <img
            src="/img/report_placeholder_128x128.png"
            alt={`${collection.name} thumbnail`}
            className="h-32 w-32 rounded-lg border border-[var(--atlas-home-border-soft)] object-cover"
            loading="lazy"
          />
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
        entityType="collection"
        id={collection.id}
        title={collection.name}
        href={href}
        isStarred={collection.isStarred}
        starCount={collection.starCount}
        profilePanel={<ProfileAnalyticsPanel id={collection.id} type="collection" />}
      />
    </article>
  )
}
