using System.Collections;
using System.Reflection;
using HBDStack.Services.Transformation.TokenExtractors;

namespace HBDStack.Services.Transformation.TokenResolvers;

public class TokenResolver : ITokenResolver
{
    #region Methods

    /// <summary>
    /// Get the first not null value of the public property of data.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">if data or token is null</exception>
    public virtual object Resolve(IToken token, params object[] data)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        if (data == null || data.Length <= 0)
            throw new ArgumentNullException(nameof(data));

        var propertyName = token.Key;

        foreach (var obj in data)
        {
            if (obj == null) continue;

            object value;

            if (obj is IDictionary)
            {
                if (!(obj is IDictionary<string, object>))
                    throw new ArgumentException("Only IDictionary<string,object> is supported");
                var d = (IDictionary<string, object>) obj;

                var key = d.Keys.FirstOrDefault(k => k.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                value = key != null ? d[key] : null;
            }
            else
            {
                value = GetProperty(obj, propertyName)?.GetValue(obj);
            }

            if (value != null) return value;
        }

        return null;
    }

    public Task<object> ResolveAsync(IToken token, params object[] data)
    {
        return Task.Run(() => Resolve(token, data));
    }

    protected virtual PropertyInfo GetProperty(object data, string propertyName)
    {
        return data.GetType().GetProperty(propertyName,
                   BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public)

               //BindingFlags.NonPublic and BindingFlags.Public are not work together
               ?? data.GetType().GetProperty(propertyName,
                   BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
    }

    #endregion Methods
}