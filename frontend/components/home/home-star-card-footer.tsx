import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ProfileAnalyticsPanelClient } from "@/components/profile/profile-analytics-panel-client"
import type { HomeStarCard } from "@/lib/home/types"

function resolveStarType(card: HomeStarCard): "collection" | "report" | null {
  const label = card.typeLabel.toLowerCase()
  if (label === "collection") return "collection"
  if (label === "report") return "report"
  return null
}

export function HomeStarCardFooter({ card }: { card: HomeStarCard }) {
  const starType = resolveStarType(card)
  if (!starType) return null

  const profileType = starType

  return (
    <EntityCardFooter
      entityType={starType}
      id={card.id}
      title={card.title}
      href={card.href}
      isStarred={card.isStarred}
      starCount={card.starCount}
      canEdit={card.canEdit}
      editUrl={card.editUrl}
      canManage={card.canManage}
      manageUrl={card.manageUrl}
      canOpenProfile={card.canOpenProfile ?? true}
      canRequestAccess={card.canRequestAccess}
      profilePanel={
        card.canOpenProfile !== false ? (
          <ProfileAnalyticsPanelClient id={card.id} type={profileType} />
        ) : undefined
      }
      features={{
        sharingEnabled: card.canShare === false ? false : undefined,
        requestAccessEnabled: card.canRequestAccess === false ? false : undefined,
      }}
    />
  )
}
