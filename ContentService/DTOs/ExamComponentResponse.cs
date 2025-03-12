using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.DTOs
{
    public class ExamComponentResponse
    {
        public IExamComponent ExamComponent { get; set; }

        public ExamComponentResponse(IExamComponent examComponent)
        {
            ExamComponent = examComponent;
        }
    }
}
