using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Data;
using Sitecore.Security.Accounts;
using Sitecore.WFFM.Abstractions.Actions;
using Sitecore.WFFM.Abstractions.Dependencies;

namespace Sitecore.Support.Form.Submit
{
  public class LoginUserWithoutPassword : Sitecore.Form.Submit.LoginUserWithoutPassword
  {
    private ID formID;

    public override void Execute(ID formId, AdaptedResultList adaptedFields, ActionCallContext actionCallContext = null, params object[] data)
    {
      this.formID = formId;
      AdaptedControlResult entry = adaptedFields.GetEntry(base.UserNameField, "User name");
      if (entry == null)
      {
        DependenciesManager.Logger.Warn("The Login User action: the user name is not set.", this);
      }
      string userNameIfExist = base.GetUserNameIfExist(entry.Value);
      if (!string.IsNullOrEmpty(userNameIfExist))
      {

        if (this.LoginUser(userNameIfExist, adaptedFields))
        {
          this.UpdateGlobalSession(userNameIfExist);
          this.UpdateAudit(formId, User.FromName(userNameIfExist, true));
        }
      }
    }

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