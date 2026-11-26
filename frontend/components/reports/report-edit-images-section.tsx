"use client"

import { ArrowDown, ArrowUp, GripVertical, ImageIcon, Loader2, Trash2 } from "lucide-react"
import { useRouter } from "next/navigation"
import { useCallback, useEffect, useState, useTransition } from "react"
import { updateReportAction } from "@/app/reports/actions"
import { ReportImageUpload } from "@/components/reports/report-image-upload"
import { Button } from "@/components/ui/button"
import {
  getReportImagePlaceholderPath,
  getReportThumbnailPath,
} from "@/lib/reports/report-image-url"
import { buildReportUpdateBodyFromDetail } from "@/lib/reports/report-update-body"
import type { ReportDetail } from "@/lib/reports/types"

const THUMB_SIZE = "256x256"
const STATIC_PLACEHOLDER = "/img/report_placeholder_256x256.png"

function sortImageIds(report: ReportDetail): number[] {
  const images = report.images ?? []
  return [...images]
    .sort((a, b) => (a.ordinal ?? 0) - (b.ordinal ?? 0))
    .map((image) => image.id)
}

function thumbSrc(reportId: number, imageId: number) {
  return getReportThumbnailPath(reportId, { imageId, size: THUMB_SIZE })
}

function placeholderSrc() {
  return getReportImagePlaceholderPath({ size: THUMB_SIZE })
}

export function ReportEditImagesSection({
  report,
  onBack,
  onContinue,
}: {
  report: ReportDetail
  onBack: () => void
  onContinue: () => void
}) {
  const router = useRouter()
  const [imageIds, setImageIds] = useState<number[]>(() => sortImageIds(report))
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  useEffect(() => {
    setImageIds(sortImageIds(report))
  }, [report])

  const persistImageIds = useCallback(
    (nextIds: number[]) => {
      setError(null)
      startTransition(() => {
        void (async () => {
          const body = buildReportUpdateBodyFromDetail(report, { imageIds: nextIds })
          const result = await updateReportAction(report.id, body)
          if (result?.error) {
            setError(result.error)
            return
          }
          setImageIds(nextIds)
          router.refresh()
        })()
      })
    },
    [report, router],
  )

  const removeImage = (imageId: number) => {
    persistImageIds(imageIds.filter((id) => id !== imageId))
  }

  const moveImage = (index: number, direction: -1 | 1) => {
    const target = index + direction
    if (target < 0 || target >= imageIds.length) {
      return
    }
    const next = [...imageIds]
    const temp = next[index]
    next[index] = next[target]
    next[target] = temp
    persistImageIds(next)
  }

  return (
    <div className="mx-auto w-full max-w-3xl space-y-6">
      <div>
        <p className="text-sm text-muted-foreground">
          Drag order is saved when you move images up or down. Delete removes the image from this
          report.
        </p>
        {pending ? (
          <p className="mt-2 inline-flex items-center gap-2 text-sm text-muted-foreground">
            <Loader2 className="size-4 animate-spin" aria-hidden />
            Saving image changes…
          </p>
        ) : null}
        {error ? <p className="mt-2 text-sm text-destructive">{error}</p> : null}
      </div>

      {imageIds.length > 0 ? (
        <ul className="flex flex-wrap gap-4">
          {imageIds.map((imageId, index) => (
            <li
              key={imageId}
              className="w-full max-w-[17rem] flex-shrink-0 rounded-lg border border-[var(--atlas-home-border-soft)] bg-white shadow-sm"
            >
              <div className="flex items-center justify-between gap-2 border-b border-[var(--atlas-home-border-soft)] px-3 py-2">
                <span className="inline-flex items-center gap-1.5 text-xs font-medium text-muted-foreground">
                  <GripVertical className="size-4 shrink-0" aria-hidden />
                  Image {index + 1}
                </span>
                <div className="flex items-center gap-0.5">
                  <Button
                    type="button"
                    size="icon"
                    variant="ghost"
                    className="size-8"
                    disabled={pending || index === 0}
                    aria-label={`Move image ${imageId} up`}
                    onClick={() => {
                      moveImage(index, -1)
                    }}
                  >
                    <ArrowUp className="size-4" />
                  </Button>
                  <Button
                    type="button"
                    size="icon"
                    variant="ghost"
                    className="size-8"
                    disabled={pending || index === imageIds.length - 1}
                    aria-label={`Move image ${imageId} down`}
                    onClick={() => {
                      moveImage(index, +1)
                    }}
                  >
                    <ArrowDown className="size-4" />
                  </Button>
                  <Button
                    type="button"
                    size="icon"
                    variant="ghost"
                    className="size-8 text-destructive hover:bg-destructive/10 hover:text-destructive"
                    disabled={pending}
                    aria-label={`Delete image ${imageId}`}
                    onClick={() => {
                      removeImage(imageId)
                    }}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              </div>
              <div className="flex aspect-square items-center justify-center bg-muted/40 p-2">
                <img
                  src={thumbSrc(report.id, imageId)}
                  alt={`Screenshot ${index + 1}`}
                  className="max-h-full max-w-full rounded-md object-contain"
                  onError={(event) => {
                    const el = event.currentTarget
                    const step = el.dataset.atlasThumbFallback ?? "0"
                    if (step === "0") {
                      el.dataset.atlasThumbFallback = "1"
                      el.src = placeholderSrc()
                      return
                    }
                    if (step === "1") {
                      el.dataset.atlasThumbFallback = "2"
                      el.src = STATIC_PLACEHOLDER
                    }
                  }}
                />
              </div>
            </li>
          ))}
        </ul>
      ) : (
        <div className="flex flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-[var(--atlas-home-border-soft)] bg-muted/20 px-6 py-12 text-center">
          <ImageIcon className="size-10 text-muted-foreground/70" aria-hidden />
          <p className="text-sm font-medium text-foreground">No images yet</p>
          <p className="max-w-sm text-sm text-muted-foreground">
            Upload a PNG, JPEG, or GIF up to 1 MB. It will appear in report documentation and
            search.
          </p>
        </div>
      )}

      <h3 className="text-sm font-semibold text-foreground">Add image</h3>
      <div className="rounded-lg border border-dashed border-[var(--atlas-home-border-soft)] bg-muted/10 p-4">
        <ReportImageUpload reportId={report.id} variant="inline" />
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3 border-t border-[var(--atlas-home-border-soft)] pt-4">
        <Button type="button" variant="outline" onClick={onBack} disabled={pending}>
          Back
        </Button>
        <Button type="button" onClick={onContinue} disabled={pending}>
          Next
        </Button>
      </div>
    </div>
  )
}