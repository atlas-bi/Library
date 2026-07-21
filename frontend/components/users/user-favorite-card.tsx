import { BadgeCheck, PlayCircle } from "lucide-react"
import Link from "next/link"
import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ReportSnippetRunAction } from "@/components/interactions/report-snippet-run-action"
import { ProfileAnalyticsPanelClient } from "@/components/profile/profile-analytics-panel-client"
import { Badge } from "@/components/ui/badge"
import { truncateText } from "@/lib/text"
import type { UserFavoriteItem } from "@/lib/users/types"

function resolveProfileType(type?: string | null): string | null {
  const normalized = type?.toLowerCase()
  if (
    normalized === "report" ||
    normalized === "collection" ||
    normalized === "group" ||
    normalized === "user"
  ) {
    return normalized
  }
  return null
}

export function UserFavoriteCard({ item }: { item: UserFavoriteItem }) {
  const title = item.name?.trim() || item.searchString?.trim() || "Favorite"
  const href = item.url?.trim() || undefined
  const typeLabel = item.typeLabel?.trim() || item.type?.trim() || "item"
  const isReport = typeLabel.toLowerCase() === "report"
  const isCollection = typeLabel.toLowerCase() === "collection"
  const excerpt = item.description?.trim() || item.bodyText?.trim() || item.secondaryText?.trim()
  const profileType = resolveProfileType(item.type)
  const profileId = item.profileTargetId ? Number(item.profileTargetId) : item.itemId

  const card = (
    <article
      className={`overflow-hidden ${isCollection ? "atlas-snippet-gold-card" : "atlas-home-card"}`}
    >
      <div className="gap-2 border-b border-[var(--atlas-home-border-soft)] px-4 py-2.5">
        <div className="flex flex-wrap items-start justify-between gap-2">
          <div className="flex min-w-0 flex-1 items-center gap-3 text-[var(--atlas-home-text-strong)]">
            {isReport && item.itemId ? (
              <ReportSnippetRunAction
                reportId={item.itemId}
                canRun={item.canRun}
                attachmentCount={item.canRequestAccess ? 1 : 0}
                runUrl={item.runUrl}
                epicMasterFile={item.epicMasterFile}
                editReportUrl={item.editUrl}
              />
            ) : null}
            <div className="min-w-0 flex-1">
              {href ? (
                <Link href={href} className="atlas-home-card-title hover:underline">
                  {title}
                </Link>
              ) : (
                <span className="atlas-home-card-title">{title}</span>
              )}
              {item.isCertified ? (
                <BadgeCheck
                  className="ml-1.5 inline size-4 text-[var(--atlas-home-link)]"
                  aria-label="Certified"
                />
              ) : null}
            </div>
          </div>
          <div className="flex flex-wrap justify-end gap-1">
            <Badge variant="secondary" className="text-xs uppercase tracking-wide">
              {typeLabel}
            </Badge>
            {item.tags
              .filter((tag) => tag.showInHeader && tag.name)
              .map((tag) => (
                <Badge
                  key={`${item.starId}-${tag.slug ?? tag.name}`}
                  variant="outline"
                  className="text-xs"
                >
                  {tag.name}
                </Badge>
              ))}
          </div>
        </div>
      </div>
      <div className="px-4 py-4">
        <div className="grid gap-4 md:grid-cols-[128px_1fr]">
          {item.thumbnailUrl || item.placeholderImageUrl ? (
            <img
              src={item.thumbnailUrl ?? item.placeholderImageUrl ?? ""}
              alt={`${title} thumbnail`}
              className="size-32 rounded-lg border border-[var(--atlas-home-border-soft)] object-cover"
            />
          ) : (
            <div className="atlas-home-thumbnail flex size-32 shrink-0 items-center justify-center rounded-lg border border-[var(--atlas-home-border-soft)] text-[var(--atlas-home-muted)]">
              {isReport ? <PlayCircle className="size-10 opacity-60" /> : null}
            </div>
          )}
          <div className="flex min-h-24 min-w-0 flex-col justify-between gap-3 text-sm font-medium leading-6 text-[var(--atlas-home-text)]">
            <p>{excerpt ? truncateText(excerpt) : "Open to view details."}</p>
            {href ? (
              <Link href={href} className="text-[var(--atlas-home-link)] hover:underline">
                read more
              </Link>
            ) : null}
          </div>
        </div>
      </div>
      {isReport && item.itemId ? (
        <div className="border-t border-[var(--atlas-home-border-soft)] p-0">
          <EntityCardFooter
            entityType="report"
            id={item.itemId}
            title={title}
            href={href ?? `/reports?id=${item.itemId}`}
            isStarred={item.isStarred}
            starCount={item.starCount}
            canEdit={item.canEditInEditor}
            editUrl={item.editUrl}
            canManage={item.canManageInEditor}
            manageUrl={item.manageUrl}
            canOpenProfile={item.canOpenProfile}
            canRequestAccess={item.canRequestAccess}
            profilePanel={
              item.canOpenProfile && profileType && profileId ? (
                <ProfileAnalyticsPanelClient id={profileId} type={profileType} />
              ) : undefined
            }
            features={{
              sharingEnabled: item.canShare ? undefined : false,
              requestAccessEnabled: item.canRequestAccess ? undefined : false,
            }}
          />
        </div>
      ) : isCollection && item.itemId ? (
        <div className="border-t border-[var(--atlas-home-border-soft)] p-0">
          <EntityCardFooter
            entityType="collection"
            id={item.itemId}
            title={title}
            href={href ?? `/collections?id=${item.itemId}`}
            isStarred={item.isStarred}
            starCount={item.starCount}
            canEdit={item.canEditInEditor}
            editUrl={item.editUrl}
            canManage={item.canManageInEditor}
            manageUrl={item.manageUrl}
            canOpenProfile={item.canOpenProfile}
            profilePanel={
              item.canOpenProfile ? (
                <ProfileAnalyticsPanelClient id={item.itemId} type="collection" />
              ) : undefined
            }
          />
        </div>
      ) : null}
    </article>
  )

  return card
}
