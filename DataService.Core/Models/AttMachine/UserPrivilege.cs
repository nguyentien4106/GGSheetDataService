using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Models.AttMachine
{
    public static class UserPrivilege
    {
        private static readonly Dictionary<int, string> _userPrivileges = new Dictionary<int, string>()
        {
            { 0, "Common User" },
            { 1, "Registrar User" },
            { 2, "Adminstrator" },
            { 3, "Super Administrator" },
        };

        public static string GetUserPrivilegeName(int code) => _userPrivileges.ContainsKey(code) ? _userPrivileges[code] : _userPrivileges[0];
    }
}
