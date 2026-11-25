"use client"

import { HomeSharedWithMeRail } from "@/components/home/home-shared-with-me-rail"
import { UserStarsWorkspace } from "@/components/users/user-stars-workspace"
import type { HomeSharedWithMeItem } from "@/lib/home/types"
import type { UserStars } from "@/lib/users/types"

type HomeStarsWorkspacePanelProps = {
  stars: UserStars
  sharedWithMe?: HomeSharedWithMeItem[]
}

export function HomeStarsWorkspacePanel({ stars, sharedWithMe = [] }: HomeStarsWorkspacePanelProps) {
  const showSharedRail = sharedWithMe.length > 0

  return (
    <section className={showSharedRail ? "grid gap-5 xl:grid-cols-[minmax(0,1fr)_220px]" : undefined}>
      <UserStarsWorkspace stars={stars} />
      {showSharedRail ? <HomeSharedWithMeRail items={sharedWithMe} /> : null}
    </section>
  )
}