namespace ProbabilityTrades.Common.Extensions;

public static class IntegerExtensions
{
    public static string GetColumnName(this int column)
    {
        column--;
        var col = Convert.ToString((char)('A' + (column % 26)));
        while (column >= 26)
        {
            column = (column / 26) - 1;
            col = Convert.ToString((char)('A' + (column % 26))) + col;
        }
        return col;
    }
}