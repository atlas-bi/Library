import Link from "next/link"
import { MarkdownContent } from "@/components/content/markdown-content"
import type { ReportDetail } from "@/lib/reports/types"

export function ReportMaintenanceSection({
  report,
  canViewUserProfiles,
}: {
  report: ReportDetail
  canViewUserProfiles?: boolean
}) {
  const logs = report.document?.maintenanceLogs ?? []
  if (logs.length === 0) return null

  return (
    <section id="maintenance" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Maintenance</h2>
      <div className="overflow-hidden rounded-md border border-[var(--atlas-home-border-soft)]">
        {logs.map((log) => {
          const maintainer = log.maintainer
          const maintainerLabel =
            maintainer?.fullName?.trim() || maintainer?.username?.trim() || "Unknown maintainer"

          return (
            <div
              key={log.id}
              className="border-b border-[var(--atlas-home-border-soft)] px-4 py-3 text-sm last:border-b-0"
            >
              <div>
                {maintainer && canViewUserProfiles ? (
                  <Link href={`/users?id=${maintainer.id}`} className="text-link hover:underline">
                    {maintainerLabel}
                  </Link>
                ) : (
                  maintainerLabel
                )}
                {log.maintenanceDate ? (
                  <span className="text-muted-foreground"> · {log.maintenanceDate}</span>
                ) : null}
                {log.status?.name ? <span> · {log.status.name}</span> : null}
              </div>
              {log.comment?.trim() ? (
                <div className="mt-2">
                  <MarkdownContent content={log.comment} />
                </div>
              ) : null}
            </div>
          )
        })}
      </div>
    </section>
  )
}
