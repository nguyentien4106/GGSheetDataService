using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Models.AttMachine
{
    public static class UserPrivilege
    {
        public static readonly Dictionary<int, string> UserPrivileges = new Dictionary<int, string>()
        {
            { 0, "Common User" },
            { 1, "Registrar User" },
            { 2, "Adminstrator" },
            { 3, "Super Administrator" },
        };


        public static string GetUserPrivilegeName(int code) => UserPrivileges.ContainsKey(code) ? UserPrivileges[code] : UserPrivileges[0];
    }
}
