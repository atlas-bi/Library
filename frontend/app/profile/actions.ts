"use server";

import {
	getProfileChart,
	getProfileFails,
	getProfileReports,
	getProfileRunList,
	getProfileStars,
	getProfileSubscriptions,
	getProfileUsers,
} from "@/lib/profile/api";
import type {
	ProfileBarItemDto,
	ProfileChartResponseDto,
	ProfileFilters,
	ProfileRunListItemDto,
	ProfileStarUserDto,
	ProfileSubscriptionDto,
} from "@/lib/profile/types";

export type ProfileAnalyticsData = {
	chart: ProfileChartResponseDto | null;
	users: ProfileBarItemDto[];
	reports: ProfileBarItemDto[];
	fails: ProfileBarItemDto[];
	runList: ProfileRunListItemDto[];
	stars: ProfileStarUserDto[];
	subscriptions: ProfileSubscriptionDto[];
};

export async function loadProfileAnalyticsAction(
	id: number,
	type: string,
	options?: Partial<Omit<ProfileFilters, "id" | "type">>,
): Promise<{ data: ProfileAnalyticsData | null; error: string | null }> {
	const filters: ProfileFilters = { id, type, ...options };
	const canLoadProfileRelationships = type !== "user";
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
	]);

	const allResults = [
		chartResult,
		usersResult,
		reportsResult,
		failsResult,
		runListResult,
		starsResult,
		subsResult,
	];

	const firstError = allResults.find((r) => r.error !== null)?.error;
	if (firstError) {
		return { data: null, error: firstError };
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
	};
}
