"use client"

import { ThumbsUp } from "lucide-react"
import { FeedbackForm } from "@/components/interactions/feedback-form"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"

export function CollectionFeedbackDialog({
  collectionName,
  collectionUrl,
}: {
  collectionName: string
  collectionUrl: string
}) {
  return (
    <Dialog>
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            <DialogTrigger asChild>
              <Button type="button" variant="ghost" size="icon" className="size-10">
                <ThumbsUp className="size-5" />
                <span className="sr-only">Share feedback</span>
              </Button>
            </DialogTrigger>
          </TooltipTrigger>
          <TooltipContent side="right">Share feedback</TooltipContent>
        </Tooltip>
      </TooltipProvider>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Share feedback</DialogTitle>
        </DialogHeader>
        <FeedbackForm reportName={collectionName} reportUrl={collectionUrl} />
      </DialogContent>
    </Dialog>
  )
}
