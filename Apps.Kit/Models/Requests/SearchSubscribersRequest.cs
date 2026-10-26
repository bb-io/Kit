using Apps.Kit.DataSourceHandlers.StaticDataHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Kit.Models.Requests;

public class SearchSubscribersRequest
{
    [Display("Status")]
    [StaticDataSource(typeof(SubscriberStatusDataHandler))]
    public string? Status { get; set; }

    [Display("Email address")]
    public string? EmailAddress { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }
}
