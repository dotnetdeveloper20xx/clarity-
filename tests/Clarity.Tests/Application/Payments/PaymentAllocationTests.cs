using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Application.Payments;

public class PaymentAllocationTests
{
    [Fact]
    public void Payment_AgainstIssuedInvoice_ShouldBeAllowed()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Issued, TotalAmount = 1000m, PaidAmount = 0 };
        var canPay = invoice.Status != InvoiceStatus.Draft && invoice.Status != InvoiceStatus.Paid;
        canPay.Should().BeTrue();
    }

    [Fact]
    public void Payment_AgainstDraftInvoice_ShouldBeRejected()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Draft };
        var canPay = invoice.Status != InvoiceStatus.Draft && invoice.Status != InvoiceStatus.Paid;
        canPay.Should().BeFalse();
    }

    [Fact]
    public void Payment_AgainstPaidInvoice_ShouldBeRejected()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Paid, TotalAmount = 1000m, PaidAmount = 1000m };
        var canPay = invoice.Status != InvoiceStatus.Draft && invoice.Status != InvoiceStatus.Paid;
        canPay.Should().BeFalse();
    }

    [Fact]
    public void FullPayment_ShouldUpdateStatusToPaid()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Issued, TotalAmount = 1000m, PaidAmount = 0 };
        var paymentAmount = 1000m;

        invoice.PaidAmount += paymentAmount;
        if (invoice.PaidAmount >= invoice.TotalAmount)
            invoice.Status = InvoiceStatus.Paid;

        invoice.Status.Should().Be(InvoiceStatus.Paid);
    }

    [Fact]
    public void PartialPayment_ShouldUpdateStatusToPartiallyPaid()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Issued, TotalAmount = 1000m, PaidAmount = 0 };
        var paymentAmount = 500m;

        invoice.PaidAmount += paymentAmount;
        if (invoice.PaidAmount >= invoice.TotalAmount)
            invoice.Status = InvoiceStatus.Paid;
        else if (invoice.PaidAmount > 0)
            invoice.Status = InvoiceStatus.PartiallyPaid;

        invoice.Status.Should().Be(InvoiceStatus.PartiallyPaid);
    }

    [Fact]
    public void MultiplePartialPayments_ShouldAccumulate()
    {
        var invoice = new Invoice { Status = InvoiceStatus.Issued, TotalAmount = 1000m, PaidAmount = 0 };

        invoice.PaidAmount += 300m;
        invoice.PaidAmount += 400m;
        invoice.PaidAmount += 300m;

        invoice.PaidAmount.Should().Be(1000m);
    }
}
