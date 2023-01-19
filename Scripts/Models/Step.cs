using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storyder.Scripts.Models
{
    public class Step
    {

        public int Id { get; set; }

        public string Content { get; set; }

        public string ImagePath { get; set; }

        public List<int> NextStepIds { get; set; } = new List<int>();

        public int NextStepId()
        {
            return NextStepId(0);
        }

        public int NextStepId(int choice)
        {
            return NextStepIds[choice];
        }
    }
}
