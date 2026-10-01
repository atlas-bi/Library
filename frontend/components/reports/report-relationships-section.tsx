import Link from "next/link"
import { CollectionSnippetCard } from "@/components/snippets/collection-snippet-card"
import { ReportSnippetCard } from "@/components/snippets/report-snippet-card"
import type { ReportDetail } from "@/lib/reports/types"

function toReportSnippet(
  item: NonNullable<ReportDetail["parents"]>[number],
): Parameters<typeof ReportSnippetCard>[0]["report"] {
  return {
    id: item.id,
    name: item.displayTitle ?? item.name,
    attachmentCount: item.attachmentCount ?? 0,
  }
}

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

      {groups.length > 0 ? (
        <div className="space-y-3">
          <h3 className="text-lg font-semibold">Report Groups</h3>
          <div className="overflow-x-auto rounded-md border border-[var(--atlas-home-border-soft)]">
            <table className="min-w-full text-sm">
              <thead>
                <tr className="border-b border-[var(--atlas-home-border-soft)] bg-muted/30">
                  <th className="px-3 py-2 text-left font-medium">Group Name</th>
                  <th className="px-3 py-2 text-left font-medium">Type</th>
                </tr>
              </thead>
              <tbody>
                {groups.map((group) => (
                  <tr
                    key={group.id}
                    className="border-b border-[var(--atlas-home-border-soft)] last:border-b-0"
                  >
                    <td className="px-3 py-2">
                      <Link href={`/groups?id=${group.id}`} className="text-link hover:underline">
                        {group.name ?? group.email ?? `Group ${group.id}`}
                      </Link>
                    </td>
                    <td className="px-3 py-2 text-muted-foreground">{group.type ?? "—"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ) : null}

      {children.length > 0 ? (
        <div className="space-y-3">
          <h3 className="text-lg font-semibold">Reports included in this report</h3>
          <div className="grid gap-4 md:grid-cols-2">
            {children.map((child) => (
              <ReportSnippetCard
                key={child.id}
                report={toReportSnippet(child)}
                features={report.features}
              />
            ))}
          </div>
        </div>
      ) : null}

      {parents.length > 0 ? (
        <div className="space-y-3">
          <h3 className="text-lg font-semibold">Reports that include this report</h3>
          <div className="grid gap-4 md:grid-cols-2">
            {parents.map((parent) => (
              <ReportSnippetCard
                key={parent.id}
                report={toReportSnippet(parent)}
                features={report.features}
              />
            ))}
          </div>
        </div>
      ) : null}

      {collections.length > 0 ? (
        <div className="space-y-3">
          <h3 id="collections" className="text-lg font-semibold scroll-mt-24">
            Linked Collections
          </h3>
          <div className="grid gap-4 md:grid-cols-2">
            {collections.map((collection) => (
              <CollectionSnippetCard
                key={collection.id}
                collection={{
                  id: collection.id,
                  name: collection.name ?? `Collection ${collection.id}`,
                }}
              />
            ))}
          </div>
        </div>
      ) : null}
    </section>
  )
}
