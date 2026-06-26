"use client"

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { Button } from "@/components/ui/button"

type AppAlertDialogProps = {
  triggerLabel: string
  title: string
  description: string
  confirmLabel?: string
  cancelLabel?: string
  intent?: "confirm" | "error"
  triggerClassName?: string
  onConfirm?: () => void
}

export function AppAlertDialog({
  triggerLabel,
  title,
  description,
  confirmLabel = "Continue",
  cancelLabel = "Cancel",
  intent = "confirm",
  triggerClassName,
  onConfirm,
}: AppAlertDialogProps) {
  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>
        <Button
          variant={intent === "error" ? "destructive" : "outline"}
          size="sm"
          className={triggerClassName}
        >
          {triggerLabel}
        </Button>
      </AlertDialogTrigger>
      <AlertDialogContent size="default">
        <AlertDialogHeader>
          <AlertDialogTitle>{title}</AlertDialogTitle>
          <AlertDialogDescription>{description}</AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>{cancelLabel}</AlertDialogCancel>
          <AlertDialogAction
            variant={intent === "error" ? "destructive" : "default"}
            onClick={onConfirm}
          >
            {confirmLabel}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
