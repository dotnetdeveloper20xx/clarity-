using Clarity.Application.Workflows.InvoiceWorkflow;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Workflows;

public class InvoiceWorkflowTests
{
    [Theory]
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Issued, true)]
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Cancelled, true)]
    [InlineData(InvoiceStatus.Issued, InvoiceStatus.PartiallyPaid, true)]
    [InlineData(InvoiceStatus.Issued, InvoiceStatus.Paid, true)]
    [InlineData(InvoiceStatus.Issued, InvoiceStatus.WrittenOff, true)]
    [InlineData(InvoiceStatus.PartiallyPaid, InvoiceStatus.Paid, true)]
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Paid, false)] // Invalid: must issue first
    [InlineData(InvoiceStatus.Paid, InvoiceStatus.Draft, false)] // Invalid: terminal
    [InlineData(InvoiceStatus.Cancelled, InvoiceStatus.Issued, false)] // Invalid: terminal
    public void InvoiceStatusTransition_ShouldValidateCorrectly(InvoiceStatus from, InvoiceStatus to, bool expected)
    {
        var result = InvoiceStatusTransitionValidator.IsValidTransition(from, to);
        result.Should().Be(expected);
    }
}
