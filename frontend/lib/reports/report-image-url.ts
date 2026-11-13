/** Razor Edit.cshtml / _Images.cshtml thumb path (ImageData via Img page handler). */
export function getReportObjectImageThumbPath(imageId: number, size = "256x256"): string {
  return `/data/img?handler=Thumb&id=-1&imgId=${imageId}&size=${size}`
}

export function getReportObjectImageThumbUrl(
  imageId: number,
  options?: { apiBase?: string; size?: string },
): string {
  const path = getReportObjectImageThumbPath(imageId, options?.size)
  const apiBase = options?.apiBase?.replace(/\/+$/, "")
  if (!apiBase) return path
  return `${apiBase}${path}`
}
