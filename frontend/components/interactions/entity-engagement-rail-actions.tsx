"use client"

import type { ReactNode } from "react"
import { EntityFeedbackDialog } from "@/components/interactions/entity-feedback-dialog"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"
import { RequestAccessDialog } from "@/components/interactions/request-access-dialog"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import { isInteractionFeatureEnabled } from "@/lib/interactions/features"
import type { InteractionEntityType } from "@/lib/interactions/types"

type EngagementFeatures = {
  sharingEnabled?: boolean
  feedbackEnabled?: boolean
  requestAccessEnabled?: boolean
}

export function EntityEngagementRailActions({
  entityType,
  entityId,
  entityName,
  entityUrl,
  profileLabel,
  profilePanel,
  isStarred,
  starCount,
  features,
  showRequestAccess = false,
  afterStar,
}: {
  entityType: InteractionEntityType
  entityId: number
  entityName: string
  entityUrl: string
  profileLabel: string
  profilePanel: ReactNode
  isStarred?: boolean
  starCount?: number
  features?: EngagementFeatures
  showRequestAccess?: boolean
  afterStar?: ReactNode
}) {
  return (
    <>
      <EntityProfileSheet entityName={entityName} entityLabel={profileLabel}>
        {profilePanel}
      </EntityProfileSheet>

      <StarToggleButton
        type={entityType}
        id={entityId}
        initialStarred={isStarred ?? false}
        initialCount={starCount ?? 0}
        iconOnly
      />

      {afterStar}

      {isInteractionFeatureEnabled(features?.sharingEnabled) ? (
        <ShareMailDialog shareName={entityName} shareUrl={entityUrl} iconOnly />
      ) : null}

      {showRequestAccess && isInteractionFeatureEnabled(features?.requestAccessEnabled) ? (
        <RequestAccessDialog reportName={entityName} reportUrl={entityUrl} />
      ) : null}

      {isInteractionFeatureEnabled(features?.feedbackEnabled) ? (
        <EntityFeedbackDialog entityName={entityName} entityUrl={entityUrl} />
      ) : null}
    </>
  )
}
