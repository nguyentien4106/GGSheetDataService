
namespace DataService.Core.Models.AttMachine
{
    public static class AttState
    {
        public static Dictionary<int, string> VeriryStates = new() 
        {
            { 0, "Check in" },
            { 1, "Check out" },
            { 2, "Break out" },
            { 3, "Break in" },
            { 4, "OT in" },
            { 5, "OT out" },
        };

        public static string GetAttState(int attState) => VeriryStates.ContainsKey(attState) ? VeriryStates[attState] : VeriryStates[0];
    }
}
