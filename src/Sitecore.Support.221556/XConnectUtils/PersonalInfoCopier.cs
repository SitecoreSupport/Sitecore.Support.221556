using System.Collections.Generic;
using Sitecore.Analytics.Model.Entities;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.Support.WFFM.Abstractions.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Support.WFFM.Actions.XConnectUtils.FacetCopiers
{
  class PersonalInfoCopier : DefaultXConnectFacetCopier<IContactPersonalInfo, PersonalInformation>
  {
    public PersonalInfoCopier(string tracketFacetKey, IContactPersonalInfo trackerFacet, string xConnectFacetKey, PersonalInformation xConnectFacet, IEnumerable<FacetNode> facetMapping) : base(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, facetMapping)
    {
    }

    public override PersonalInformation ToXConnectFacet()
    {
      var xConnectFacet = _xConnectFacet ?? new PersonalInformation();

      xConnectFacet.Birthdate = _trackerFacet.BirthDate;

      xConnectFacet.FirstName = _trackerFacet.FirstName;

      xConnectFacet.Gender = _trackerFacet.Gender;

      xConnectFacet.JobTitle = _trackerFacet.JobTitle;

      xConnectFacet.LastName = _trackerFacet.Surname;

      xConnectFacet.MiddleName = _trackerFacet.MiddleName;

      xConnectFacet.Nickname = _trackerFacet.Nickname;

      xConnectFacet.Suffix = _trackerFacet.Suffix;

      xConnectFacet.Title = _trackerFacet.Title;

      return xConnectFacet;
    }
  }
}