using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Support.Form.Submit
{
  using Sitecore.Form.Submit;
  using Sitecore.Security.Authentication;
  using Sitecore.WFFM.Abstractions.Actions;

  public class LoginUserWithPassword : Sitecore.Support.Form.Submit.LoginUserWithoutPassword
  {
    protected override bool LoginUser(string userName, AdaptedResultList fields)
    {
      AdaptedControlResult entry = fields.GetEntry(base.PasswordField, "Password");
      string password = (entry != null) ? (entry.Value ?? string.Empty) : string.Empty;
      return AuthenticationManager.Login(userName, password, true);
    }
  }

}