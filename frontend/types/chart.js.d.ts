declare module "chart.js" {
  const Chart: new (ctx: CanvasRenderingContext2D, config: unknown) => {
    destroy: () => void
    update: () => void
  }
  export default Chart
}
