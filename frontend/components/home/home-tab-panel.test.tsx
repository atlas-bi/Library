import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { describe, expect, it } from 'vitest';
import { HomeTabPanel } from '@/components/home/home-tab-panel';
import { TooltipProvider } from '@/components/ui/tooltip';
import type { UserStars } from '@/lib/users/types';

const editableStarsFixture: UserStars = {
    userId: 1,
    viewerUserId: 1,
    isCurrentUser: true,
    canEditWorkspace: true,
    permissions: {
        canCreateFolders: true,
        canRenameFolders: true,
        canDeleteFolders: true,
        canReorderFolders: true,
        canReorderFavorites: true,
        canMoveFavoritesToFolders: true,
        canToggleFavorites: true,
    },
    summary: {
        totalCount: 1,
        unsortedCount: 1,
        hasFolders: false,
        showUnsortedBucket: true,
    },
    filters: {
        hasReports: true,
        hasCollections: false,
        hasInitiatives: false,
        hasTerms: false,
        hasUsers: false,
        hasGroups: false,
        hasSearches: false,
        showQuickFilters: true,
    },
    folders: [],
    items: [
        {
            starId: 10,
            type: 'report',
            typeLabel: 'Report',
            itemId: 7,
            name: 'Executive Dashboard',
            description: 'Leadership summary',
            url: '/reports?id=7',
            canReorder: true,
            isStarred: true,
            starCount: 2,
            isCertified: false,
            isApproved: true,
            canOpenProfile: true,
            canShare: true,
            canRequestAccess: false,
            canRun: true,
            opensRunModal: false,
            canEditInEditor: false,
            canManageInEditor: false,
            tags: [],
            relatedCollectionNames: [],
        },
    ],
    suggestedReports: [],
};

function renderWithProviders(ui: ReactNode) {
    return render(<TooltipProvider>{ui}</TooltipProvider>);
}

describe('HomeTabPanel', () => {
    it('renders the editable stars workspace on the home Stars tab', () => {
        renderWithProviders(
            <HomeTabPanel
                panel={{
                    kind: 'stars-workspace',
                    stars: editableStarsFixture,
                    sharedWithMe: [
                        {
                            id: 99,
                            name: 'Shared Report',
                            href: '/reports?id=99',
                            sharedFrom: 'Jane Smith',
                        },
                    ],
                }}
            />,
        );

        expect(screen.getByText('New folder')).toBeInTheDocument();
        expect(screen.getByText('Executive Dashboard')).toBeInTheDocument();
        expect(screen.getByText('Shared With Me')).toBeInTheDocument();
        expect(screen.getByRole('link', { name: 'Shared Report' })).toBeInTheDocument();
    });
});