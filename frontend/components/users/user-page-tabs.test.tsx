import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { UserPageTabs } from '@/components/users/user-page-tabs';

const allTabsVisible = {
    starsVisible: true,
    subscriptionsVisible: true,
    groupsVisible: true,
    activityVisible: true,
    runListVisible: true,
    atlasHistoryVisible: true,
    analyticsVisible: true,
};

describe('UserPageTabs', () => {
    it("opens report runs by default for another user's profile", () => {
        window.history.replaceState(null, '', '/users?id=2');

        render(
            <UserPageTabs
                isCurrentUser={false}
                tabs={allTabsVisible}
                stars={<div>Stars panel</div>}
                subscriptions={<div>Subscriptions panel</div>}
                groups={<div>Groups panel</div>}
                activity={<div>Activity panel</div>}
                runList={<div>Report runs panel</div>}
                atlasHistory={<div>Atlas history panel</div>}
                analytics={<div>Analytics panel</div>}
            />,
        );

        expect(screen.getByText('Report runs panel')).toBeInTheDocument();
        expect(screen.queryByText('Activity panel')).not.toBeInTheDocument();
        expect(screen.queryByText('Stars panel')).not.toBeInTheDocument();
        expect(window.location.hash).toBe('#run-list');
    });

    it('opens stars by default for the current user', () => {
        window.history.replaceState(null, '', '/users?id=1');

        render(
            <UserPageTabs
                isCurrentUser
                tabs={allTabsVisible}
                stars={<div>Stars panel</div>}
                subscriptions={<div>Subscriptions panel</div>}
                groups={<div>Groups panel</div>}
                activity={<div>Activity panel</div>}
                runList={<div>Report runs panel</div>}
                atlasHistory={<div>Atlas history panel</div>}
                analytics={<div>Analytics panel</div>}
            />,
        );

        expect(screen.getByText('Stars panel')).toBeInTheDocument();
        expect(screen.queryByText('Report runs panel')).not.toBeInTheDocument();
        expect(window.location.hash).toBe('#stars');
    });
});
