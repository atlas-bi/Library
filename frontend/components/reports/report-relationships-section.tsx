import { CollectionSnippetCard } from "@/components/snippets/collection-snippet-card"
import { ReportSnippetCard } from "@/components/snippets/report-snippet-card"
import { ReportRelationshipGroupsTable } from "@/components/reports/report-relationship-groups-table"
import {
  toCollectionListItem,
  toCollectionReportSnippet,
} from "@/lib/reports/report-snippet-adapters"
import type { ReportDetail } from "@/lib/reports/types"

export function ReportRelationshipsSection({ report }: { report: ReportDetail }) {
  const parents = report.parents ?? []
  const children = report.children ?? []
  const groups = report.canViewGroups ? (report.groups ?? []) : []
  const collections = report.collections ?? []

  if (parents.length + children.length + groups.length + collections.length === 0) {
    return null
  }

  return (
    <section id="relationships" className="space-y-6 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Relationships</h2>

      {groups.length > 0 ? <ReportRelationshipGroupsTable groups={groups} /> : null}

      {children.length > 0 ? (
        <div className="space-y-3">
          <h3 className="text-lg font-semibold text-[var(--atlas-home-text-strong)]">
            Reports included in this report.
          </h3>
          <div className="grid gap-4 md:grid-cols-2">
            {children.map((child) => (
              <ReportSnippetCard key={child.id} report={toCollectionReportSnippet(child)} />
            ))}
          </div>
        </div>
      ) : null}

      {parents.length > 0 ? (
        <div className="space-y-3">
          <h3 className="text-lg font-semibold text-[var(--atlas-home-text-strong)]">
            Reports that include this report.
          </h3>
          <div className="grid gap-4 md:grid-cols-2">
            {parents.map((parent) => (
              <ReportSnippetCard key={parent.id} report={toCollectionReportSnippet(parent)} />
            ))}
          </div>
        </div>
      ) : null}

      {collections.length > 0 ? (
        <div className="space-y-3">
          <h3 id="collections" className="text-lg font-semibold text-[var(--atlas-home-text-strong)]">
            Linked Collections
          </h3>
          <div className="grid gap-4 md:grid-cols-2">
            {collections.map((collection) => (
              <CollectionSnippetCard key={collection.id} collection={toCollectionListItem(collection)} />
            ))}
          </div>
        </div>
      ) : null}
    </section>
  )
}
