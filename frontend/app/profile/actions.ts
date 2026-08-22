"use server";

import {
	getProfileChart,
	getProfileFails,
	getProfileFilters,
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
	ProfileFiltersResponseDto,
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
	const canLoadProfileRelationships = type !== "user" && id !== -1;
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
		type === "report"
			? getProfileRunList(filters)
			: Promise.resolve({ data: [], error: null }),
		canLoadProfileRelationships
			? getProfileStars(filters)
			: Promise.resolve({ data: [], error: null }),
		type === "report" && id !== -1
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

export type ProfileFiltersData = ProfileFiltersResponseDto;

/**
 * Loads sidebar filter options (available server names, databases, etc.) from
 * the backend. Returns null silently when the endpoint is unavailable so the
 * sidebar degrades gracefully to free-text TagInputs.
 */
export async function loadProfileFiltersAction(
	id: number,
	type: string,
	options?: Partial<Omit<ProfileFilters, "id" | "type">>,
): Promise<ProfileFiltersData | null> {
	const filters: ProfileFilters = { id, type, ...options };
	const result = await getProfileFilters(filters);
	// Return null on any error (404 = endpoint not yet deployed, etc.)
	if (result.error) return null;
	return result.data;
}
