-- =============================================
-- Stored Procedure: sp_GetConsultantProductivity
-- Purpose: Team leader / management reporting on fee earner productivity
-- Performance: Aggregates time entries by user with billable/non-billable split
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetConsultantProductivity]
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FromDate IS NULL SET @FromDate = DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1);
    IF @ToDate IS NULL SET @ToDate = CAST(GETUTCDATE() AS DATE);

    SELECT
        u.Id AS UserId,
        u.FirstName + ' ' + u.LastName AS ConsultantName,
        u.Email,
        COUNT(*) AS TotalEntries,
        SUM(t.DurationMinutes) AS TotalMinutes,
        SUM(CASE WHEN t.IsBillable = 1 THEN t.DurationMinutes ELSE 0 END) AS BillableMinutes,
        SUM(CASE WHEN t.IsBillable = 0 THEN t.DurationMinutes ELSE 0 END) AS NonBillableMinutes,
        -- Utilisation rate (billable / total * 100)
        CASE WHEN SUM(t.DurationMinutes) > 0
            THEN CAST(SUM(CASE WHEN t.IsBillable = 1 THEN t.DurationMinutes ELSE 0 END) AS DECIMAL(5,2))
                / CAST(SUM(t.DurationMinutes) AS DECIMAL(5,2)) * 100
            ELSE 0
        END AS UtilisationRate,
        -- Revenue value
        SUM(CASE WHEN t.IsBillable = 1 AND t.RateAmount IS NOT NULL
            THEN CAST(t.DurationMinutes AS DECIMAL(18,2)) / 60.0 * t.RateAmount
            ELSE 0
        END) AS BillableValue,
        -- Status breakdown
        SUM(CASE WHEN t.Status = 0 THEN 1 ELSE 0 END) AS DraftCount,
        SUM(CASE WHEN t.Status = 1 THEN 1 ELSE 0 END) AS SubmittedCount,
        SUM(CASE WHEN t.Status = 2 THEN 1 ELSE 0 END) AS ApprovedCount,
        SUM(CASE WHEN t.Status = 4 THEN 1 ELSE 0 END) AS BilledCount,
        -- Active matters
        COUNT(DISTINCT t.MatterId) AS ActiveMatterCount
    FROM TimeEntries t
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE t.IsDeleted = 0
        AND t.[Date] >= @FromDate
        AND t.[Date] <= @ToDate
    GROUP BY u.Id, u.FirstName, u.LastName, u.Email
    ORDER BY BillableValue DESC;
END
GO
