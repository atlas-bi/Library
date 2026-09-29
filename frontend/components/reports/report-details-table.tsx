import Link from "next/link"
import type { ReactNode } from "react"
import type { ReportDetail } from "@/lib/reports/types"

function PersonLink({
  person,
  canLink,
}: {
  person?: { id: number; fullName?: string; username?: string } | null
  canLink?: boolean
}) {
  if (!person) return null
  const label = person.fullName?.trim() || person.username?.trim() || `User ${person.id}`
  if (canLink) {
    return (
      <Link href={`/users?id=${person.id}`} className="text-link hover:underline">
        {label}
      </Link>
    )
  }
  return <span>{label}</span>
}

function DetailRow({ label, value }: { label: string; value: ReactNode }) {
  if (value == null || value === "") return null
  return (
    <tr className="border-b border-[var(--atlas-home-border-soft)]">
      <th className="w-56 px-3 py-2 text-left font-medium text-[var(--atlas-home-muted)]">{label}</th>
      <td className="px-3 py-2 text-sm">{value}</td>
    </tr>
  )
}

function ynYes(value?: string | null) {
  return value?.toUpperCase() === "Y" ? "Yes" : null
}

function ynLabel(value?: string | null) {
  if (!value) return null
  return value.toUpperCase() === "Y" ? "Yes" : "No"
}

function isSsrsReport(report: ReportDetail) {
  const typeName = report.typeName?.toLowerCase() ?? ""
  const typeShortName = report.typeShortName?.toLowerCase() ?? ""
  return typeName.includes("ssrs") || typeShortName.includes("ssrs")
}

export function ReportDetailsTable({
  report,
  canViewUserProfiles,
}: {
  report: ReportDetail
  canViewUserProfiles?: boolean
}) {
  const doc = report.document
  const visibleInSearch =
    report.visibleInSearch === undefined ? undefined : report.visibleInSearch ? "Yes" : "No"
  const fragilityTagNames =
    doc?.fragilityTags?.map((tag) => tag.name).filter(Boolean).join(", ") ?? null

  return (
    <section id="details" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Details</h2>
      <div className="overflow-x-auto rounded-md border border-[var(--atlas-home-border-soft)]">
        <table className="min-w-full text-sm">
          <tbody>
            <DetailRow
              label="System Name"
              value={
                report.displayTitle && report.name !== report.displayTitle ? report.name : null
              }
            />
            <DetailRow label="Report Type" value={report.typeName ?? report.typeShortName} />
            <DetailRow
              label="ETL Load Date"
              value={
                report.lastLoadDate
                  ? new Date(report.lastLoadDate).toLocaleDateString()
                  : null
              }
            />
            <DetailRow
              label="Last Modified"
              value={
                report.lastModified ? (
                  <span>
                    {new Date(report.lastModified).toLocaleString()}
                    {report.lastModifiedBy ? (
                      <>
                        {" · "}
                        <PersonLink person={report.lastModifiedBy} canLink={canViewUserProfiles} />
                      </>
                    ) : null}
                  </span>
                ) : null
              }
            />
            <DetailRow
              label="Documentation Last Modified"
              value={
                doc?.lastUpdateDateTime ? (
                  <span>
                    {new Date(doc.lastUpdateDateTime).toLocaleString()}
                    {doc.updatedBy ? (
                      <>
                        {" · "}
                        <PersonLink person={doc.updatedBy} canLink={canViewUserProfiles} />
                      </>
                    ) : null}
                  </span>
                ) : null
              }
            />
            <DetailRow
              label="Report Author"
              value={<PersonLink person={report.author} canLink={canViewUserProfiles} />}
            />
            <DetailRow
              label="Report Requester"
              value={<PersonLink person={doc?.requester ?? report.requester} canLink={canViewUserProfiles} />}
            />
            <DetailRow
              label="Operational Owner"
              value={<PersonLink person={doc?.operationalOwner} canLink={canViewUserProfiles} />}
            />
            <DetailRow label="Availability" value={report.availability} />
            <DetailRow label="Visible in Search" value={visibleInSearch} />
            <DetailRow
              label="GitLab Project"
              value={
                doc?.gitLabProjectUrl ? (
                  <a
                    href={doc.gitLabProjectUrl}
                    target="_blank"
                    rel="noreferrer"
                    className="text-link hover:underline"
                  >
                    View Source
                  </a>
                ) : null
              }
            />
            <DetailRow
              label="Epic Identifier"
              value={
                report.epicMasterFile ? (
                  <span>
                    {report.epicMasterFile}
                    {report.epicRecordId != null ? ` ${report.epicRecordId}` : ""}
                    {report.epicReportTemplateId != null
                      ? ` (Template ${report.epicReportTemplateId})`
                      : ""}
                  </span>
                ) : null
              }
            />
            <DetailRow label="Atlas Id" value={String(report.id)} />
            <DetailRow label="Runs" value={typeof report.runs === "number" ? String(report.runs) : null} />
            <DetailRow label="Orphaned?" value={ynYes(report.orphanedReportObjectYn)} />
            <DetailRow label="Executive Visibility" value={ynYes(doc?.executiveVisibilityYn)} />
            <DetailRow
              label="Enabled for Hyperspace"
              value={
                isSsrsReport(report) && doc?.enabledForHyperspace
                  ? ynLabel(doc.enabledForHyperspace)
                  : null
              }
            />
            <DetailRow label="Do Not Purge" value={ynYes(doc?.doNotPurge)} />
            <DetailRow label="Hidden?" value={ynYes(doc?.hidden)} />
            <DetailRow label="Estimated Run Frequency" value={doc?.estimatedRunFrequency?.name} />
            <DetailRow label="Fragility Rating" value={doc?.fragility?.name} />
            <DetailRow label="Maintenance Schedule" value={doc?.maintenanceSchedule?.name} />
            <DetailRow label="Organizational Value Rating" value={doc?.organizationalValue?.name} />
            <DetailRow label="Fragility Tags" value={fragilityTagNames} />
            <DetailRow
              label="Report Tags"
              value={
                report.objectTags && report.objectTags.length > 0
                  ? report.objectTags.map((tag) => tag.name).filter(Boolean).join(", ")
                  : null
              }
            />
          </tbody>
        </table>
      </div>
    </section>
  )
}
