import Link from "next/link"
import { getProfileRunList } from "@/lib/profile/api"
import type { ProfileFilters } from "@/lib/profile/types"

export async function GroupRunListPanel({
  groupId,
  reportTypeIds,
}: {
  groupId: number
  reportTypeIds: number[]
}) {
  const filters: ProfileFilters = {
    id: groupId,
    type: "group",
    reportType: reportTypeIds.length > 0 ? reportTypeIds : undefined,
  }

  const result = await getProfileRunList(filters)
  const rows = result.data ?? []

  if (rows.length === 0) {
    return <p className="text-sm text-muted-foreground">No report runs to show.</p>
  }

  return (
    <div className="overflow-x-auto rounded-md border">
      <table className="w-full min-w-[640px] text-left text-sm" aria-label="Group report runs">
        <thead className="border-b bg-muted/30 text-muted-foreground">
          <tr>
            <th scope="col" className="px-4 py-3 font-semibold">
              Report
            </th>
            <th scope="col" className="px-4 py-3 font-semibold">
              Type
            </th>
            <th scope="col" className="px-4 py-3 font-semibold">
              Runs
            </th>
            <th scope="col" className="px-4 py-3 font-semibold">
              Last Run
            </th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr
              key={`${row.name}-${row.url ?? ""}-${row.lastRun ?? ""}`}
              className="border-b last:border-b-0"
            >
              <td className="px-4 py-3 font-medium">
                {row.url ? (
                  <Link href={row.url} className="text-link hover:underline">
                    {row.name}
                  </Link>
                ) : (
                  row.name
                )}
              </td>
              <td className="px-4 py-3">{row.type?.trim() || "—"}</td>
              <td className="px-4 py-3">{typeof row.runs === "number" ? row.runs : "—"}</td>
              <td className="px-4 py-3">{row.lastRun?.trim() || "—"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
