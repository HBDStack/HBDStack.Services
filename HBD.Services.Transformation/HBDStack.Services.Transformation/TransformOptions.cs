using HBDStack.Services.Transformation.Convertors;
using HBDStack.Services.Transformation.TokenExtractors;
using HBDStack.Services.Transformation.TokenResolvers;

namespace HBDStack.Services.Transformation;

public class TransformOptions
{
    #region Properties

    /// <summary>
    /// The token value will be cached internally for subsequence use. If don't want the value to be cached. You can disable the cache here.
    /// </summary>
    public bool DisabledLocalCache { get; set; } = true;

    /// <summary>
    /// The <see cref="IValueFormatter"/> to format the value of Token before apply to the template.
    /// </summary>
    public IValueFormatter Formatter { get; set; }

    /// <summary>
    /// The <see cref="ITokenExtractor"/> for templates.
    /// </summary>
    public IList<ITokenExtractor> TokenExtractors { get; } = new List<ITokenExtractor>();

    /// <summary>
    /// The <see cref="ITokenResolver"/> for all <see cref="IToken"/>
    /// </summary>
    public ITokenResolver TokenResolver { get; set; }

    /// <summary>
    /// Global Data object that share to all transforming.
    /// There are some global data which sharing across application shall be configure here when app start
    /// </summary>
    public object[] GlobalParameters { get; set; }

    #endregion Properties
}