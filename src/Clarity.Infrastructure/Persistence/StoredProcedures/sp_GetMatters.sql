-- =============================================
-- Stored Procedure: sp_GetMatters
-- Purpose: Paginated, sorted, filtered matter retrieval with client/consultant joins
-- Performance: Uses indexed columns (ClientId, Status, LeadConsultantId)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetMatters]
    @SearchTerm NVARCHAR(200) = NULL,
    @Status INT = NULL,
    @MatterType INT = NULL,
    @ClientId UNIQUEIDENTIFIER = NULL,
    @LeadConsultantId UNIQUEIDENTIFIER = NULL,
    @SortColumn NVARCHAR(50) = 'OpenedDate',
    @SortDirection NVARCHAR(4) = 'DESC',
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Total count
    SELECT COUNT(*) AS TotalCount
    FROM Matters m
    WHERE m.IsDeleted = 0
        AND (@SearchTerm IS NULL OR m.Title LIKE '%' + @SearchTerm + '%' OR m.ReferenceNumber LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR m.Status = @Status)
        AND (@MatterType IS NULL OR m.MatterType = @MatterType)
        AND (@ClientId IS NULL OR m.ClientId = @ClientId)
        AND (@LeadConsultantId IS NULL OR m.LeadConsultantId = @LeadConsultantId);

    -- Paginated results with joins
    SELECT
        m.Id, m.ReferenceNumber, m.ClientId, c.Name AS ClientName,
        m.Title, m.[Description], m.MatterType, m.Status,
        m.FeeArrangement, m.EstimatedValue, m.FixedFeeAmount,
        m.OpenedDate, m.ClosedDate,
        m.LeadConsultantId, u.FirstName + ' ' + u.LastName AS LeadConsultantName,
        m.CreatedAt
    FROM Matters m
    INNER JOIN Clients c ON m.ClientId = c.Id
    INNER JOIN Users u ON m.LeadConsultantId = u.Id
    WHERE m.IsDeleted = 0
        AND (@SearchTerm IS NULL OR m.Title LIKE '%' + @SearchTerm + '%' OR m.ReferenceNumber LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR m.Status = @Status)
        AND (@MatterType IS NULL OR m.MatterType = @MatterType)
        AND (@ClientId IS NULL OR m.ClientId = @ClientId)
        AND (@LeadConsultantId IS NULL OR m.LeadConsultantId = @LeadConsultantId)
    ORDER BY
        CASE WHEN @SortColumn = 'OpenedDate' AND @SortDirection = 'DESC' THEN m.OpenedDate END DESC,
        CASE WHEN @SortColumn = 'OpenedDate' AND @SortDirection = 'ASC' THEN m.OpenedDate END ASC,
        CASE WHEN @SortColumn = 'Title' AND @SortDirection = 'ASC' THEN m.Title END ASC,
        CASE WHEN @SortColumn = 'Title' AND @SortDirection = 'DESC' THEN m.Title END DESC,
        CASE WHEN @SortColumn = 'Status' AND @SortDirection = 'ASC' THEN m.Status END ASC,
        CASE WHEN @SortColumn = 'Status' AND @SortDirection = 'DESC' THEN m.Status END DESC,
        m.OpenedDate DESC -- Default
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
