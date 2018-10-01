using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics;

namespace Sitecore.Support.Form.Submit
{
  class CreateUser : Sitecore.Form.Submit.CreateUser
  {
    protected override void UpdateGlobalSession(string userName)
    {
      if (this.AssociateUserWithVisitor && Tracker.Current != null && Tracker.Current.Session != null)
      {
        if (Tracker.Current.Contact.Identifiers != null && Tracker.Current.Contact.Identifiers.FirstOrDefault(x => x.Source == "wffm") != null)
        {
          return;
        }
        Tracker.Current.Session.IdentifyAs("wffm", userName);
      }
    }
  }
}