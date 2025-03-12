using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;

namespace ContentService.DTOs
{
    public class ExamResponse
    {
        public List<ExamPractice>? ExamList { get; set; }
    }
}
