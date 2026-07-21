import type { GroupDetail } from "@/lib/groups/types"

export function GroupDetailsTable({ group }: { group: GroupDetail }) {
  const rows: Array<{ label: string; value: string }> = []

  if (group.email?.trim()) {
    rows.push({ label: "Email", value: group.email.trim() })
  }
  rows.push({ label: "Type", value: group.type?.trim() || "—" })
  rows.push({ label: "Source", value: group.source?.trim() || "—" })

  return (
    <div className="overflow-x-auto">
      <table className="w-full max-w-2xl text-left text-sm" aria-label="Group details">
        <tbody>
          {rows.map((row) => (
            <tr key={row.label} className="border-b border-border/60">
              <th scope="row" className="w-40 py-2 pr-4 font-medium text-muted-foreground">
                {row.label}
              </th>
              <td className="py-2">
                {row.label === "Email" && row.value.includes("@") ? (
                  <a href={`mailto:${row.value}`} className="text-link hover:underline">
                    {row.value}
                  </a>
                ) : (
                  row.value
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
