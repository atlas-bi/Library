"use client"

import { Pencil, Plus, Trash2 } from "lucide-react"
import { useRouter } from "next/navigation"
import type { ReactNode } from "react"
import { useTransition } from "react"
import { deleteTermAction } from "@/app/terms/actions"
import {
  ActionRail,
  ActionRailGroup,
  RailIconLink,
  RailTooltipButton,
} from "@/components/interactions/action-rail"
import { EntityEngagementRailActions } from "@/components/interactions/entity-engagement-rail-actions"
import type { TermDetailDto } from "@/lib/terms/types"

function RailDeleteButton({
  termId,
  termName,
}: {
  termId: number
  termName: string
}) {
  const router = useRouter()
  const [pending, startTransition] = useTransition()

  return (
    <RailTooltipButton
      label="Delete this term"
      disabled={pending}
      className="atlas-action-rail-button text-destructive hover:text-destructive"
      onClick={() => {
        const label = termName.trim() || `term ${termId}`
        if (!window.confirm(`Delete ${label}? This cannot be undone.`)) return
        startTransition(() => {
          void (async () => {
            const result = await deleteTermAction(termId)
            if (!result?.error) router.refresh()
          })()
        })
      }}
    >
      <Trash2 className="size-5" />
      <span className="sr-only">Delete term</span>
    </RailTooltipButton>
  )
}

export function TermActionRail({
  term,
  profilePanel,
}: {
  term: TermDetailDto
  profilePanel: ReactNode
}) {
  const features = term.features ?? {}
  const shareUrl = `/terms?id=${term.id}`

  const hasAdminActions =
    term.permissions?.canCreateTerm || 
    term.permissions?.canEditTerm || 
    term.permissions?.canDeleteTerm

  return (
    <ActionRail label="Term actions">
      <ActionRailGroup>
        <EntityEngagementRailActions
          entityType="term"
          entityId={term.id}
          entityName={term.name}
          entityUrl={shareUrl}
          profileLabel="term profile"
          
          profilePanel={profilePanel}
          isStarred={term.isStarred}
          starCount={term.starCount}
          
          features={features}
        />
      </ActionRailGroup>

      {hasAdminActions ? (
        <ActionRailGroup separated>
          {term.permissions?.canCreateTerm ? (
            <RailIconLink href="/terms/new" label="Create new term">
              <Plus className="size-5" />
              <span className="sr-only">Create new term</span>
            </RailIconLink>
          ) : null}

          {term.permissions?.canEditTerm ? (
            <RailIconLink href={`/terms/edit?id=${term.id}`} label="Open Atlas editor">
              <Pencil className="size-5" />
              <span className="sr-only">Edit term</span>
            </RailIconLink>
          ) : null}

          {term.permissions?.canDeleteTerm ? (
            <RailDeleteButton termId={term.id} termName={term.name} />
          ) : null}
        </ActionRailGroup>
      ) : null}
    </ActionRail>
  )
}
