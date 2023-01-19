using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storyder.Scripts.Models
{
    public class Story
    {

        public IList<Step> Steps { get; set; }

        public string Title { get; set; }

    }
}
