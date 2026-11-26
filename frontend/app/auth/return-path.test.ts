import { describe, expect, test } from 'vitest';
import {
    buildLoginCallbackUrl,
    loginPath,
    safeReturnPath,
} from './return-path';

describe('auth return path', () => {
    test('keeps same-site paths, including the user menu routes', () => {
        expect(safeReturnPath('/users')).toBe('/users');
        expect(safeReturnPath('/users#stars')).toBe('/users#stars');
        expect(safeReturnPath('/users/settings')).toBe('/users/settings');
        expect(safeReturnPath('/users?id=2')).toBe('/users?id=2');
    });

    test('rejects off-site targets', () => {
        expect(safeReturnPath('https://evil.test/users')).toBe('/');
        expect(safeReturnPath('//evil.test/users')).toBe('/');
        expect(safeReturnPath('/\\evil.test')).toBe('/');
        expect(safeReturnPath('')).toBe('/');
    });

    test('builds a login path that remembers where to return', () => {
        expect(loginPath('/users/settings')).toBe(
            '/auth/login?returnTo=%2Fusers%2Fsettings',
        );
        expect(loginPath('/users#stars')).toBe(
            '/auth/login?returnTo=%2Fusers%23stars',
        );
        expect(loginPath('/')).toBe('/auth/login');
    });

    test('adds the return path to the auth callback', () => {
        expect(
            buildLoginCallbackUrl(
                'https://library.atlas.bi',
                '/users/settings',
            ),
        ).toBe(
            'https://library.atlas.bi/auth/callback?returnTo=%2Fusers%2Fsettings',
        );
        expect(buildLoginCallbackUrl('https://library.atlas.bi/', null)).toBe(
            'https://library.atlas.bi/auth/callback',
        );
    });
});
