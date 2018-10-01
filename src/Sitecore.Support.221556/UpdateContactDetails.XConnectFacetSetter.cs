using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Diagnostics;
using Sitecore.WFFM.Abstractions.Analytics;
using Sitecore.WFFM.Abstractions.Shared;
using Sitecore.Support.WFFM.Actions.XConnectUtils;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using Facet = Sitecore.XConnect.Facet;

namespace Sitecore.Support.WFFM.Actions.SaveActions
{
    public partial class UpdateContactDetails
    {
        /// <summary>
        /// The type which is responsible for setting the facets passed by the saveaction into the current contact details.
        /// </summary>
        class XConnectFacetSetter
        {
            #region Fields

            private readonly IAnalyticsTracker _analyticsTracker;
            private readonly IEnumerable<FacetNode> _facetMapping;

            private XConnect.Client.XConnectClient _xConnectClient;
            private XConnect.Contact _xConnectContact;

            private List<string> _facetNames;

            #endregion

            #region C'tor

            public XConnectFacetSetter(IAnalyticsTracker analyticsTracker, IEnumerable<FacetNode> facetMapping)
            {
                Assert.ArgumentNotNull(analyticsTracker, nameof(analyticsTracker));
                Assert.ArgumentNotNull(facetMapping, nameof(facetMapping));

                this._analyticsTracker = analyticsTracker;
                this._facetMapping = facetMapping;
                this._facetNames = new List<string>();
            }

            #endregion

            #region Public methods

            public void Execute()
            {
                try
                {
                    this.InititlizeFacetNames();

                    this.InititlizeXConnect();
                    

                    if (this._xConnectContact != null)
                    {

                        this.SetFacet<IContactPersonalInfo, PersonalInformation>("Personal", PersonalInformation.DefaultFacetKey);

                        this.SetFacet<IContactAddresses, AddressList>("Addresses", AddressList.DefaultFacetKey);

                        this.SetFacet<IContactEmailAddresses, EmailAddressList>("Emails", EmailAddressList.DefaultFacetKey);

                        this.SetFacet<IContactPhoneNumbers, PhoneNumberList>("Phone Numbers", PhoneNumberList.DefaultFacetKey);

                        this.SetFacet<IContactPicture, Avatar>("Picture", Avatar.DefaultFacetKey);

                        this.SetFacet<IContactCommunicationProfile, ConsentInformation>("Communication Profile", ConsentInformation.DefaultFacetKey);

                        this._xConnectClient.Submit();

                        this.SetFacet<IContactPreferences, PersonalInformation>("Preferences", PersonalInformation.DefaultFacetKey);

                        this._xConnectClient.Submit();
                    }
                }
                catch (XdbExecutionException ex)
                {
                    throw ex;
                }
                finally
                {
                    this._xConnectClient.Dispose();
                }
            }


            #endregion

            #region Private Methods

            void InititlizeXConnect()
            {
                this._xConnectClient = SitecoreXConnectClientConfiguration.GetClient();

                Assert.IsNotNull(_xConnectClient, nameof(_xConnectClient));

                var contactIdentifier = this._analyticsTracker.Current.Contact.Identifiers.FirstOrDefault(t => t.Source == "wffm")?.Identifier;

                var contactReference = new IdentifiedContactReference("wffm", contactIdentifier);

                var expandOptions = new ContactExpandOptions(FacetMapper.MapToXConnectFacets(this._facetNames));

                this._xConnectContact = this._xConnectClient.Get<XConnect.Contact>(contactReference, expandOptions);
            }

            void InititlizeFacetNames()
            {
                if (_facetMapping.Any())
                {
                    this._facetNames = _facetMapping.Select(x => x.Path.Split('/')[0]).ToList();
                }
            }


            private void SetFacet<TTrackerFacet, TXConnectFacet>(string tracketFacetKey, string xConnectFacetKey) where TTrackerFacet : class, IFacet where TXConnectFacet : Facet
            {
                if (this._facetNames.Contains(tracketFacetKey))
                {
                    DoSetFacet<TTrackerFacet, TXConnectFacet>(tracketFacetKey, xConnectFacetKey);
                }
            }

            private void DoSetFacet<TTrackerFacet, TXConnectFacet>(string tracketFacetKey, string xConnectFacetKey) where TTrackerFacet : class, IFacet where TXConnectFacet : Facet
            {
                if (_analyticsTracker.Current.Contact.Facets.ContainsKey(tracketFacetKey))
                {
                    var trackerFacet = _analyticsTracker.Current.Contact.GetFacet<TTrackerFacet>(tracketFacetKey);

                    var xConnectFacet = _xConnectContact.GetFacet<TXConnectFacet>();

                    var converter = Sitecore.Support.WFFM.Actions.XConnectUtils.XConnectFacetCopierFactory.CreateConverter<TTrackerFacet, TXConnectFacet>(tracketFacetKey, trackerFacet, xConnectFacetKey, xConnectFacet, _facetMapping);

                    Assert.IsNotNull(converter, nameof(converter));

                    xConnectFacet = converter.ToXConnectFacet();

                    this._xConnectClient.SetFacet(this._xConnectContact, xConnectFacetKey, xConnectFacet);
                }
            }

            #endregion
        }
    }
}
