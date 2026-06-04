-- =============================================
-- Stored Procedure: sp_GetAgedDebt
-- Purpose: Financial reporting - aged debt analysis
-- Performance: Single query with band calculation
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAgedDebt]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);

    SELECT
        c.Id AS ClientId,
        c.Name AS ClientName,
        i.InvoiceNumber,
        i.TotalAmount,
        i.PaidAmount,
        (i.TotalAmount - i.PaidAmount) AS Outstanding,
        i.DueDate,
        DATEDIFF(DAY, i.DueDate, @Today) AS DaysOverdue,
        CASE
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 0 THEN 'Current'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 30 THEN '1-30 Days'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 60 THEN '31-60 Days'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 90 THEN '61-90 Days'
            ELSE '90+ Days'
        END AS AgeBand
    FROM Invoices i
    INNER JOIN Clients c ON i.ClientId = c.Id
    WHERE i.IsDeleted = 0
        AND i.Status IN (1, 2) -- Issued or PartiallyPaid
        AND i.DueDate IS NOT NULL
    ORDER BY DaysOverdue DESC;

    -- Summary by band
    SELECT
        CASE
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 0 THEN 'Current'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 30 THEN '1-30 Days'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 60 THEN '31-60 Days'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 90 THEN '61-90 Days'
            ELSE '90+ Days'
        END AS Band,
        COUNT(*) AS InvoiceCount,
        SUM(i.TotalAmount - i.PaidAmount) AS TotalOutstanding
    FROM Invoices i
    WHERE i.IsDeleted = 0
        AND i.Status IN (1, 2)
        AND i.DueDate IS NOT NULL
    GROUP BY
        CASE
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 0 THEN 'Current'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 30 THEN '1-30 Days'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 60 THEN '31-60 Days'
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 90 THEN '61-90 Days'
            ELSE '90+ Days'
        END
    ORDER BY
        CASE
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 0 THEN 0
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 30 THEN 1
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 60 THEN 2
            WHEN DATEDIFF(DAY, i.DueDate, @Today) <= 90 THEN 3
            ELSE 4
        END;
END
GO
