using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.WFFM.Abstractions;
using Sitecore.WFFM.Abstractions.Actions;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.WFFM.Abstractions.Shared;
using Sitecore.WFFM.Actions.Base;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Sitecore.WFFM.Actions.SaveActions
{
  [Required("IsXdbTrackerEnabled", true)]
  public class UpdateContactDetails : WffmSaveAction
  {
    private readonly IAnalyticsTracker analyticsTracker;

    private readonly IAuthentificationManager authentificationManager;

    private readonly ILogger logger;

    private readonly IFacetFactory facetFactory;

    public string Mapping
    {
      get;
      set;
    }

    public UpdateContactDetails(IAnalyticsTracker analyticsTracker, IAuthentificationManager authentificationManager, ILogger logger, IFacetFactory facetFactory)
    {
      Assert.IsNotNull(analyticsTracker, "analyticsTracker");
      Assert.IsNotNull(authentificationManager, "authentificationManager");
      Assert.IsNotNull(logger, "logger");
      Assert.IsNotNull(facetFactory, "facetFactory");
      this.analyticsTracker = analyticsTracker;
      this.authentificationManager = authentificationManager;
      this.logger = logger;
      this.facetFactory = facetFactory;
    }

    public override void Execute(ID formId, AdaptedResultList adaptedFields, ActionCallContext actionCallContext = null, params object[] data)
    {
      UpdateContact(adaptedFields);
    }

    protected virtual void UpdateContact(AdaptedResultList fields)
    {
      Assert.ArgumentNotNull(fields, "adaptedFields");
      Assert.IsNotNullOrEmpty(Mapping, "Empty mapping xml.");
      Assert.IsNotNull(analyticsTracker.CurrentContact, "Tracker.Current.Contact");
      if (!authentificationManager.IsActiveUserAuthenticated)
      {
        logger.Warn("[UPDATE CONTACT DETAILS Save action] User is not authenticated to edit contact details.", this);
      }
      else
      {
        IEnumerable<FacetNode> enumerable = ParseMapping(Mapping, fields);
        IContactFacetFactory contactFacetFactory = facetFactory.GetContactFacetFactory();
        foreach (FacetNode item in enumerable)
        {
          contactFacetFactory.SetFacetValue(analyticsTracker.CurrentContact, item.Key, item.Path, item.Value, true);
        }
      }
    }

    public IEnumerable<FacetNode> ParseMapping(string mapping, AdaptedResultList adaptedFieldResultList)
    {
      Assert.ArgumentNotNullOrEmpty(mapping, "mapping");
      Assert.ArgumentNotNull(adaptedFieldResultList, "adaptedFieldResultList");
      return (from Dictionary<string, object> item in (object[])new JavaScriptSerializer().Deserialize(mapping, typeof(object))
              let itemValue = item["value"].ToString()
              let itemId = (item.ContainsKey("id") && item["id"] != null) ? item["id"].ToString() : "Preferred"
              let value = adaptedFieldResultList.GetValueByFieldID(ID.Parse(item["key"].ToString()))
              where !string.IsNullOrEmpty(value)
              select new FacetNode(itemId, itemValue, value)).ToList();
    }
  }
}