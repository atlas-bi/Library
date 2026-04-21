"use client";

import type { ReactNode } from "react";
import { useAuth } from "./auth-provider";
import type { Permission } from "@/lib/auth/types";

// ---------------------------------------------------------------------------
// Declarative permission gate component
// ---------------------------------------------------------------------------

interface RequirePermissionProps {
  /** The permission(s) the user must have. */
  permission: Permission | Permission[];
  /**
   * When multiple permissions are provided:
   * - `"all"` → user needs ALL of them (default)
   * - `"any"` → user needs at least ONE
   */
  mode?: "all" | "any";
  /** Content to render when the user lacks the permission. */
  fallback?: ReactNode;
  children: ReactNode;
}

/**
 * Conditionally renders children based on the current user's permissions.
 *
 * @example
 * ```tsx
 * <RequirePermission permission="Edit Report Documentation">
 *   <EditReportButton />
 * </RequirePermission>
 *
 * <RequirePermission permission={["Approve Terms", "Edit Terms"]} mode="any">
 *   <TermsManagementPanel />
 * </RequirePermission>
 * ```
 */
export function RequirePermission({
  permission,
  mode = "all",
  fallback = null,
  children,
}: RequirePermissionProps) {
  const { hasPermission, hasAllPermissions, hasAnyPermission, loading } =
    useAuth();

  if (loading) return null;

  const allowed = Array.isArray(permission)
    ? mode === "any"
      ? hasAnyPermission(permission)
      : hasAllPermissions(permission)
    : hasPermission(permission);

  return allowed ? <>{children}</> : <>{fallback}</>;
}

// ---------------------------------------------------------------------------
// Role-based gate
// ---------------------------------------------------------------------------

interface RequireRoleProps {
  /** The role(s) the user must have. */
  role: string | string[];
  /** When multiple roles: `"any"` = at least one (default), `"all"` = every role. */
  mode?: "all" | "any";
  fallback?: ReactNode;
  children: ReactNode;
}

/**
 * Conditionally renders children based on the current user's roles.
 *
 * @example
 * ```tsx
 * <RequireRole role="Term Administrator">
 *   <TermAdminDashboard />
 * </RequireRole>
 * ```
 */
export function RequireRole({
  role,
  mode = "any",
  fallback = null,
  children,
}: RequireRoleProps) {
  const { hasRole, hasAnyRole, loading } = useAuth();

  if (loading) return null;

  const allowed = Array.isArray(role)
    ? mode === "any"
      ? hasAnyRole(role)
      : role.every((r) => hasRole(r))
    : hasRole(role);

  return allowed ? <>{children}</> : <>{fallback}</>;
}

// ---------------------------------------------------------------------------
// Admin-only gate
// ---------------------------------------------------------------------------

interface RequireAdminProps {
  fallback?: ReactNode;
  children: ReactNode;
}

/**
 * Renders children only when `adminEnabled` is `true`.
 *
 * @example
 * ```tsx
 * <RequireAdmin>
 *   <AdminSettingsPanel />
 * </RequireAdmin>
 * ```
 */
export function RequireAdmin({
  fallback = null,
  children,
}: RequireAdminProps) {
  const { isAdmin, loading } = useAuth();

  if (loading) return null;

  return isAdmin ? <>{children}</> : <>{fallback}</>;
}
