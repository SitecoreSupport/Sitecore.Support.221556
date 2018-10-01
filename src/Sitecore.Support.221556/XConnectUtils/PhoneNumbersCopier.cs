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
  class PhoneNumbersCopier : DefaultXConnectFacetCopier<IContactPhoneNumbers, PhoneNumberList>
  {
    public PhoneNumbersCopier(string tracketFacetKey, IContactPhoneNumbers trackerFacet, string xConnectFacetKey, PhoneNumberList xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {
    }

    public override PhoneNumberList ToXConnectFacet()
    {
      if (TrackerFacetHasPreferred())
      {
        var preferred = _trackerFacet.Entries[_trackerFacet.Preferred];

        var xConectPreferred = ConvertToXConnect(preferred, _xConnectFacet?.PreferredPhoneNumber);

        if (_xConnectFacet == null)
        {
          return new PhoneNumberList(xConectPreferred, _trackerFacet.Preferred);
        }
        else
        {
          _xConnectFacet.PreferredPhoneNumber = xConectPreferred;
          _xConnectFacet.PreferredKey = _trackerFacet.Preferred;
        }

      }
      return _xConnectFacet;
    }

    #region Private Methods

    private PhoneNumber ConvertToXConnect(IPhoneNumber trackerPhone, PhoneNumber xConnectPhone)
    {
      return new PhoneNumber(trackerPhone.CountryCode, trackerPhone.Number)
      {
        Extension = trackerPhone.Extension
      };
    }

    private bool TrackerFacetHasPreferred()
    {
      return !string.IsNullOrEmpty(_trackerFacet.Preferred) && _trackerFacet.Entries[_trackerFacet.Preferred] != null;
    }

    #endregion
  }
}
