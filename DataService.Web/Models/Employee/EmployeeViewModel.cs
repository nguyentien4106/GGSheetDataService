
namespace DataService.Web.Models.Employee
{
    public class EmployeeViewModel(DataService.Infrastructure.Entities.Employee employee)
    {
        public int Id = employee.Id;

        public string Pin = employee.Pin;

        public string Name = employee.Name;

        public string Password = employee.Password;

        public string CardNumber = employee.CardNumber;

        public string Privilege = DataService.Models.AttMachine.UserPrivilege.GetUserPrivilegeName(employee.Privilege);
    }
}
