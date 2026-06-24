import Link from "next/link"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import type { UserSubscription } from "@/lib/users/types"

function formatStatus(value?: string | null) {
  return value?.trim() || "—"
}

export function UserSubscriptionsTable({ rows }: { rows: UserSubscription[] }) {
  if (rows.length === 0) {
    return <p className="text-sm text-muted-foreground">No subscriptions to show.</p>
  }

  return (
    <Table aria-label="User subscriptions">
      <TableHeader>
        <TableRow>
          <TableHead>Report</TableHead>
          <TableHead>Subscription Description</TableHead>
          <TableHead>Last Run</TableHead>
          <TableHead>Message</TableHead>
          <TableHead>Subscribed As</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {rows.map((subscription, index) => (
          <TableRow key={`${subscription.reportId ?? subscription.name ?? index}`}>
            <TableCell className="font-medium">
              {subscription.reportId ? (
                <Link
                  href={`/reports?id=${subscription.reportId}`}
                  className="text-link hover:underline"
                >
                  {subscription.name?.trim() || `Report ${subscription.reportId}`}
                </Link>
              ) : (
                subscription.name?.trim() || "—"
              )}
            </TableCell>
            <TableCell>{formatStatus(subscription.description)}</TableCell>
            <TableCell>{formatStatus(subscription.lastRun)}</TableCell>
            <TableCell>{formatStatus(subscription.lastStatus)}</TableCell>
            <TableCell>{formatStatus(subscription.sentTo)}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
