﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain.ExamComponents
{
    public class SpeakingComponent : IExamComponent
    {
        public SpeakingComponent()
        {
        }
        public void Display()
        {
            Console.WriteLine("Speaking Component");
        }
    }
}
