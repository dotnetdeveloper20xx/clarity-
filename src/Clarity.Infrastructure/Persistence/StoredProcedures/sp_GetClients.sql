-- =============================================
-- Stored Procedure: sp_GetClients
-- Purpose: Paginated, sorted, filtered client retrieval
-- Performance: Uses indexed columns for filtering
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetClients]
    @SearchTerm NVARCHAR(200) = NULL,
    @Status INT = NULL,
    @ClientType INT = NULL,
    @SortColumn NVARCHAR(50) = 'Name',
    @SortDirection NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Get total count for pagination metadata
    SELECT COUNT(*) AS TotalCount
    FROM Clients
    WHERE IsDeleted = 0
        AND (@SearchTerm IS NULL OR Name LIKE '%' + @SearchTerm + '%' OR ReferenceNumber LIKE '%' + @SearchTerm + '%' OR Email LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR Status = @Status)
        AND (@ClientType IS NULL OR ClientType = @ClientType);

    -- Get paginated results
    SELECT
        Id, ReferenceNumber, Name, ClientType, Status,
        Email, Phone, AddressLine1, City, PostCode, Country,
        CompanyNumber, Notes, CreatedAt
    FROM Clients
    WHERE IsDeleted = 0
        AND (@SearchTerm IS NULL OR Name LIKE '%' + @SearchTerm + '%' OR ReferenceNumber LIKE '%' + @SearchTerm + '%' OR Email LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR Status = @Status)
        AND (@ClientType IS NULL OR ClientType = @ClientType)
    ORDER BY
        CASE WHEN @SortColumn = 'Name' AND @SortDirection = 'ASC' THEN Name END ASC,
        CASE WHEN @SortColumn = 'Name' AND @SortDirection = 'DESC' THEN Name END DESC,
        CASE WHEN @SortColumn = 'ReferenceNumber' AND @SortDirection = 'ASC' THEN ReferenceNumber END ASC,
        CASE WHEN @SortColumn = 'ReferenceNumber' AND @SortDirection = 'DESC' THEN ReferenceNumber END DESC,
        CASE WHEN @SortColumn = 'CreatedAt' AND @SortDirection = 'ASC' THEN CreatedAt END ASC,
        CASE WHEN @SortColumn = 'CreatedAt' AND @SortDirection = 'DESC' THEN CreatedAt END DESC,
        Name ASC -- Default fallback
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
