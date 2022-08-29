drop table if exists #report_cert_load_temp;
drop table if exists #report_cert_temp;

with NMN
as (
  select [t4].ReportObjectID
    ,(
      case
        when (
            (
              case
                when [t4].[MaintenanceScheduleID] = 1
                  then Dateadd(MONTH, 3, COALESCE([t4].[value], Getdate()))
                when [t4].[MaintenanceScheduleID] = 2
                  then Dateadd(MONTH, 6, COALESCE([t4].[value], Getdate()))
                when [t4].[MaintenanceScheduleID] = 3
                  then Dateadd(YEAR, 1, COALESCE([t4].[value], Getdate()))
                when [t4].[MaintenanceScheduleID] = 4
                  then Dateadd(YEAR, 2, COALESCE([t4].[value], Getdate()))
                else COALESCE([t4].[value], Getdate())
                end
              )
            ) > Getdate()
          then 1
        else 0
        end
      ) as [no_maintenance_needed]
  from (
    select [t0].[MaintenanceScheduleID]
      ,(
        select [t3].[MaintenanceDate]
        from (
          select top (1) [t1].[MaintenanceDate]
          from [app].[MaintenanceLog] as [t1]
          where [t1].[MaintenanceLogID] = (
              (
                select Max([t2].[MaintenanceLogID])
                from [app].[MaintenanceLog] as [t2]
                where [t2].ReportId = [t0].[ReportObjectID]
                )
              )
          ) as [t3]
        ) as [value]
      ,[t0].[ReportObjectID]
    from [app].[ReportObject_doc] as [t0]
    left join dbo.ReportObject r on [t0].ReportObjectID = r.ReportObjectID
    ) as [t4]
  )
  ,ML
as (
  select r.ReportObjectID
  from dbo.ReportObject r
  left join app.MaintenanceLog l on l.reportid = r.ReportObjectID
  where l.MaintenanceLogStatusID in (
      1
      ,2
      )
  )
  ,I
as (
  select max(i.ImageID) as ImageID
    ,r.ReportObjectID
  from dbo.ReportObject r
  left join app.ReportObjectImages_doc i on i.ReportObjectID = r.ReportObjectID
  group by r.ReportObjectID
  )
select distinct r.reportobjectid
  ,r.ReportObjectTypeID
  ,case
    when ML.ReportObjectID is not null
      and d.MaintenanceScheduleID in (
        1
        ,2
        ,3
        )
      and Isnull(NMN.no_maintenance_needed, 0) = 1
      then 'Analytics Certified'
    when r.EpicReleased = 'Y'
      then 'Epic Released'
    when I.ImageID is not null
      and (
        d.KeyAssumptions is not null
        or d.DeveloperDescription is not null
        )
      then 'Analytics Reviewed'
    when Isnull((
          select Sum(t.Runs)
          from dbo.ReportObjectRunDataBridge t
          where t.ReportObjectId = r.ReportObjectID
          ), 0) > 24
      then 'Legacy'
    else 'High Risk'
    end as CertTag
into #report_cert_temp
from dbo.ReportObject r
left outer join app.ReportObject_doc d on d.reportobjectid = r.reportobjectid
left outer join NMN on r.ReportObjectID = NMN.reportobjectid
left outer join ML on r.ReportObjectID = ML.ReportObjectID
left outer join I on r.ReportObjectID = I.ReportObjectID

update r
set r.CertTag = 'Self-Service'
from #report_cert_temp r
where r.ReportObjectTypeID in (
    23
    ,19
    )
  and r.CertTag like '%Analytics%';

update r
set r.CertTag = 'Self-Service'
from #report_cert_temp r
where r.ReportObjectTypeID in (
    37
    ,41
    ,42
    );

select r.ReportObjectID as ReportId
  ,t.tagid as Tagid
into #report_cert_load_temp
from #report_cert_temp r
inner join dbo.Tags t on r.CertTag = t.Name

merge dbo.ReportTagLinks as dest
using #report_cert_load_temp as source
  on dest.ReportId = source.ReportId
    and dest.TagId = source.TagId
when not matched
  then
    insert (
      tagid
      ,reportid
      )
    values (
      source.tagid
      ,source.reportid
      )
when not matched by SOURCE
  then
    delete;
