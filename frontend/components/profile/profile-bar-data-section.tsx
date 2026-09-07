import Link from "next/link"
import type { ProfileBarItemDto } from "@/lib/profile/types"

function getBarTitle(item: ProfileBarItemDto) {
  return item.title ?? item.key
}

function getBarSubtitle(item: ProfileBarItemDto) {
  return item.titleTwo ?? item.dateTitle ?? item.date ?? item.dateLabel ?? null
}

export function ProfileBarDataSection({
  title,
  items,
}: {
  title: string
  items: ProfileBarItemDto[]
}) {
  if (items.length === 0) return null

  const maxCount = Math.max(...items.map((item) => item.count), 1)

  return (
    <div className="space-y-3 rounded-md border bg-card/40 p-4">
      <h3 className="text-sm font-semibold">{title}</h3>
      <ul className="space-y-3">
        {items.map((item) => {
          const width = Math.max(4, Math.round((item.count / maxCount) * 100))
          const label = getBarTitle(item)
          const subtitle = getBarSubtitle(item)

          return (
            <li key={item.key} className="space-y-1">
              <div className="flex items-start justify-between gap-3 text-sm">
                <div className="min-w-0">
                  {item.href ? (
                    <Link href={item.href} className="font-medium text-link hover:underline">
                      {label}
                    </Link>
                  ) : (
                    <span className="font-medium">{label}</span>
                  )}
                  {subtitle ? (
                    <div className="truncate text-xs text-muted-foreground">{subtitle}</div>
                  ) : null}
                </div>
                <div className="shrink-0 text-right text-xs text-muted-foreground">
                  <div>{item.count.toLocaleString()} runs</div>
                  {typeof item.percent === "number" ? (
                    <div>
                      {Intl.NumberFormat("en-US", {
                        style: "percent",
                        maximumFractionDigits: 0,
                      }).format(item.percent)}
                    </div>
                  ) : null}
                </div>
              </div>
              <div className="h-2 overflow-hidden rounded-full bg-muted">
                <div
                  className="h-full rounded-full bg-[rgba(38,128,235,0.55)]"
                  style={{ width: `${width}%` }}
                />
              </div>
            </li>
          )
        })}
      </ul>
    </div>
  )
}

export function ProfileBarDataGrid({
  type,
  id,
  users,
  reports,
  fails,
}: {
  type: string
  id: number
  users: ProfileBarItemDto[]
  reports: ProfileBarItemDto[]
  fails: ProfileBarItemDto[]
}) {
  const showUsers = type !== "user"
  const showReports =
    type === "user" || type === "term" || type === "collection" || (type === "report" && id === -1)

  const columnClass =
    type === "term" || type === "collection" || (type === "report" && id === -1)
      ? "md:grid-cols-3"
      : "md:grid-cols-2"

  return (
    <div className={`grid gap-4 ${columnClass}`}>
      {showUsers ? <ProfileBarDataSection title="Top users" items={users} /> : null}
      {showReports ? <ProfileBarDataSection title="Top reports" items={reports} /> : null}
      <ProfileBarDataSection title="Failures" items={fails} />
    </div>
  )
}
