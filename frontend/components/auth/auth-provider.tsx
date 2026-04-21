"use client";

import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from "react";
import type { AuthUser, Permission } from "@/lib/auth/types";

// ---------------------------------------------------------------------------
// Context shape
// ---------------------------------------------------------------------------
interface AuthContextValue {
  /** The authenticated user, or `null` while loading / unauthenticated. */
  user: AuthUser | null;
  /** `true` while the initial /api/auth/me request is in-flight. */
  loading: boolean;
  /** Re-fetch the user from the API (e.g. after toggling admin mode). */
  refresh: () => Promise<void>;

  // ── Permission helpers ──────────────────────────────────────────────
  /** `true` when adminEnabled is `true`. */
  isAdmin: boolean;
  /** Check a single permission. Admin bypasses all checks. */
  hasPermission: (permission: Permission) => boolean;
  /** Check that the user holds ALL of the given permissions. */
  hasAllPermissions: (permissions: Permission[]) => boolean;
  /** Check that the user holds at least ONE of the given permissions. */
  hasAnyPermission: (permissions: Permission[]) => boolean;
  /** Check that the user has a specific role. */
  hasRole: (role: string) => boolean;
  /** Check that the user has at least ONE of the given roles. */
  hasAnyRole: (roles: string[]) => boolean;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

// ---------------------------------------------------------------------------
// Provider
// ---------------------------------------------------------------------------
interface AuthProviderProps {
  children: ReactNode;
  /**
   * Optionally pass the user fetched on the server so the client can hydrate
   * immediately without a loading flash.
   */
  initialUser?: AuthUser | null;
}

export function AuthProvider({ children, initialUser }: AuthProviderProps) {
  const [user, setUser] = useState<AuthUser | null>(initialUser ?? null);
  const [loading, setLoading] = useState(!initialUser);

  // ── Fetch user from the API ──────────────────────────────────────────
  const fetchUser = useCallback(async () => {
    try {
      setLoading(true);
      const res = await fetch("/api/auth/me");
      if (!res.ok) {
        setUser(null);
        return;
      }
      const data: AuthUser = await res.json();
      setUser(data);
    } catch {
      setUser(null);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // If we already have a server-supplied user, skip the client fetch.
    if (initialUser) return;
    fetchUser();
  }, [initialUser, fetchUser]);

  // ── Permission helpers ──────────────────────────────────────────────
  const isAdmin = user?.adminEnabled ?? false;

  const hasPermission = useCallback(
    (permission: Permission) => {
      if (!user) return false;
      if (user.adminEnabled) return true;
      return user.permissions.includes(permission);
    },
    [user]
  );

  const hasAllPermissions = useCallback(
    (permissions: Permission[]) => permissions.every((p) => hasPermission(p)),
    [hasPermission]
  );

  const hasAnyPermission = useCallback(
    (permissions: Permission[]) => permissions.some((p) => hasPermission(p)),
    [hasPermission]
  );

  const hasRole = useCallback(
    (role: string) => user?.roles.includes(role) ?? false,
    [user]
  );

  const hasAnyRole = useCallback(
    (roles: string[]) => roles.some((r) => user?.roles.includes(r) ?? false),
    [user]
  );

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        refresh: fetchUser,
        isAdmin,
        hasPermission,
        hasAllPermissions,
        hasAnyPermission,
        hasRole,
        hasAnyRole,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// ---------------------------------------------------------------------------
// Hook
// ---------------------------------------------------------------------------
export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth() must be used within an <AuthProvider />");
  }
  return ctx;
}
