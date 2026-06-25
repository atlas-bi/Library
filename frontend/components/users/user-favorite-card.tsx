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
      className={`overflow-hidden border border-border/70 bg-card ${isCollection ? "ring-1 ring-amber-200/70" : ""}`}
    >
      <div className="gap-2 border-b border-border/60 px-4 py-3">
        <div className="flex flex-wrap items-start justify-between gap-2">
          <div className="flex min-w-0 flex-1 items-center gap-2">
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
                <Link href={href} className="font-semibold hover:underline">
                  {title}
                </Link>
              ) : (
                <span className="font-semibold">{title}</span>
              )}
              {item.isCertified ? (
                <BadgeCheck className="ml-1.5 inline size-4 text-info" aria-label="Certified" />
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
      <div className="py-3">
        <div className="flex gap-4">
          {item.thumbnailUrl || item.placeholderImageUrl ? (
            // biome-ignore lint/performance/noImgElement: backend-provided thumbnails for workspace cards.
            <img
              src={item.thumbnailUrl ?? item.placeholderImageUrl ?? ""}
              alt={`${title} thumbnail`}
              className="size-32 rounded-md border object-cover"
            />
          ) : (
            <div className="flex size-32 shrink-0 items-center justify-center rounded-md border bg-muted/50 text-muted-foreground">
              {isReport ? <PlayCircle className="size-10 opacity-60" /> : null}
            </div>
          )}
          <div className="min-w-0 flex-1 text-sm text-muted-foreground">
            <p>{excerpt ? truncateText(excerpt) : "Open to view details."}</p>
            {href ? (
              <Link href={href} className="mt-2 inline-block text-link hover:underline">
                read more
              </Link>
            ) : null}
          </div>
        </div>
      </div>
      {isReport && item.itemId ? (
        <div className="border-t p-0">
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
        <div className="border-t p-0">
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
