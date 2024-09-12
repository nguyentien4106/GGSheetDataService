using DataService.Models.AttMachine;

namespace DataService.Web.Models.Employee
{
    public class AddNewEmployeeViewModel
    {
        public Dictionary<int, string> EmployeePrivileges => UserPrivilege.UserPrivileges;
    }
}
