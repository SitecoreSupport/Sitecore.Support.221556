using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.WFFM.Abstractions.Shared;
using Sitecore.WFFM.Analytics;

namespace Sitecore.Support.WFFM.Analytics.Dependencies
{
  public class DefaultImplFacetFactory : IFacetFactory
  {
    public IContactFacetFactory GetContactFacetFactory()
    {
      return new Sitecore.Support.WFFM.Analytics.ContactFacetFactory();
    }
  }
}