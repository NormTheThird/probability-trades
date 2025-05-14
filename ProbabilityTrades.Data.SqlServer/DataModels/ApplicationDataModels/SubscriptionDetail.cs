using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class SubscriptionDetail
{
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public Guid UserId { get; set; }

    public Guid? StripePaymentId { get; set; }

    public Guid? DiscountCodeId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
