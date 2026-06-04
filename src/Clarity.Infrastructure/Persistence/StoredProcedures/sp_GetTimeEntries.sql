-- =============================================
-- Stored Procedure: sp_GetTimeEntries
-- Purpose: Paginated, sorted, filtered time entry retrieval
-- Performance: Uses indexed columns (MatterId, UserId, Status, Date)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetTimeEntries]
    @MatterId UNIQUEIDENTIFIER = NULL,
    @UserId UNIQUEIDENTIFIER = NULL,
    @Status INT = NULL,
    @IsBillable BIT = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @SortColumn NVARCHAR(50) = 'Date',
    @SortDirection NVARCHAR(4) = 'DESC',
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Total count
    SELECT COUNT(*) AS TotalCount
    FROM TimeEntries t
    WHERE t.IsDeleted = 0
        AND (@MatterId IS NULL OR t.MatterId = @MatterId)
        AND (@UserId IS NULL OR t.UserId = @UserId)
        AND (@Status IS NULL OR t.Status = @Status)
        AND (@IsBillable IS NULL OR t.IsBillable = @IsBillable)
        AND (@FromDate IS NULL OR t.[Date] >= @FromDate)
        AND (@ToDate IS NULL OR t.[Date] <= @ToDate);

    -- Paginated results
    SELECT
        t.Id, t.MatterId, m.ReferenceNumber AS MatterReference, m.Title AS MatterTitle,
        t.UserId, u.FirstName + ' ' + u.LastName AS UserName,
        t.[Date], t.DurationMinutes, t.[Description],
        t.IsBillable, t.RateAmount, t.Status,
        t.ApprovedById, t.ApprovedAt, t.RejectionReason,
        t.CreatedAt,
        -- Calculated fields for reporting
        CAST(t.DurationMinutes AS DECIMAL(10,2)) / 60.0 AS Hours,
        CASE WHEN t.IsBillable = 1 AND t.RateAmount IS NOT NULL
            THEN CAST(t.DurationMinutes AS DECIMAL(10,2)) / 60.0 * t.RateAmount
            ELSE 0
        END AS BillableValue
    FROM TimeEntries t
    INNER JOIN Matters m ON t.MatterId = m.Id
    INNER JOIN Users u ON t.UserId = u.Id
    WHERE t.IsDeleted = 0
        AND (@MatterId IS NULL OR t.MatterId = @MatterId)
        AND (@UserId IS NULL OR t.UserId = @UserId)
        AND (@Status IS NULL OR t.Status = @Status)
        AND (@IsBillable IS NULL OR t.IsBillable = @IsBillable)
        AND (@FromDate IS NULL OR t.[Date] >= @FromDate)
        AND (@ToDate IS NULL OR t.[Date] <= @ToDate)
    ORDER BY
        CASE WHEN @SortColumn = 'Date' AND @SortDirection = 'DESC' THEN t.[Date] END DESC,
        CASE WHEN @SortColumn = 'Date' AND @SortDirection = 'ASC' THEN t.[Date] END ASC,
        CASE WHEN @SortColumn = 'DurationMinutes' AND @SortDirection = 'DESC' THEN t.DurationMinutes END DESC,
        CASE WHEN @SortColumn = 'DurationMinutes' AND @SortDirection = 'ASC' THEN t.DurationMinutes END ASC,
        t.[Date] DESC -- Default
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
