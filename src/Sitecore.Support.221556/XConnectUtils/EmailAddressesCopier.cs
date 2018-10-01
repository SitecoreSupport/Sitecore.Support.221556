using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers
{
  class EmailAddressesCopier : DefaultXConnectFacetCopier<IContactEmailAddresses, EmailAddressList>
  {
    public EmailAddressesCopier(string tracketFacetKey, IContactEmailAddresses trackerFacet, string xConnectFacetKey, EmailAddressList xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {
    }

    public override EmailAddressList ToXConnectFacet()
    {
      if (TrackerFacetHasPreferred())
      {
        var preferred = _trackerFacet.Entries[_trackerFacet.Preferred];

        var xConectPreferred = ConvertToXConnect(preferred, _xConnectFacet?.PreferredEmail);

        if (_xConnectFacet == null)
        {
          return new EmailAddressList(xConectPreferred, _trackerFacet.Preferred);
        }
        else
        {
          _xConnectFacet.PreferredEmail = xConectPreferred;
          _xConnectFacet.PreferredKey = _trackerFacet.Preferred;
        }

      }
      return _xConnectFacet;
    }

    #region Private Methods

    private EmailAddress ConvertToXConnect(IEmailAddress trackerEmailAddress, EmailAddress xConnectEmailAddress)
    {
      return new EmailAddress(trackerEmailAddress.SmtpAddress, false);
    }

    private bool TrackerFacetHasPreferred()
    {
      return !string.IsNullOrEmpty(_trackerFacet.Preferred) && _trackerFacet.Entries[_trackerFacet.Preferred] != null;
    }

    #endregion
  }
}
