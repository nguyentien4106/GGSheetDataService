using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Core.Models.AttMachine
{
    public static class VerifyMethod
    {
        private static readonly Dictionary<int, string> _methods = new Dictionary<int, string>()
        {
            { 0, "FP_OR_PW_OR_RF" },
            { 1, "FP" },
            { 2, "PIN" },
            { 3, "PW" },
            { 4, "RF" },
            { 5, "FP_OR_PW" },
            { 6, "FP_OR_RF" },
            { 7, "PW_OR_RF" },
            { 8, "PIN_AND_FP" },
            { 9, "FP_AND_PW" },
            { 10, "FP_AND_RF" },
            { 11, "PW_AND_RF" },
            { 12, "FP_AND_PW_AND_RF" },
            { 13, "PIN_AND_FP_AND_PW" },
            { 14, "FP_AND_RF_OR_PIN" },
        };

        public static string GetVerifyMethod(int code) => _methods.ContainsKey(code) ? _methods[code] : _methods[0];
    }
}
