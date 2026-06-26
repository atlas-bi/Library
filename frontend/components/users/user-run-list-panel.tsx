import { ProfileRunListTable } from "@/components/profile/profile-run-list-table"
import { getProfileRunList } from "@/lib/profile/api"

export async function UserRunListPanel({
  userId,
  reportTypeIds,
}: {
  userId: number
  reportTypeIds: number[]
}) {
  const result = await getProfileRunList({
    id: userId,
    type: "user",
    reportType: reportTypeIds.length > 0 ? reportTypeIds : undefined,
  })

  return <ProfileRunListTable rows={result.data ?? []} />
}
