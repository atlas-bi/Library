"use client"

import { ProfileFullView } from "@/components/profile/profile-full-view"

export function ProfileAnalyticsPanelClient({
  id,
  type,
  userProfilesEnabled = true,
}: {
  id: number
  type: string
  userProfilesEnabled?: boolean
}) {
  return <ProfileFullView id={id} type={type} userProfilesEnabled={userProfilesEnabled} />
}
