import Link from "next/link"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import type { ProfileSubscriptionDto } from "@/lib/profile/types"

function formatStatus(value?: string | null) {
  return value?.replace(/;/g, "; ") ?? "—"
}

function formatLastRun(value?: string | null) {
  return value?.trim() || "—"
}

export function ProfileSubscriptionsTable({
  rows,
  userProfilesEnabled = true,
}: {
  rows: ProfileSubscriptionDto[]
  userProfilesEnabled?: boolean
}) {
  if (rows.length === 0) {
    return <p className="text-sm font-medium">There are no subscriptions.</p>
  }

  return (
    <Table aria-label="Profile subscriptions">
      <TableHeader>
        <TableRow>
          <TableHead>User</TableHead>
          <TableHead>Subscription Description</TableHead>
          <TableHead>Last Run</TableHead>
          <TableHead>Message</TableHead>
          <TableHead>Subscribed As</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {rows.map((subscription) => (
          <TableRow key={subscription.id}>
            <TableCell>
              {subscription.userId && subscription.userName && userProfilesEnabled ? (
                <Link
                  href={`/users?id=${subscription.userId}`}
                  className="text-link hover:underline"
                >
                  {subscription.userName}
                </Link>
              ) : (
                (subscription.userName ?? "N/A")
              )}
            </TableCell>
            <TableCell>{subscription.description?.trim() || "—"}</TableCell>
            <TableCell>{formatLastRun(subscription.lastRunTime)}</TableCell>
            <TableCell>{formatStatus(subscription.lastStatus)}</TableCell>
            <TableCell>{formatStatus(subscription.subscriptionTo)}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
