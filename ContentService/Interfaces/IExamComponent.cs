using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;

namespace ContentService.Interfaces
{
    public interface IExamComponent
    {
        string id { get; set; }
        string? ExamId { get; set; }
        ComponentType GetComponentType();
    }
}
