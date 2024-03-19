using System.DirectoryServices;

namespace PerformansYonetimSistemi.Models.Ldap
{
    public class LDAPHelper
    {
        public string LDAPPath { get; set; }
        public bool IsAuthenticated(DomainUser user)
        {
            bool result = false;
            try
            {
                string fullName = string.Format("{0}\\{1}", user.Domain, user.Name);
                DirectoryEntry entry = new DirectoryEntry(LDAPPath, fullName, user.Password);
                DirectorySearcher voyager = new DirectorySearcher(entry);
                voyager.Filter = string.Format("(sAMAccountName={0})", user.Name);
                voyager.PropertiesToLoad.Add("CN");
                SearchResult sR = voyager.FindOne();
                if (sR != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            return result;
        }
    }
}
