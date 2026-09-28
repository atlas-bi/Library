import Link from "next/link"
import { MarkdownContent } from "@/components/content/markdown-content"
import type {
  TaskAnalyticsReportDto,
  TaskCanMakeReportDto,
  TaskMaintenanceReportDto,
  TaskRetireReportDto,
  TaskUndocumentedReportDto,
  TaskUnusedReportDto,
} from "@/lib/tasks/types"

function TasksTable({
  ariaLabel,
  children,
}: {
  ariaLabel: string
  children: React.ReactNode
}) {
  return (
    <div className="atlas-home-card overflow-x-auto">
      <table className="atlas-home-table min-w-full" aria-label={ariaLabel}>
        {children}
      </table>
    </div>
  )
}

function isPastDue(dateText: string): boolean {
  const match = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/.exec(dateText.trim())
  if (!match) return false
  const month = Number(match[1])
  const day = Number(match[2])
  const year = Number(match[3])
  const due = new Date(year, month - 1, day)
  if (Number.isNaN(due.getTime())) return false
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return due < today
}

function MaintenanceList({ items, hint }: { items: TaskMaintenanceReportDto[]; hint: string }) {
  return (
    <div className="space-y-3">
      <p className="text-sm text-[var(--atlas-home-muted)]">{hint}</p>
      {items.length === 0 ? (
        <p className="text-sm text-[var(--atlas-home-muted)]">No reports to show.</p>
      ) : (
        <ul className="space-y-2">
          {items.map((item) => (
            <li key={item.reportId} className="text-sm">
              <Link
                href={`/reports?id=${item.reportId}`}
                className="font-medium text-[var(--atlas-home-link)] hover:underline"
              >
                {item.name}
              </Link>
              <span className="text-[var(--atlas-home-muted)]">
                {" "}
                ·{" "}
                {isPastDue(item.date) ? (
                  <span className="text-red-600">Due on {item.date}.</span>
                ) : (
                  <>Due on {item.date}.</>
                )}{" "}
                Last updated/maintained by {item.user}
              </span>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export function RecommendRetirePanel({ items }: { items: TaskRetireReportDto[] }) {
  return (
    <div className="space-y-3">
      <p className="text-sm text-[var(--atlas-home-muted)]">
        Reports with red outline have not been reviewed in the last 6 months.
      </p>
      {items.length === 0 ? (
        <p className="text-sm text-[var(--atlas-home-muted)]">No reports to show.</p>
      ) : (
        <ul className="space-y-4">
          {items.map((item) => (
            <li key={item.reportId}>
              <Link
                href={`/reports?id=${item.reportId}`}
                className="font-medium text-[var(--atlas-home-link)] hover:underline"
              >
                {item.name}
              </Link>
              <span className="text-sm text-[var(--atlas-home-muted)]">
                {" "}
                · Recommended by {item.fullName} on {item.maintenanceDateString}
              </span>
              {item.comment ? (
                <div className="mt-2 text-sm">
                  <MarkdownContent content={item.comment} />
                </div>
              ) : null}
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export function UnusedPanel({ items }: { items: TaskUnusedReportDto[] }) {
  return (
    <div className="space-y-3">
      <p className="text-sm text-[var(--atlas-home-muted)]">
        Reports with no run data collected for last two years & edited over 2 months ago.
      </p>
      {items.length === 0 ? (
        <p className="text-sm text-[var(--atlas-home-muted)]">No reports to show.</p>
      ) : (
        <TasksTable ariaLabel="unused reports">
          <thead className="bg-transparent text-[var(--atlas-home-title)]">
            <tr>
              <th className="px-4 py-3 font-semibold">Report Name</th>
              <th className="px-4 py-3 font-semibold">Report Type</th>
              <th className="px-4 py-3 font-semibold">Last Modified By</th>
              <th className="px-4 py-3 font-semibold">Last Modified</th>
              <th className="px-4 py-3 font-semibold">Server</th>
              <th className="px-4 py-3 font-semibold">Master File</th>
              <th className="px-4 py-3 font-semibold">Epic File</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.reportUrl}>
                <td className="px-4 py-3 font-semibold">
                  <Link href={item.reportUrl} className="hover:underline">
                    {item.name}
                  </Link>
                </td>
                <td className="px-4 py-3">{item.type}</td>
                <td className="px-4 py-3">{item.modifiedBy}</td>
                <td className="px-4 py-3">{item.lastModified}</td>
                <td className="px-4 py-3">{item.server}</td>
                <td className="px-4 py-3">{item.masterFile}</td>
                <td className="px-4 py-3">{item.epicId}</td>
              </tr>
            ))}
          </tbody>
        </TasksTable>
      )}
    </div>
  )
}

export function MaintenanceRequiredPanel({ items }: { items: TaskMaintenanceReportDto[] }) {
  return (
    <MaintenanceList
      items={items}
      hint="Reports with red outline have maintenance that is past due."
    />
  )
}

export function AuditPanel({ items }: { items: TaskMaintenanceReportDto[] }) {
  return (
    <MaintenanceList
      items={items}
      hint="Reports on an audit-only maintenance schedule that are due within two months."
    />
  )
}

export function MissingSchedulePanel({ items }: { items: TaskMaintenanceReportDto[] }) {
  return (
    <MaintenanceList
      items={items}
      hint="Visible reports with no maintenance schedule and a due date within two months."
    />
  )
}

export function NotInAnalyticsPanel({ items }: { items: TaskAnalyticsReportDto[] }) {
  return (
    <div className="space-y-3">
      <p className="text-sm text-[var(--atlas-home-muted)]">
        Reports edited outside of Analytics in last 6 months.
      </p>
      <TasksTable ariaLabel="edited outside analytics">
        <thead className="bg-transparent text-[var(--atlas-home-title)]">
          <tr>
            <th className="px-4 py-3 font-semibold">Report Name</th>
            <th className="px-4 py-3 font-semibold">Report Type</th>
            <th className="px-4 py-3 font-semibold">Author</th>
            <th className="px-4 py-3 font-semibold">Last Modified By</th>
            <th className="px-4 py-3 font-semibold">Last Modified</th>
            <th className="px-4 py-3 font-semibold">Runs</th>
            <th className="px-4 py-3 font-semibold">RecordViewer</th>
            <th className="px-4 py-3 font-semibold">Editor</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.reportUrl}>
              <td className="px-4 py-3 font-semibold">
                <Link href={item.reportUrl} className="hover:underline">
                  {item.name}
                </Link>
              </td>
              <td className="px-4 py-3">{item.reportType}</td>
              <td className="px-4 py-3">{item.author}</td>
              <td className="px-4 py-3">{item.modifiedBy}</td>
              <td className="px-4 py-3">{item.lastModified}</td>
              <td className="px-4 py-3">{item.runs}</td>
              <td className="px-4 py-3">
                {item.recordViewerUrl ? (
                  <a href={item.recordViewerUrl} className="text-[var(--atlas-home-link)] hover:underline">
                    {item.epic}
                  </a>
                ) : (
                  item.epic
                )}
              </td>
              <td className="px-4 py-3">
                {item.editReportUrl ? (
                  <a href={item.editReportUrl} className="text-[var(--atlas-home-link)] hover:underline">
                    Edit
                  </a>
                ) : (
                  "—"
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </TasksTable>
    </div>
  )
}

function UndocumentedTable({
  items,
  hint,
}: {
  items: TaskUndocumentedReportDto[]
  hint: string
}) {
  return (
    <div className="space-y-3">
      <p className="text-sm text-[var(--atlas-home-muted)]">{hint}</p>
      <TasksTable ariaLabel="undocumented reports">
        <thead className="bg-transparent text-[var(--atlas-home-title)]">
          <tr>
            <th className="px-4 py-3 font-semibold">Report Name</th>
            <th className="px-4 py-3 font-semibold">Report Type</th>
            <th className="px-4 py-3 font-semibold">Last Modified By</th>
            <th className="px-4 py-3 font-semibold">Runs</th>
            <th className="px-4 py-3 font-semibold">Favorite?</th>
            <th className="px-4 py-3 font-semibold">Last Modified</th>
            <th className="px-4 py-3 font-semibold">Last Run</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.reportObjectId}>
              <td className="px-4 py-3 font-semibold">
                <Link href={`/reports?id=${item.reportObjectId}`} className="hover:underline">
                  {item.name}
                </Link>
              </td>
              <td className="px-4 py-3">{item.reportType}</td>
              <td className="px-4 py-3">{item.modifiedBy}</td>
              <td className="px-4 py-3">{item.runs}</td>
              <td className="px-4 py-3">{item.favorite || "—"}</td>
              <td className="px-4 py-3">{item.lastMaintained}</td>
              <td className="px-4 py-3">{item.lastRun}</td>
            </tr>
          ))}
        </tbody>
      </TasksTable>
    </div>
  )
}

export function TopUndocumentedPanel({ items }: { items: TaskUndocumentedReportDto[] }) {
  return (
    <UndocumentedTable
      items={items}
      hint="Top 60 Undocumented Reports (Workbench, SSRS, Dashboard, and Crystal)"
    />
  )
}

export function NewUndocumentedPanel({ items }: { items: TaskUndocumentedReportDto[] }) {
  return (
    <UndocumentedTable
      items={items}
      hint="Undocumented reports modified in the last month."
    />
  )
}

export function CanMakeReportsPanel({
  items,
  canViewGroups,
}: {
  items: TaskCanMakeReportDto[]
  canViewGroups: boolean
}) {
  return (
    <div className="space-y-3">
      <p className="text-sm text-[var(--atlas-home-muted)]">
        Users that can create reports in Prod Hyperspace
      </p>
      <TasksTable ariaLabel="prod access users">
        <thead className="bg-transparent text-[var(--atlas-home-title)]">
          <tr>
            <th className="px-4 py-3 font-semibold">User</th>
            <th className="px-4 py-3 font-semibold">Role Granting Access</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={`${item.userId ?? item.name}-${item.roleId ?? item.role}`}>
              <td className="px-4 py-3 font-semibold">
                {item.userId ? (
                  <Link href={`/users?id=${item.userId}`} className="hover:underline">
                    {item.name}
                  </Link>
                ) : (
                  item.name
                )}
              </td>
              <td className="px-4 py-3">
                {canViewGroups && item.roleId ? (
                  <Link href={`/groups?id=${item.roleId}`} className="hover:underline">
                    {item.role}
                  </Link>
                ) : (
                  item.role
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </TasksTable>
    </div>
  )
}
