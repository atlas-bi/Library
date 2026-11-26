import { NextResponse } from 'next/server';
import { getAuthRedirectOrigin } from '../redirect-origin';
import { loginPath, safeReturnPath } from '../return-path';

const tokenCookieName = 'atlas_token';
const maxAgeSeconds = 8 * 60 * 60;

export function GET(request: Request) {
    const url = new URL(request.url);
    const redirectOrigin = getAuthRedirectOrigin(request.url);
    const token = url.searchParams.get('token');

    const returnTo = safeReturnPath(url.searchParams.get('returnTo'));

    if (!token) {
        return NextResponse.redirect(
            new URL(loginPath(returnTo), redirectOrigin),
        );
    }

    const response = NextResponse.redirect(new URL(returnTo, redirectOrigin));
    response.cookies.set({
        name: tokenCookieName,
        value: token,
        httpOnly: true,
        sameSite: 'lax',
        secure: redirectOrigin.startsWith('https://'),
        path: '/',
        maxAge: maxAgeSeconds,
    });

    return response;
}
