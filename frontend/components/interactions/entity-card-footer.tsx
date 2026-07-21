"use client"

import { Pencil, Settings } from "lucide-react"
import type { ReactNode } from "react"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"
import { FooterIconAction, FooterIconActions } from "@/components/interactions/footer-icon-actions"
import { RequestAccessDialog } from "@/components/interactions/request-access-dialog"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
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

function renderFooterLink(href: string, icon: ReactNode, label: string, key: string) {
  return (
    <a
      key={key}
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

export type EntityCardFooterProps = {
  entityType: "collection" | "report" | "term"
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
  profilePanel?: ReactNode
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
  profilePanel,
}: EntityCardFooterProps) {
  const starType = entityType
  const profileLabel = entityType === "collection"
    ? "collection profile"
    : entityType === "term"
      ? "term profile"
      : "report profile"
  const sharingEnabled = isInteractionFeatureEnabled(readFeatureFlag(features, "sharingEnabled"))
  const requestAccessEnabled =
    isInteractionFeatureEnabled(readFeatureFlag(features, "requestAccessEnabled")) &&
    canRequestAccess

  const starCell = (
    <StarToggleButton
      key="star"
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
        <FooterIconActions>
          {canOpenProfile && profilePanel ? (
            <FooterIconAction label={`Open ${profileLabel}`}>
              <EntityProfileSheet entityName={title} entityLabel={profileLabel} variant="footer">
                {profilePanel}
              </EntityProfileSheet>
            </FooterIconAction>
          ) : null}
          {sharingEnabled ? (
            <FooterIconAction label="Share">
              <ShareMailDialog shareName={title} shareUrl={href} iconOnly variant="footer" />
            </FooterIconAction>
          ) : null}
        </FooterIconActions>
      </div>
    )
  }

  const segments: ReactNode[] = [starCell]

  if (canEdit && editUrl) {
    segments.push(
      renderFooterLink(editUrl, <Pencil className="h-4 w-4" strokeWidth={1.8} />, "Edit", "edit"),
    )
  }

  if (canManage && manageUrl) {
    segments.push(
      renderFooterLink(
        manageUrl,
        <Settings className="h-4 w-4" strokeWidth={1.8} />,
        "Manage",
        "manage",
      ),
    )
  }

  const hasIconActions = (canOpenProfile && profilePanel) || sharingEnabled || requestAccessEnabled

  if (hasIconActions) {
    segments.push(
      <FooterIconActions key="icon-actions">
        {canOpenProfile && profilePanel ? (
          <FooterIconAction label={`Open ${profileLabel}`}>
            <EntityProfileSheet entityName={title} entityLabel={profileLabel} variant="footer">
              {profilePanel}
            </EntityProfileSheet>
          </FooterIconAction>
        ) : null}
        {sharingEnabled ? (
          <FooterIconAction label="Share">
            <ShareMailDialog shareName={title} shareUrl={href} iconOnly variant="footer" />
          </FooterIconAction>
        ) : null}
        {requestAccessEnabled ? (
          <FooterIconAction label="Request access">
            <RequestAccessDialog reportName={title} reportUrl={href} variant="footer" />
          </FooterIconAction>
        ) : null}
      </FooterIconActions>,
    )
  }

  const columnClass =
    segments.length === 2
      ? "grid-cols-2"
      : segments.length === 3
        ? "grid-cols-3"
        : "grid-cols-2 md:grid-cols-4"

  return (
    <div className={`grid border-t border-[var(--atlas-home-border-soft)] ${columnClass}`}>
      {segments}
    </div>
  )
}
