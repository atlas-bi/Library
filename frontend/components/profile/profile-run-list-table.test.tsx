import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import {
    ProfileRunListTable,
    compareProfileRunListRows,
} from '@/components/profile/profile-run-list-table';

describe('compareProfileRunListRows', () => {
    it('sorts runs numerically', () => {
        expect(
            compareProfileRunListRows(
                { name: 'A', runs: 2 },
                { name: 'B', runs: 10 },
                'runs',
            ),
        ).toBeLessThan(0);
    });

    it('sorts last run values as dates when possible', () => {
        expect(
            compareProfileRunListRows(
                { name: 'A', lastRun: '01/02/2024' },
                { name: 'B', lastRun: '02/02/2024' },
                'lastRun',
            ),
        ).toBeLessThan(0);
    });
});

describe('ProfileRunListTable', () => {
    it('renders sortable columns and reorders rows when a header is clicked', async () => {
        const user = userEvent.setup();
        render(
            <ProfileRunListTable
                rows={[
                    { name: 'Beta Report', runs: 1, lastRun: '01/01/2024' },
                    { name: 'Alpha Report', runs: 3, lastRun: '02/01/2024' },
                ]}
            />,
        );

        const bodyRows = () =>
            screen.getAllByRole('row').slice(1).map((row) => row.textContent ?? '');

        expect(bodyRows()[0]).toContain('Beta Report');
        expect(bodyRows()[1]).toContain('Alpha Report');

        await user.click(screen.getByRole('button', { name: /^Name/ }));

        expect(bodyRows()[0]).toContain('Alpha Report');
    });
});
