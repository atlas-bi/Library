/** Razor Edit.cshtml / _Images.cshtml thumb URLs (ImageData via Img page handler). */
export function getReportObjectImageThumbUrl(imageId: number, size = "256x256"): string {
  return `/data/img?handler=Thumb&id=-1&imgId=${imageId}&size=${size}`
}
