import { getServerApiBase } from "@/lib/api-base";
import { getToken } from "@/lib/auth";
import type { ReportDetail } from "./types";

export async function getReportDetailById(
  id: number
): Promise<ReportDetail | null> {
  const token = await getToken();
  if (!token) return null;

  const apiBase = getServerApiBase();
  if (!apiBase) return null;

  const res = await fetch(`${apiBase}/api/reports/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  });

  if (!res.ok) return null;
  return (await res.json()) as ReportDetail;
}

