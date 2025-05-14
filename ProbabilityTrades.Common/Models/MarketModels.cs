namespace ProbabilityTrades.Common.Models;

public class MarketBaseModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string DataSource { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class MarketModel : MarketBaseModel, ICloneable
{
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public string LastChangedBy { get; set; } = string.Empty;
    public DateTimeOffset DateLastChanged { get; set; } = DateTime.MinValue;
    public DateTimeOffset DateCreated { get; set; } = DateTime.MinValue;

    public object Clone()
    {
        return (MarketModel)MemberwiseClone();
    }
}