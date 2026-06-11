"use client"

import { Pencil, Settings } from "lucide-react"
import Link from "next/link"
import type { ReactNode } from "react"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"
import { RequestAccessDialog } from "@/components/interactions/request-access-dialog"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { CollectionFeatureFlagsDto } from "@/lib/collections/types"
import { isInteractionFeatureEnabled } from "@/lib/interactions/features"
import type { ReportFeatureFlags } from "@/lib/reports/types"

type InteractionFeatures = CollectionFeatureFlagsDto | ReportFeatureFlags | undefined

function readFeatureFlag(
  features: InteractionFeatures,
  key: "sharingEnabled" | "requestAccessEnabled",
) {
  const value = features?.[key]
  return typeof value === "boolean" ? value : undefined
}

function renderFooterLink(href: string, icon: ReactNode, label: string) {
  return (
    <a
      href={href}
      target="_blank"
      rel="noreferrer"
      className="atlas-home-footer-cell inline-flex items-center justify-center gap-2 border-r border-[var(--atlas-home-border-soft)] text-center text-sm text-[var(--atlas-home-link)] hover:bg-[var(--atlas-home-surface-muted)]"
    >
      {icon}
      <span>{label}</span>
    </a>
  )
}

function renderPlaceholderCell(label: string) {
  return (
    <div className="atlas-home-footer-cell border-r border-[var(--atlas-home-border-soft)] text-center text-sm text-[var(--atlas-home-muted)]">
      {label}
    </div>
  )
}

function ProfileShareGroup({
  id,
  title,
  href,
  profileType,
  profileLabel,
  showProfile,
  showShare,
}: {
  id: number
  title: string
  href: string
  profileType: "collection" | "report"
  profileLabel: string
  showProfile: boolean
  showShare: boolean
}) {
  return (
    <div className="atlas-home-footer-cell inline-flex items-center justify-center gap-3 text-center text-sm">
      {showProfile ? (
        <EntityProfileSheet entityName={title} entityLabel={profileLabel} variant="footer">
          <ProfileAnalyticsPanel id={id} type={profileType} />
        </EntityProfileSheet>
      ) : null}
      {showShare ? (
        <ShareMailDialog shareName={title} shareUrl={href} iconOnly variant="footer" />
      ) : null}
    </div>
  )
}

export type EntityCardFooterProps = {
  entityType: "collection" | "report"
  id: number
  title: string
  href: string
  isStarred?: boolean
  starCount?: number
  features?: InteractionFeatures
  canEdit?: boolean
  editUrl?: string | null
  canManage?: boolean
  manageUrl?: string | null
  canOpenProfile?: boolean
  canRequestAccess?: boolean
}

export function EntityCardFooter({
  entityType,
  id,
  title,
  href,
  isStarred = false,
  starCount = 0,
  features,
  canEdit = false,
  editUrl,
  canManage = false,
  manageUrl,
  canOpenProfile = true,
  canRequestAccess = false,
}: EntityCardFooterProps) {
  const starType = entityType
  const profileLabel = entityType === "collection" ? "collection profile" : "report profile"
  const sharingEnabled = isInteractionFeatureEnabled(readFeatureFlag(features, "sharingEnabled"))
  const requestAccessEnabled =
    isInteractionFeatureEnabled(readFeatureFlag(features, "requestAccessEnabled")) &&
    canRequestAccess

  const starCell = (
    <StarToggleButton
      type={starType}
      id={id}
      initialStarred={isStarred}
      initialCount={starCount}
      variant="card-footer"
    />
  )

  if (entityType === "collection") {
    return (
      <div className="grid grid-cols-2 border-t border-[var(--atlas-home-border-soft)]">
        {starCell}
        <ProfileShareGroup
          id={id}
          title={title}
          href={href}
          profileType="collection"
          profileLabel={profileLabel}
          showProfile={canOpenProfile}
          showShare={sharingEnabled}
        />
      </div>
    )
  }

  return (
    <div className="grid grid-cols-2 border-t border-[var(--atlas-home-border-soft)] md:grid-cols-4">
      {starCell}
      {canEdit && editUrl
        ? renderFooterLink(editUrl, <Pencil className="h-4 w-4" strokeWidth={1.8} />, "Edit")
        : renderPlaceholderCell("Edit")}
      {canManage && manageUrl
        ? renderFooterLink(manageUrl, <Settings className="h-4 w-4" strokeWidth={1.8} />, "Manage")
        : renderPlaceholderCell("Manage")}
      <div className="atlas-home-footer-cell inline-flex items-center justify-center gap-3 text-center text-sm">
        {canOpenProfile ? (
          <EntityProfileSheet entityName={title} entityLabel={profileLabel} variant="footer">
            <ProfileAnalyticsPanel id={id} type="report" />
          </EntityProfileSheet>
        ) : (
          <Link
            href={href}
            aria-label="Open report details"
            className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
          >
            Details
          </Link>
        )}
        {sharingEnabled ? (
          <ShareMailDialog shareName={title} shareUrl={href} iconOnly variant="footer" />
        ) : null}
        {requestAccessEnabled ? (
          <RequestAccessDialog reportName={title} reportUrl={href} variant="footer" />
        ) : null}
      </div>
    </div>
  )
}
