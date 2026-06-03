using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Domain;

public class MatterEntityTests
{
    [Fact]
    public void NewMatter_ShouldHaveOpenStatus()
    {
        var matter = new Matter();
        matter.Status.Should().Be(MatterStatus.Open);
    }

    [Fact]
    public void NewMatter_ShouldHaveHourlyFeeArrangement()
    {
        var matter = new Matter();
        matter.FeeArrangement.Should().Be(FeeArrangement.Hourly);
    }

    [Fact]
    public void NewMatter_ShouldNotBeDeleted()
    {
        var matter = new Matter();
        matter.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void NewClient_ShouldHavePendingStatus()
    {
        var client = new Client();
        client.Status.Should().Be(ClientStatus.Pending);
    }

    [Fact]
    public void NewTimeEntry_ShouldHaveDraftStatus()
    {
        var entry = new TimeEntry();
        entry.Status.Should().Be(TimeEntryStatus.Draft);
    }

    [Fact]
    public void NewTimeEntry_ShouldBeBillableByDefault()
    {
        var entry = new TimeEntry();
        entry.IsBillable.Should().BeTrue();
    }

    [Fact]
    public void NewInvoice_ShouldHaveDraftStatus()
    {
        var invoice = new Invoice();
        invoice.Status.Should().Be(InvoiceStatus.Draft);
    }

    [Fact]
    public void NewInvoice_ShouldHave20PercentTaxRate()
    {
        var invoice = new Invoice();
        invoice.TaxRate.Should().Be(20.00m);
    }
}
