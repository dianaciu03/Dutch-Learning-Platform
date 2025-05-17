using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;

namespace ContentService.Helpers
{
    public static class EnumConverter
    {
        public static ComponentType ParseComponentType(string type)
        {
            if (Enum.TryParse(type, true, out ComponentType result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid exam type: {type}");
        }

        public static CEFRLevel ParseCEFRLevel(string level)
        {
            if (Enum.TryParse(level, true, out CEFRLevel result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid CEFR level: {level}");
        }
    }
}
