"use client"

import type { SyntheticEvent } from "react"
import {
  getReportImagePlaceholderPath,
  getReportThumbnailPath,
} from "@/lib/reports/report-image-url"
import type { ReportDetail } from "@/lib/reports/types"

function fallbackTo(fallbackSrc: string) {
  return (event: SyntheticEvent<HTMLImageElement>) => {
    const el = event.currentTarget
    if (el.dataset.atlasThumbFallback === "1") return
    el.dataset.atlasThumbFallback = "1"
    el.src = fallbackSrc
  }
}

export function ReportImagesSection({ report }: { report: ReportDetail }) {
  const images = [...(report.images ?? [])].sort((a, b) => (a.ordinal ?? 0) - (b.ordinal ?? 0))
  const onError = fallbackTo(getReportImagePlaceholderPath({ size: "1200x400" }))

  if (images.length === 0) return null

  return (
    <section id="images" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Images</h2>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {images.map((image, index) => (
          <a
            key={image.id}
            href={getReportThumbnailPath(report.id, { imageId: image.id, size: "1200x2000" })}
            target="_blank"
            rel="noreferrer"
            className="block rounded-md border p-2"
          >
            <img
              src={getReportThumbnailPath(report.id, { imageId: image.id, size: "1200x400" })}
              alt={`${report.displayTitle || report.name} screenshot ${index + 1}`}
              className="h-auto w-full"
              loading="lazy"
              onError={onError}
            />
          </a>
        ))}
      </div>
    </section>
  )
}
