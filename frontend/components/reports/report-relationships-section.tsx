import Link from "next/link"
import type { ReportDetail } from "@/lib/reports/types"

function RelationshipList({
  title,
  items,
}: {
  title: string
  items: Array<{ id?: number; name?: string | null; displayTitle?: string | null; type?: string | null }>
}) {
  if (items.length === 0) return null

  return (
    <div className="space-y-2">
      <h3 className="text-base font-semibold">{title}</h3>
      <ul className="space-y-1 text-sm">
        {items.map((item) => (
          <li key={`${title}-${item.id}`}>
            {item.id ? (
              <Link href={`/reports?id=${item.id}`} className="text-link hover:underline">
                {item.displayTitle ?? item.name ?? `Report ${item.id}`}
              </Link>
            ) : (
              item.name
            )}
            {item.type ? <span className="text-muted-foreground"> · {item.type}</span> : null}
          </li>
        ))}
      </ul>
    </div>
  )
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
    <section id="relationships" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Relationships</h2>
      <div className="space-y-5 rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4">
        <RelationshipList title="Parents" items={parents} />
        <RelationshipList title="Children" items={children} />
        {groups.length > 0 ? (
          <div className="space-y-2">
            <h3 className="text-base font-semibold">Groups</h3>
            <ul className="space-y-1 text-sm">
              {groups.map((group) => (
                <li key={group.id}>
                  <Link href={`/groups?id=${group.id}`} className="text-link hover:underline">
                    {group.name ?? group.email ?? `Group ${group.id}`}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        ) : null}
        {collections.length > 0 ? (
          <div className="space-y-2">
            <h3 className="text-base font-semibold">Collections</h3>
            <ul className="space-y-1 text-sm">
              {collections.map((collection) => (
                <li key={collection.id}>
                  <Link href={`/collections?id=${collection.id}`} className="text-link hover:underline">
                    {collection.name ?? `Collection ${collection.id}`}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        ) : null}
      </div>
    </section>
  )
}
