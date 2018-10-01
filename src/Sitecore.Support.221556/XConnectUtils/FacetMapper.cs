using System.Collections.Generic;
using System.Linq;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils
{
  public class FacetMapper
  {
    private static Dictionary<string, string> map = new Dictionary<string, string>()
        {
            {"Personal", PersonalInformation.DefaultFacetKey},
            {"Addresses", AddressList.DefaultFacetKey},
            {"Emails", EmailAddressList.DefaultFacetKey},
            {"Phone Numbers", PhoneNumberList.DefaultFacetKey},
            {"Picture", Avatar.DefaultFacetKey},
            {"Communication Profile", ConsentInformation.DefaultFacetKey},
            {"Preferences", PersonalInformation.DefaultFacetKey}
        };

    internal static string[] MapToXConnectFacets(List<string> trackerFacetNames)
    {
      return trackerFacetNames?.Select(f => map[f]).ToArray();
    }
  }
}
