'use server';

import {
    getProfileChart,
    getProfileFails,
    getProfileFilters,
    getProfileReports,
    getProfileRunList,
    getProfileStars,
    getProfileSubscriptions,
    getProfileUsers,
} from '@/lib/profile/api';
import type {
    ProfileBarItemDto,
    ProfileChartResponseDto,
    ProfileFilters,
    ProfileFiltersResponseDto,
    ProfileRunListItemDto,
    ProfileStarUserDto,
    ProfileSubscriptionDto,
} from '@/lib/profile/types';

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
    options?: Partial<Omit<ProfileFilters, 'id' | 'type'>>,
): Promise<{ data: ProfileAnalyticsData | null; error: string | null }> {
    const filters: ProfileFilters = { id, type, ...options };
    const canLoadProfileRelationships = type !== 'user' && id !== -1;
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
        type === 'report'
            ? getProfileRunList(filters)
            : Promise.resolve({ data: [], error: null }),
        canLoadProfileRelationships
            ? getProfileStars(filters)
            : Promise.resolve({ data: [], error: null }),
        type === 'report' && id !== -1
            ? getProfileSubscriptions(filters)
            : Promise.resolve({ data: [], error: null }),
    ]);

    const authError = [
        chartResult,
        usersResult,
        reportsResult,
        failsResult,
        runListResult,
        starsResult,
        subsResult,
    ].find((result) => result.error === 'auth_required')?.error;

    if (authError) {
        return { data: null, error: authError };
    }

    const data: ProfileAnalyticsData = {
        chart: chartResult.data ?? {
            runs: 0,
            users: 0,
            runTime: 0,
            history: [],
        },
        users: usersResult.error ? [] : (usersResult.data ?? []),
        reports: reportsResult.error ? [] : (reportsResult.data ?? []),
        fails: failsResult.error ? [] : (failsResult.data ?? []),
        runList: runListResult.error ? [] : (runListResult.data ?? []),
        stars: starsResult.error ? [] : (starsResult.data ?? []),
        subscriptions: subsResult.error ? [] : (subsResult.data ?? []),
    };

    const hasRenderableData =
        chartResult.data !== null ||
        data.users.length > 0 ||
        data.reports.length > 0 ||
        data.fails.length > 0 ||
        data.runList.length > 0 ||
        data.stars.length > 0 ||
        data.subscriptions.length > 0 ||
        type === 'term' ||
        type === 'collection';

    if (!hasRenderableData) {
        const firstError =
            chartResult.error ??
            usersResult.error ??
            reportsResult.error ??
            failsResult.error ??
            runListResult.error ??
            starsResult.error ??
            subsResult.error;

        if (firstError) {
            return { data: null, error: firstError };
        }
    }

    return { data, error: null };
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
    options?: Partial<Omit<ProfileFilters, 'id' | 'type'>>,
): Promise<ProfileFiltersData | null> {
    const filters: ProfileFilters = { id, type, ...options };
    const result = await getProfileFilters(filters);
    // Return null on any error (404 = endpoint not yet deployed, etc.)
    if (result.error) return null;
    return result.data;
}
