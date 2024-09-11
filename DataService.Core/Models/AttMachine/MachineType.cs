namespace DataService.Core.Models.AttMachine
{
    public static class MachineType
    {
        private static Dictionary<int, string> _machineTypes = new()
        {
            { 0, "Black & White" },
            { 1, "TFT" },
            { 2, "Face" },
        };

        public static string GetMachineType(int attState)
        {
            return _machineTypes.ContainsKey(attState) ? _machineTypes[attState] : _machineTypes[0];
        }

        public static List<string> GetMachineTypes() => _machineTypes.Values.ToList();   
    }   
}
