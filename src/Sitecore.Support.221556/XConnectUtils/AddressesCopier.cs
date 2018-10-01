using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers
{
  class AddressesCopier : DefaultXConnectFacetCopier<IContactAddresses, AddressList>
  {
    public AddressesCopier(string tracketFacetKey, IContactAddresses trackerFacet, string xConnectFacetKey, AddressList xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {

    }

    public override AddressList ToXConnectFacet()
    {
      if (TrackerFacetHasPreferred())
      {
        var preferred = _trackerFacet.Entries[_trackerFacet.Preferred];

        var xConectPreferred = ConvertToXConnect(preferred, _xConnectFacet?.PreferredAddress);

        if (_xConnectFacet == null)
        {
          return new AddressList(xConectPreferred, _trackerFacet.Preferred);
        }
        else
        {
          _xConnectFacet.PreferredAddress = xConectPreferred;
          _xConnectFacet.PreferredKey = _trackerFacet.Preferred;
        }

      }
      return _xConnectFacet;
    }

    #region Private Methods

    private Address ConvertToXConnect(IAddress trackerAddress, Address xConnectAddress)
    {
      xConnectAddress = xConnectAddress ?? new Address();

      base.CopyAttribute(trackerAddress, "StreetLine1", xConnectAddress, "AddressLine1");
      base.CopyAttribute(trackerAddress, "StreetLine2", xConnectAddress, "AddressLine2");
      base.CopyAttribute(trackerAddress, "StreetLine3", xConnectAddress, "AddressLine3");
      base.CopyAttribute(trackerAddress, "StreetLine4", xConnectAddress, "AddressLine4");

      base.CopyAttribute(trackerAddress, "City", xConnectAddress, "City");
      base.CopyAttribute(trackerAddress, "Country", xConnectAddress, "CountryCode");
      base.CopyAttribute(trackerAddress, "PostalCode", xConnectAddress, "PostalCode");
      base.CopyAttribute(trackerAddress, "StateProvince", xConnectAddress, "StateOrProvince");

      xConnectAddress.GeoCoordinate = ConvertToXConnect(trackerAddress.Location);

      return xConnectAddress;
    }

    private GeoCoordinate ConvertToXConnect(IGeographicCoordinate trackerGeo)
    {
      return new GeoCoordinate(trackerGeo.Latitude, trackerGeo.Longitude);
    }

    private bool TrackerFacetHasPreferred()
    {
      return !string.IsNullOrEmpty(_trackerFacet.Preferred) && _trackerFacet.Entries[_trackerFacet.Preferred] != null;
    }

    #endregion
  }
}
