import {
  BadgeCheck,
  BookOpen,
  ChartBar,
  Folder,
  FolderOpen,
  Lightbulb,
  PlayCircle,
  Search,
  UserRound,
  Users,
  Waypoints,
} from "lucide-react"
import Link from "next/link"
import { HomeStarCardFooter } from "@/components/home/home-star-card-footer"
import type { HomeStarCard, HomeStarsPanel } from "@/lib/home/types"

function filterIcon(label: string) {
  switch (label) {
    case "Reports":
      return <ChartBar className="h-4 w-4" strokeWidth={1.8} />
    case "Collections":
      return <Waypoints className="h-4 w-4" strokeWidth={1.8} />
    case "Initiatives":
      return <Lightbulb className="h-4 w-4" strokeWidth={1.8} />
    case "Terms":
      return <BookOpen className="h-4 w-4" strokeWidth={1.8} />
    case "Users":
      return <UserRound className="h-4 w-4" strokeWidth={1.8} />
    case "Groups":
      return <Users className="h-4 w-4" strokeWidth={1.8} />
    default:
      return <Search className="h-4 w-4" strokeWidth={1.8} />
  }
}

function renderDetailsLink(card: HomeStarCard, className: string, labelClassName: string) {
  if (card.canOpenDetails) {
    return (
      <Link href={card.href} className={className}>
        <span className={labelClassName}>{card.title}</span>
      </Link>
    )
  }

  return <span className={labelClassName}>{card.title}</span>
}

function isCollectionCard(card: HomeStarCard) {
  return card.typeLabel.toLowerCase() === "collection"
}

function isReportCard(card: HomeStarCard) {
  return card.typeLabel.toLowerCase() === "report"
}

export function HomeStarsPanelView({ panel }: { panel: HomeStarsPanel }) {
  return (
    <section className="space-y-5">
      {panel.filters.length > 0 ? (
        <div className="my-4 flex flex-wrap items-center gap-4">
          <div className="inline-flex items-center gap-2 text-sm font-semibold text-[var(--atlas-home-text-strong)]">
            <Search className="h-4 w-4 text-[var(--atlas-home-muted)]" strokeWidth={1.8} />
            <strong>Quick Filter</strong>
          </div>
          <input
            type="text"
            readOnly
            placeholder="type to filter..."
            className="atlas-home-search-shell h-10 min-w-52 bg-white px-3 text-sm shadow-none outline-none"
            aria-label="Filter starred items"
          />
          {panel.filters.map((filter) => (
            <div
              key={filter.id}
              className="atlas-home-filter-button inline-flex items-center gap-2 rounded-full px-4 py-2 text-sm"
            >
              {filterIcon(filter.label)}
              {filter.label}
            </div>
          ))}
        </div>
      ) : null}

      <div className="grid gap-5 lg:grid-cols-[256px_1fr]">
        <aside className="sticky top-20 self-start space-y-3">
          {panel.folders.map((folder, index) => (
            <div
              key={folder.id}
              className={`atlas-home-card relative border border-transparent px-4 py-4 text-[var(--atlas-home-text)] ${index === 0 ? "font-bold" : "font-medium"}`}
            >
              <span className="inline-flex items-center gap-3">
                <span className="relative inline-flex text-[var(--atlas-home-text)]">
                  {index === 0 ? (
                    <FolderOpen className="h-5 w-5" strokeWidth={1.8} />
                  ) : (
                    <Folder className="h-5 w-5" strokeWidth={1.8} />
                  )}
                </span>
                <span>{folder.label}</span>
              </span>
              <span className="atlas-home-folder-badge absolute -top-3 -right-3 rounded-full px-2 py-0.5 text-xs">
                {folder.count}
              </span>
            </div>
          ))}
        </aside>

        <div className="space-y-4">
          {panel.cards.length > 0 ? (
            panel.cards.map((card) => {
              const collectionCard = isCollectionCard(card)
              const reportCard = isReportCard(card)

              return (
                <article
                  key={card.id}
                  className={`overflow-hidden ${collectionCard ? "atlas-snippet-gold-card" : "atlas-home-card"}`}
                >
                  <div className="flex items-center justify-between gap-3 border-b border-[var(--atlas-home-border-soft)] px-4 py-2.5">
                    <div className="flex items-center gap-3 text-[var(--atlas-home-text-strong)]">
                      {reportCard ? (
                        card.canRun && card.runUrl ? (
                          <a
                            href={card.runUrl}
                            target="_blank"
                            rel="noreferrer"
                            aria-label="Run report"
                            className="inline-flex cursor-pointer items-center text-[var(--atlas-home-success)]"
                          >
                            <PlayCircle className="h-8 w-8" strokeWidth={1.5} />
                          </a>
                        ) : (
                          <span
                            title={card.runDisabledReason ?? "Run report unavailable"}
                            className="inline-flex items-center text-[var(--atlas-home-muted)]"
                          >
                            <PlayCircle className="h-8 w-8" strokeWidth={1.5} />
                          </span>
                        )
                      ) : null}
                      <div className="flex flex-wrap items-center gap-2">
                        {renderDetailsLink(
                          card,
                          "atlas-home-card-title hover:underline",
                          "atlas-home-card-title",
                        )}
                        {card.isCertified ? (
                          <span
                            className="inline-flex items-center gap-1 text-[var(--atlas-home-link)]"
                            title="Certified report"
                          >
                            <BadgeCheck className="h-4 w-4 fill-current" strokeWidth={1.8} />
                          </span>
                        ) : null}
                      </div>
                    </div>
                    <div className="flex flex-wrap items-center justify-end gap-2">
                      <div className="atlas-home-type-pill rounded-full px-3 py-1 text-xs">
                        {card.typeLabel}
                      </div>
                      {card.tags
                        ?.filter((tag) => tag.showInHeader)
                        .map((tag) => (
                          <span
                            key={`${card.id}-${tag.name}`}
                            className="atlas-home-type-pill rounded-full px-3 py-1 text-xs"
                          >
                            {tag.name}
                          </span>
                        ))}
                    </div>
                  </div>

                  <div className="grid gap-4 px-4 py-4 md:grid-cols-[128px_1fr]">
                    <div className="flex items-start">
                      {card.thumbnailUrl || card.placeholderImageUrl ? (
                        // biome-ignore lint/performance/noImgElement: homepage parity uses backend-provided report thumbnails directly.
                        <img
                          src={card.thumbnailUrl ?? card.placeholderImageUrl}
                          alt={`${card.title} thumbnail`}
                          className="h-32 w-32 rounded-lg border border-[var(--atlas-home-border-soft)] object-cover"
                        />
                      ) : (
                        <div className="atlas-home-thumbnail h-32 w-32 rounded-lg" />
                      )}
                    </div>
                    <div className="flex min-h-24 flex-col justify-between gap-3 text-sm font-medium leading-6 text-[var(--atlas-home-text)]">
                      <p>{card.description}</p>
                      <div className="text-sm text-[var(--atlas-home-link)]">
                        {card.canOpenDetails ? (
                          <Link href={card.href} className="hover:underline">
                            read more
                          </Link>
                        ) : (
                          "Open to view details."
                        )}
                      </div>
                    </div>
                  </div>

                  <HomeStarCardFooter card={card} />
                </article>
              )
            })
          ) : (
            <div className="atlas-home-card min-h-[72px] px-6 py-6 text-sm text-[var(--atlas-home-text)]">
              {panel.emptyMessage}
            </div>
          )}
        </div>
      </div>
    </section>
  )
}
