"use client"

import { ThumbsUp } from "lucide-react"
import { FeedbackForm } from "@/components/interactions/feedback-form"
import { InteractionTooltip } from "@/components/interactions/interaction-tooltip"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"

export function EntityFeedbackDialog({
  entityName,
  entityUrl,
  variant = "default",
}: {
  entityName: string
  entityUrl: string
  variant?: "default" | "footer"
}) {
  const tooltipPlacement = variant === "footer" ? "footer" : "rail"
  const trigger =
    variant === "footer" ? (
      <button
        type="button"
        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
      >
        <ThumbsUp className="h-4 w-4" strokeWidth={1.8} />
        <span className="sr-only">Share feedback</span>
      </button>
    ) : (
      <Button type="button" variant="ghost" size="icon" className="atlas-action-rail-button">
        <ThumbsUp className="size-5" />
        <span className="sr-only">Share feedback</span>
      </Button>
    )

  return (
    <Dialog>
      <InteractionTooltip label="Share feedback" placement={tooltipPlacement}>
        <DialogTrigger asChild>{trigger}</DialogTrigger>
      </InteractionTooltip>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Share feedback</DialogTitle>
        </DialogHeader>
        <FeedbackForm entityName={entityName} entityUrl={entityUrl} />
      </DialogContent>
    </Dialog>
  )
}
