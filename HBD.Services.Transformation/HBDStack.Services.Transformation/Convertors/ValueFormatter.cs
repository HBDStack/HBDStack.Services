using HBDStack.Services.Transformation.TokenExtractors;

namespace HBDStack.Services.Transformation.Convertors;

/// <summary>
/// The convertor will be use to convert object to string before replace to the template.
/// </summary>
public class ValueFormatter : IValueFormatter
{
    #region Properties

    public string DateFormat { get; set; } = "dd/MM/yyyy hh.mm.ss";

    public string IntegerFormat { get; set; } = "###,##0";

    public string NumberFormat { get; set; } = "###,##0.00";

    #endregion Properties

    #region Methods

    public virtual string Convert(IToken token, object value)
    {
        if (value == null)
            return string.Empty;

        switch (value)
        {
            case bool b: return b ? "Yes" : "No";
            case int a: return a.ToString(IntegerFormat);
            case long a: return a.ToString(IntegerFormat);
            case double a: return a.ToString(NumberFormat);
            case decimal a: return a.ToString(NumberFormat);
            case float a: return a.ToString(NumberFormat);
            case DateTime a: return a.ToString(DateFormat);
            case DateTimeOffset a: return a.ToString(DateFormat);
        }

        return value.ToString();
    }

    #endregion Methods
}