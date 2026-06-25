/** Match Razor defaults: features are enabled unless explicitly set to false. */
export function isInteractionFeatureEnabled(flag?: boolean): boolean {
  return flag !== false
}
