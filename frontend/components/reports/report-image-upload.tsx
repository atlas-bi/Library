"use client"

import { useRouter } from "next/navigation"
import { useRef, useState, useTransition } from "react"
import { uploadReportImageAction } from "@/app/reports/actions"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { cn } from "@/lib/utils"

export function ReportImageUpload({
  reportId,
  variant = "stacked",
}: {
  reportId: number
  variant?: "stacked" | "inline"
}) {
  const router = useRouter()
  const inputRef = useRef<HTMLInputElement>(null)
  const [error, setError] = useState<string | null>(null)
  const [message, setMessage] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  const isInline = variant === "inline"

  return (
    <div className={cn("space-y-2", isInline && "space-y-3")}>
      <Label htmlFor={`report-image-${reportId}`} className={cn(isInline && "sr-only")}>
        Upload image (max 1 MB)
      </Label>
      {isInline ? (
        <p className="text-sm text-muted-foreground">PNG, JPEG, or GIF — maximum 1 MB per file.</p>
      ) : null}
      <div
        className={cn(
          "flex gap-3",
          isInline ? "flex-col sm:flex-row sm:items-center" : "flex-col",
        )}
      >
        <input
          ref={inputRef}
          id={`report-image-${reportId}`}
          type="file"
          accept="image/png,image/jpeg,image/gif,image/webp"
          className={cn(
            "block text-sm file:mr-3 file:rounded-md file:border file:border-input file:bg-background file:px-3 file:py-1.5 file:text-sm file:font-medium",
            isInline && "min-w-0 flex-1",
          )}
          disabled={pending}
        />
        <Button
          type="button"
          disabled={pending}
          className={cn(isInline && "shrink-0 sm:w-auto")}
          onClick={() => {
            const input = inputRef.current
            if (!input?.files?.[0]) {
              setError("Choose an image first.")
              return
            }
            setError(null)
            setMessage(null)
            const formData = new FormData()
            formData.append("file", input.files[0])
            startTransition(() => {
              void (async () => {
                const result = await uploadReportImageAction(reportId, formData)
                if ("error" in result && result.error) {
                  setError(result.error)
                  return
                }
                setMessage("Image uploaded.")
                input.value = ""
                router.refresh()
              })()
            })
          }}
        >
          {pending ? "Uploading…" : "Upload"}
        </Button>
      </div>
      {error ? <p className="text-sm text-destructive">{error}</p> : null}
      {message ? <p className="text-sm text-muted-foreground">{message}</p> : null}
    </div>
  )
}