using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers
{
  class PictureCopier : DefaultXConnectFacetCopier<IContactPicture, Avatar>
  {
    public PictureCopier(string tracketFacetKey, IContactPicture trackerFacet, string xConnectFacetKey, Avatar xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {
    }

    public override Avatar ToXConnectFacet()
    {
      return new Avatar(_trackerFacet.MimeType, _trackerFacet.Picture);
    }
  }
}
