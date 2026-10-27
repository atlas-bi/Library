"use client"

import { useEffect, useRef } from "react"
import { formatAnalyticsCount } from "@/lib/analytics/format"
import type { AnalyticsAccessHistoryPoint } from "@/lib/analytics/types"

function formatAxisSeconds(value: number): string {
  return `${Math.round(value * 10) / 10}s`
}

export function AnalyticsVisitsChart({ history }: { history: AnalyticsAccessHistoryPoint[] }) {
  const canvasRef = useRef<HTMLCanvasElement>(null)
  const chartRef = useRef<{ destroy: () => void } | null>(null)

  useEffect(() => {
    if (!canvasRef.current || history.length === 0) return

    let cancelled = false

    const render = async () => {
      const { default: Chart } = await import("chart.js")
      if (cancelled || !canvasRef.current) return

      chartRef.current?.destroy()

      const ctx = canvasRef.current.getContext("2d")
      if (!ctx) return

      chartRef.current = new Chart(ctx, {
        type: "bar",
        data: {
          labels: history.map((point) => point.date),
          datasets: [
            {
              label: "Load Time",
              borderColor: "rgba(38,128,235,0.2)",
              backgroundColor: "transparent",
              type: "line",
              yAxisID: "2",
              data: history.map((point) => point.loadTime),
              fill: false,
              lineTension: 0,
              borderWidth: 2,
              pointRadius: 2,
            },
            {
              label: "Views",
              backgroundColor: "rgba(38,128,235,0.3)",
              borderColor: "rgba(38,128,235,0.4)",
              borderWidth: 1,
              stack: "bar",
              yAxisID: "1",
              data: history.map((point) => point.pages),
            },
            {
              label: "Visitors",
              backgroundColor: "rgba(38,128,235,0.5)",
              borderColor: "rgba(38,128,235,0.6)",
              borderWidth: 1,
              stack: "bar",
              yAxisID: "1",
              data: history.map((point) => point.sessions),
            },
          ],
        },
        options: {
          maintainAspectRatio: false,
          responsive: true,
          animation: { duration: 300 },
          legend: { display: true, position: "bottom" },
          title: { display: false },
          scales: {
            yAxes: [
              {
                id: "1",
                position: "left",
                ticks: {
                  beginAtZero: true,
                  callback: (value: number) => formatAnalyticsCount(Number(value)),
                },
                stacked: false,
              },
              {
                id: "2",
                position: "right",
                ticks: {
                  beginAtZero: true,
                  callback: (value: number) => formatAxisSeconds(Number(value)),
                },
                stacked: false,
                gridLines: { drawOnChartArea: false },
              },
            ],
            xAxes: [
              {
                stacked: true,
                gridLines: { display: false },
              },
            ],
          },
        },
      })
    }

    void render()

    return () => {
      cancelled = true
      chartRef.current?.destroy()
      chartRef.current = null
    }
  }, [history])

  if (history.length === 0) {
    return (
      <div className="flex h-[400px] items-center justify-center border border-transparent text-sm text-[#7a7a7a]">
        No visit history for this range.
      </div>
    )
  }

  return (
    <div className="chart-wrapper h-[400px] w-full">
      <canvas ref={canvasRef} aria-label="Visits chart" />
    </div>
  )
}
