﻿using HBDStack.Services.Transformation.TokenExtractors;

namespace HBDStack.Services.Transformation;

public interface ITransformerService
{
    #region Methods

    /// <summary>
    /// Transform template from TransformData and additionalData
    /// </summary>
    /// <param name="template">the template ex: Hello [Name]. Your {Email} had been [ApprovedStatus]</param>
    /// <param name="parameters">the additional Data that is not in the sharing data. the value in additionalData will overwrite the value from TransformData as well</param>
    /// <returns>"Hello Duy. Your drunkcoding@outlook.net had been Approved" with TransformData or additionalData is new {Name = "Duy", Email= "drunkcoding@outlook.net", ApprovedStatus = "Approved"}</returns>
    Task<string> TransformAsync(string template, params object[] parameters);

    /// <summary>
    /// Transform template from TransformData and additionalData
    /// </summary>
    /// <param name="template">the template ex: Hello [Name]. Your {Email} had been [ApprovedStatus]</param>
    /// <param name="parameterFactory">Dynamic loading data based on Token <see cref="IToken"/></param>
    /// <returns>"Hello Duy. Your drunkcoding@outlook.net had been Approved" with TransformData or dataProvider is new {Name = "Duy", Email= "drunkcoding@outlook.net", ApprovedStatus = "Approved"}</returns>
    Task<string> TransformAsync(string template, Func<IToken, Task<object>> parameterFactory);

    #endregion Methods
}