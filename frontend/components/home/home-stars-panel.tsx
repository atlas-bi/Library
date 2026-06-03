import {
  BadgeCheck,
  BarChart3,
  BookOpen,
  ChartBar,
  Folder,
  FolderOpen,
  GlobeLock,
  Lightbulb,
  Pencil,
  PlayCircle,
  Search,
  Settings,
  Share2,
  Star,
  UserRound,
  Users,
  Waypoints,
} from "lucide-react"
import Link from "next/link"
import type { ReactNode } from "react"
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

function renderFooterLink(href: string, icon: ReactNode, label: string) {
  return (
    <a
      href={href}
      target="_blank"
      rel="noreferrer"
      className="atlas-home-footer-cell inline-flex cursor-pointer items-center justify-center gap-2 border-r border-[var(--atlas-home-border-soft)] text-center text-sm hover:text-[var(--atlas-home-link)]"
    >
      {icon}
      <span>{label}</span>
    </a>
  )
}

export function HomeStarsPanelView({ panel }: { panel: HomeStarsPanel }) {
  return (
    <section className="space-y-5">
      {panel.filters.length > 0 ? (
        <div className="my-4 flex flex-wrap items-center gap-4">
          <div className="inline-flex items-center gap-2 text-sm font-semibold text-[var(--atlas-home-title)]">
            <Search className="h-4 w-4 text-[var(--atlas-home-muted)]" strokeWidth={1.8} />
            Quick Filter
          </div>
          <div className="flex h-10 min-w-52 items-center rounded-md border border-[var(--atlas-home-border)] bg-white px-3 text-sm text-[var(--atlas-home-muted)]">
            type to filter...
          </div>
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
            panel.cards.map((card) => (
              <article key={card.id} className="atlas-home-card overflow-hidden">
                <div className="flex items-center justify-between gap-3 border-b border-[var(--atlas-home-border-soft)] px-4 py-2.5">
                  <div className="flex items-center gap-3 text-[var(--atlas-home-title)]">
                    {card.canRun && card.runUrl ? (
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
                    )}
                    <div className="flex flex-wrap items-center gap-2">
                      {renderDetailsLink(
                        card,
                        "text-sm font-semibold hover:text-[var(--atlas-home-link)] hover:underline",
                        "text-sm font-semibold",
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
                  <div className="flex min-h-24 flex-col justify-between gap-3 text-sm leading-6 text-[var(--atlas-home-text)]">
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

                <div className="grid grid-cols-2 border-t border-[var(--atlas-home-border-soft)] md:grid-cols-4">
                  <div className="atlas-home-footer-cell inline-flex items-center justify-center gap-2 border-r border-[var(--atlas-home-border-soft)] text-center text-sm">
                    <Star
                      className={`h-4 w-4 ${card.isStarred ? "atlas-home-star-icon" : "text-[var(--atlas-home-muted)]"}`}
                      strokeWidth={1.8}
                      fill={card.isStarred ? "currentColor" : "none"}
                    />
                    <span>
                      {card.isStarred ? "Starred" : "Star"} {card.starCount ?? 0}
                    </span>
                  </div>
                  {card.canEdit && card.editUrl ? (
                    renderFooterLink(
                      card.editUrl,
                      <Pencil className="h-4 w-4" strokeWidth={1.8} />,
                      "Edit",
                    )
                  ) : (
                    <div className="atlas-home-footer-cell border-r border-[var(--atlas-home-border-soft)] text-center text-sm text-[var(--atlas-home-muted)]">
                      Edit
                    </div>
                  )}
                  {card.canManage && card.manageUrl ? (
                    renderFooterLink(
                      card.manageUrl,
                      <Settings className="h-4 w-4" strokeWidth={1.8} />,
                      "Manage",
                    )
                  ) : (
                    <div className="atlas-home-footer-cell border-r border-[var(--atlas-home-border-soft)] text-center text-sm text-[var(--atlas-home-muted)]">
                      Manage
                    </div>
                  )}
                  <div className="atlas-home-footer-cell inline-flex items-center justify-center gap-3 text-center text-sm">
                    {card.canOpenProfile ? (
                      <a
                        href={card.href}
                        aria-label="Open report profile"
                        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
                      >
                        <BarChart3 className="h-4 w-4" strokeWidth={1.8} />
                      </a>
                    ) : null}
                    {card.canShare ? (
                      <button
                        type="button"
                        aria-label="Share report"
                        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
                      >
                        <Share2 className="h-4 w-4" strokeWidth={1.8} />
                      </button>
                    ) : null}
                    {card.canRequestAccess ? (
                      <button
                        type="button"
                        aria-label="Request access"
                        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
                      >
                        <GlobeLock className="h-4 w-4" strokeWidth={1.8} />
                      </button>
                    ) : null}
                  </div>
                </div>
              </article>
            ))
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
