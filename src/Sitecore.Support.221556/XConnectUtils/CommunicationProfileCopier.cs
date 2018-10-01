using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers
{
  class CommunicationProfileCopier : DefaultXConnectFacetCopier<IContactCommunicationProfile, ConsentInformation>
  {
    public CommunicationProfileCopier(string tracketFacetKey, IContactCommunicationProfile trackerFacet, string xConnectFacetKey, ConsentInformation xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {
    }

    public override ConsentInformation ToXConnectFacet()
    {
      var xConnectFacet = _xConnectFacet ?? new ConsentInformation();
      xConnectFacet.ConsentRevoked = _trackerFacet.ConsentRevoked;
      xConnectFacet.DoNotMarket = _trackerFacet.CommunicationRevoked;

      return xConnectFacet;
    }
  }
}
