using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Models.AttMachine
{
    public static class AttState
    {
        private static Dictionary<int, string> _attStates = new() 
        {
            { 0, "Check in" },
            { 1, "Check out" },
            { 2, "Break out" },
            { 3, "Break in" },
            { 4, "OT in" },
            { 5, "OT out" },
        };

        public static string GetAttState(int attState)
        {
            return _attStates.ContainsKey(attState) ? _attStates[attState] : _attStates[0];
        }
    }
}
