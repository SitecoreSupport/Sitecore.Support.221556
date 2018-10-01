using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.Analytics.Model.Framework;
using Sitecore.WFFM.Abstractions.Analytics;

namespace Sitecore.Support.WFFM.Abstractions.XConnect
{
  public abstract class DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet> where TTrackerFacet : class, IFacet where TXConnectFacet : Sitecore.XConnect.Facet
  {
    #region Fields

    protected readonly string _tracketFacetKey;
    protected readonly TTrackerFacet _trackerFacet;

    protected readonly string _xConnectFacetKey;
    protected readonly TXConnectFacet _xConnectFacet;

    protected readonly IEnumerable<FacetNode> _facetMapping;

    #endregion

    #region C'tors

    public DefaultXConnectFacetCopier(string tracketFacetKey, TTrackerFacet trackerFacet, string xConnectFacetKey, TXConnectFacet xConnectFacet, IEnumerable<FacetNode> facetMapping)
    {
      _tracketFacetKey = tracketFacetKey;
      _trackerFacet = trackerFacet;

      _xConnectFacetKey = xConnectFacetKey;
      _xConnectFacet = xConnectFacet;

      _facetMapping = facetMapping;
    }

    #endregion

    #region Abstract Methods

    public abstract TXConnectFacet ToXConnectFacet();

    #endregion

    #region Public Methods

    protected virtual void CopyAttribute(object trackerObject, string trackerAttributeName, object xConnectObject, string xConnectAttributeName)
    {
      if (this.ShouldCopyAttribute(trackerAttributeName))
      {
        var trackerAttribute = trackerObject.GetType().GetProperty(trackerAttributeName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        var xconnectAttribute = xConnectObject.GetType().GetProperty(xConnectAttributeName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (trackerAttribute != null && xconnectAttribute != null)
        {
          xconnectAttribute.SetValue(xConnectObject, System.Convert.ChangeType(trackerAttribute.GetValue(trackerObject), xconnectAttribute.PropertyType));
        }
      }
    }

    #endregion

    #region Private Methods

    private bool ShouldCopyAttribute(string attributeName)
    {
      return this._facetMapping.Any(map => map.Path.StartsWith(_tracketFacetKey, StringComparison.InvariantCultureIgnoreCase) && map.Path.EndsWith(attributeName, StringComparison.InvariantCultureIgnoreCase));
    }


    #endregion
  }
}