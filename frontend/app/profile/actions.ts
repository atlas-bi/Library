"use server"

import {
  getProfileChart,
  getProfileFails,
  getProfileReports,
  getProfileRunList,
  getProfileStars,
  getProfileSubscriptions,
  getProfileUsers,
} from "@/lib/profile/api"
import type {
  ProfileBarItemDto,
  ProfileChartResponseDto,
  ProfileFilters,
  ProfileRunListItemDto,
  ProfileStarUserDto,
  ProfileSubscriptionDto,
} from "@/lib/profile/types"

export type ProfileAnalyticsData = {
  chart: ProfileChartResponseDto | null
  users: ProfileBarItemDto[]
  reports: ProfileBarItemDto[]
  fails: ProfileBarItemDto[]
  runList: ProfileRunListItemDto[]
  stars: ProfileStarUserDto[]
  subscriptions: ProfileSubscriptionDto[]
}

export async function loadProfileAnalyticsAction(
  id: number,
  type: string,
  options?: Partial<Omit<ProfileFilters, "id" | "type">>,
): Promise<{ data: ProfileAnalyticsData | null; error: string | null }> {
  const filters: ProfileFilters = { id, type, ...options }
  const canLoadProfileRelationships = type !== "user"
  const [
    chartResult,
    usersResult,
    reportsResult,
    failsResult,
    runListResult,
    starsResult,
    subsResult,
  ] = await Promise.all([
    getProfileChart(filters),
    getProfileUsers(filters),
    getProfileReports(filters),
    getProfileFails(filters),
    getProfileRunList(filters),
    canLoadProfileRelationships
      ? getProfileStars(filters)
      : Promise.resolve({ data: [], error: null }),
    canLoadProfileRelationships
      ? getProfileSubscriptions(filters)
      : Promise.resolve({ data: [], error: null }),
  ])

  if (
    chartResult.error === "auth_required" ||
    usersResult.error === "auth_required" ||
    reportsResult.error === "auth_required"
  ) {
    return { data: null, error: "auth_required" }
  }

  return {
    data: {
      chart: chartResult.data,
      users: usersResult.data ?? [],
      reports: reportsResult.data ?? [],
      fails: failsResult.data ?? [],
      runList: runListResult.data ?? [],
      stars: starsResult.data ?? [],
      subscriptions: subsResult.data ?? [],
    },
    error: null,
  }
}
