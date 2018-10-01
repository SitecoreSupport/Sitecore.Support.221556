using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils
{
  public class XConnectFacetCopierFactory
  {
    public static DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet> CreateConverter<TTrackerFacet, TXConnectFacet>(string tracketFacetKey, TTrackerFacet trackerFacet, string xConnectFacetKey, TXConnectFacet xConnectFacet, IEnumerable<FacetNode> facetMapping)
        where TTrackerFacet : class, IFacet
        where TXConnectFacet : Sitecore.XConnect.Facet
    {

      if (trackerFacet is IContactPersonalInfo)
      {
        return new PersonalInfoCopier(tracketFacetKey, trackerFacet as IContactPersonalInfo, xConnectFacetKey, xConnectFacet as PersonalInformation, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      if (trackerFacet is IContactAddresses)
      {
        return new AddressesCopier(tracketFacetKey, trackerFacet as IContactAddresses, xConnectFacetKey, xConnectFacet as AddressList, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      if (trackerFacet is IContactEmailAddresses)
      {
        return new EmailAddressesCopier(tracketFacetKey, trackerFacet as IContactEmailAddresses, xConnectFacetKey, xConnectFacet as EmailAddressList, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      if (trackerFacet is IContactPhoneNumbers)
      {
        return new PhoneNumbersCopier(tracketFacetKey, trackerFacet as IContactPhoneNumbers, xConnectFacetKey, xConnectFacet as PhoneNumberList, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      if (trackerFacet is IContactPicture)
      {
        return new PictureCopier(tracketFacetKey, trackerFacet as IContactPicture, xConnectFacetKey, xConnectFacet as Avatar, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      if (trackerFacet is IContactCommunicationProfile)
      {
        return new CommunicationProfileCopier(tracketFacetKey, trackerFacet as IContactCommunicationProfile, xConnectFacetKey, xConnectFacet as ConsentInformation, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      if (trackerFacet is IContactPreferences)
      {
        return new PreferencesCopier(tracketFacetKey, trackerFacet as IContactPreferences, xConnectFacetKey, xConnectFacet as PersonalInformation, facetMapping) as DefaultXConnectFacetCopier<TTrackerFacet, TXConnectFacet>;
      }

      return null;
    }
  }
}
