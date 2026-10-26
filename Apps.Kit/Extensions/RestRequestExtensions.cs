using System.Globalization;
using Apps.Kit.Constants;
using RestSharp;

namespace Apps.Kit.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest AddOptionalQueryParameter(this RestRequest request, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            request.AddQueryParameter(name, value);

        return request;
    }

    public static RestRequest AddOptionalDateQueryParameter(this RestRequest request, string name, DateTime? value) =>
        request.AddOptionalQueryParameter(name, value?.ToString(QueryParameters.DateFormat, CultureInfo.InvariantCulture));

    public static RestRequest AddDateTimeQueryParameter(this RestRequest request, string name, DateTime value) =>
        request.AddQueryParameter(name, value.ToString(QueryParameters.DateTimeFormat, CultureInfo.InvariantCulture));
}
