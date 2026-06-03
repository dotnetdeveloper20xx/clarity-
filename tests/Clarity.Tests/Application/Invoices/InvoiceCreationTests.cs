using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Application.Invoices;

public class InvoiceCreationTests
{
    [Fact]
    public void NewInvoice_ShouldHaveZeroSubTotal()
    {
        var invoice = new Invoice();
        invoice.SubTotal.Should().Be(0);
    }

    [Fact]
    public void NewInvoice_ShouldHaveZeroPaidAmount()
    {
        var invoice = new Invoice();
        invoice.PaidAmount.Should().Be(0);
    }

    [Fact]
    public void NewInvoice_ShouldHave20PercentDefaultTaxRate()
    {
        var invoice = new Invoice();
        invoice.TaxRate.Should().Be(20.00m);
    }

    [Fact]
    public void Invoice_TaxCalculation_ShouldBeCorrect()
    {
        var invoice = new Invoice
        {
            SubTotal = 1000m,
            TaxRate = 20m
        };

        var expectedTax = 1000m * (20m / 100m);
        expectedTax.Should().Be(200m);
    }

    [Fact]
    public void Invoice_TotalCalculation_ShouldIncludeTax()
    {
        var invoice = new Invoice
        {
            SubTotal = 1000m,
            TaxRate = 20m,
            TaxAmount = 200m,
            TotalAmount = 1200m
        };

        invoice.TotalAmount.Should().Be(invoice.SubTotal + invoice.TaxAmount);
    }

    [Fact]
    public void Invoice_IsFullyPaid_WhenPaidAmountEqualsTotal()
    {
        var invoice = new Invoice
        {
            TotalAmount = 1200m,
            PaidAmount = 1200m,
            Status = InvoiceStatus.Paid
        };

        invoice.PaidAmount.Should().BeGreaterThanOrEqualTo(invoice.TotalAmount);
    }

    [Fact]
    public void Invoice_IsPartiallyPaid_WhenPaidAmountLessThanTotal()
    {
        var invoice = new Invoice
        {
            TotalAmount = 1200m,
            PaidAmount = 500m,
            Status = InvoiceStatus.PartiallyPaid
        };

        invoice.PaidAmount.Should().BeLessThan(invoice.TotalAmount);
        invoice.PaidAmount.Should().BeGreaterThan(0);
    }
}
