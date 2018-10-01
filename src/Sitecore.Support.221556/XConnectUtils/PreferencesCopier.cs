using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics.Model.Entities;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers
{
  class PreferencesCopier : DefaultXConnectFacetCopier<IContactPreferences, PersonalInformation>
  {
    public PreferencesCopier(string tracketFacetKey, IContactPreferences trackerFacet, string xConnectFacetKey, PersonalInformation xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {
    }

    public override PersonalInformation ToXConnectFacet()
    {
      var xConnectFacet = _xConnectFacet ?? new PersonalInformation();
      xConnectFacet.PreferredLanguage = _trackerFacet.Language;
      return xConnectFacet;
    }
  }
}
