using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentService.Domain
{
    public enum CEFRLevel
    {
        A1,
        A2,
        B1,
        B2,
        C1,
        C2
    }

    public static class CEFRLevelExtensions
    {
        private static readonly Dictionary<CEFRLevel, string> CEFRLevelNames = new()
        {
            { CEFRLevel.A1, "A1 - beginner" },
            { CEFRLevel.A2, "A2 - elementary" },
            { CEFRLevel.B1, "B1 - intermediate" },
            { CEFRLevel.B2, "B2 - upper-intermediate" },
            { CEFRLevel.C1, "C1 - advanced" },
            { CEFRLevel.C2, "C2 - proficiency" }
        };

        public static string GetName(this CEFRLevel level)
        {
            return CEFRLevelNames[level];
        }
    }
}
