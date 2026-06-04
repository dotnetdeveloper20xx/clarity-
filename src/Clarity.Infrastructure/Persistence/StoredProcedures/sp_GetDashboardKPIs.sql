-- =============================================
-- Stored Procedure: sp_GetDashboardKPIs
-- Purpose: Single-call dashboard data retrieval for optimal performance
-- Performance: Aggregates across multiple tables in one round-trip
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetDashboardKPIs]
    @UserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1);

    -- Open matters
    SELECT COUNT(*) AS OpenMattersCount
    FROM Matters
    WHERE IsDeleted = 0 AND Status NOT IN (6, 7); -- Not Closed/Archived

    -- Overdue tasks
    SELECT COUNT(*) AS OverdueTasksCount
    FROM MatterTasks
    WHERE IsDeleted = 0 AND DueDate < @Today AND Status NOT IN (3, 4); -- Not Complete/Cancelled

    -- Pending compliance
    SELECT COUNT(*) AS PendingComplianceCount
    FROM ComplianceChecks WHERE Status = 0; -- Pending

    -- Compliance alerts (failed + investigation)
    SELECT COUNT(*) AS ComplianceAlertsCount
    FROM ComplianceChecks WHERE Status IN (2, 3); -- Fail, RequiresInvestigation

    -- Unbilled time value (approved, not yet invoiced)
    SELECT ISNULL(SUM(
        CAST(DurationMinutes AS DECIMAL(18,2)) / 60.0 * ISNULL(RateAmount, 0)
    ), 0) AS UnbilledTimeValue
    FROM TimeEntries
    WHERE IsDeleted = 0 AND Status = 2 AND InvoiceId IS NULL AND IsBillable = 1; -- Approved, unbilled

    -- Outstanding invoices
    SELECT ISNULL(SUM(TotalAmount - PaidAmount), 0) AS OutstandingInvoicesTotal
    FROM Invoices
    WHERE IsDeleted = 0 AND Status IN (1, 2); -- Issued, PartiallyPaid

    -- Paid this month
    SELECT ISNULL(SUM(Amount), 0) AS PaidThisMonthTotal
    FROM Payments
    WHERE IsReversed = 0 AND PaymentDate >= @MonthStart;

    -- Draft time entries
    SELECT COUNT(*) AS DraftTimeEntriesCount
    FROM TimeEntries WHERE IsDeleted = 0 AND Status = 0; -- Draft

    -- Pending approvals (submitted)
    SELECT COUNT(*) AS PendingApprovalsCount
    FROM TimeEntries WHERE IsDeleted = 0 AND Status = 1; -- Submitted

    -- Unread notifications for current user
    IF @UserId IS NOT NULL
        SELECT COUNT(*) AS UnreadNotificationsCount
        FROM Notifications WHERE UserId = @UserId AND IsRead = 0;
    ELSE
        SELECT 0 AS UnreadNotificationsCount;
END
GO
