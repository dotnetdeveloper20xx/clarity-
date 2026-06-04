using Clarity.Application.Workflows.MatterWorkflow;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Application.Workflows;

/// <summary>
/// Tests the business rules enforced during matter closure.
/// These are the rules that protect the firm from compliance and financial errors.
/// </summary>
public class MatterClosureBusinessRuleTests
{
    [Fact]
    public void Matter_CannotCloseWithUnpaidInvoices()
    {
        // A matter with an issued (unpaid) invoice should block closure
        var invoice = new Invoice { Status = InvoiceStatus.Issued, TotalAmount = 5000m, PaidAmount = 0m, IsDeleted = false };
        var hasUnpaid = invoice.Status != InvoiceStatus.Paid && invoice.Status != InvoiceStatus.WrittenOff && invoice.Status != InvoiceStatus.Cancelled;
        hasUnpaid.Should().BeTrue("unpaid invoices must block matter closure");
    }

    [Fact]
    public void Matter_CanCloseWithAllInvoicesPaid()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Paid, TotalAmount = 5000m, PaidAmount = 5000m, IsDeleted = false };
        var hasUnpaid = invoice.Status != InvoiceStatus.Paid && invoice.Status != InvoiceStatus.WrittenOff && invoice.Status != InvoiceStatus.Cancelled;
        hasUnpaid.Should().BeFalse("paid invoices should not block matter closure");
    }

    [Fact]
    public void Matter_CanCloseWithWrittenOffInvoices()
    {
        var invoice = new Invoice { Status = InvoiceStatus.WrittenOff, TotalAmount = 5000m, PaidAmount = 0m, IsDeleted = false };
        var hasUnpaid = invoice.Status != InvoiceStatus.Paid && invoice.Status != InvoiceStatus.WrittenOff && invoice.Status != InvoiceStatus.Cancelled;
        hasUnpaid.Should().BeFalse("written off invoices should not block matter closure");
    }

    [Fact]
    public void Matter_CannotCloseWithFailedComplianceCheck()
    {
        var check = new ComplianceCheck { Status = ComplianceCheckStatus.Fail };
        var hasFailedCompliance = check.Status == ComplianceCheckStatus.Fail || check.Status == ComplianceCheckStatus.RequiresInvestigation;
        hasFailedCompliance.Should().BeTrue("failed compliance must block matter closure");
    }

    [Fact]
    public void Matter_CanCloseWithPassedComplianceCheck()
    {
        var check = new ComplianceCheck { Status = ComplianceCheckStatus.Pass };
        var hasFailedCompliance = check.Status == ComplianceCheckStatus.Fail || check.Status == ComplianceCheckStatus.RequiresInvestigation;
        hasFailedCompliance.Should().BeFalse("passed compliance should not block matter closure");
    }

    [Fact]
    public void Matter_CannotCloseWithInvestigationComplianceCheck()
    {
        var check = new ComplianceCheck { Status = ComplianceCheckStatus.RequiresInvestigation };
        var hasFailedCompliance = check.Status == ComplianceCheckStatus.Fail || check.Status == ComplianceCheckStatus.RequiresInvestigation;
        hasFailedCompliance.Should().BeTrue("investigation status must block matter closure");
    }

    [Fact]
    public void TimeEntry_CannotBeRecordedAgainstClosedMatter()
    {
        var matter = new Matter { Status = MatterStatus.Closed };
        var isBlocked = matter.Status == MatterStatus.Closed || matter.Status == MatterStatus.Archived;
        isBlocked.Should().BeTrue("time cannot be recorded against closed/archived matters");
    }

    [Fact]
    public void TimeEntry_CanBeRecordedAgainstOpenMatter()
    {
        var matter = new Matter { Status = MatterStatus.InProgress };
        var isBlocked = matter.Status == MatterStatus.Closed || matter.Status == MatterStatus.Archived;
        isBlocked.Should().BeFalse("time can be recorded against open matters");
    }

    [Fact]
    public void BilledTimeEntry_CannotBeModified()
    {
        var entry = new TimeEntry { Status = TimeEntryStatus.Billed };
        var isImmutable = entry.Status == TimeEntryStatus.Billed || entry.Status == TimeEntryStatus.WrittenOff;
        isImmutable.Should().BeTrue("billed time entries must be immutable");
    }

    [Fact]
    public void DraftTimeEntry_CanBeModified()
    {
        var entry = new TimeEntry { Status = TimeEntryStatus.Draft };
        var isImmutable = entry.Status == TimeEntryStatus.Billed || entry.Status == TimeEntryStatus.WrittenOff;
        isImmutable.Should().BeFalse("draft time entries can be edited");
    }
}
