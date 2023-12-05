using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{

        internal class SearchCriteria
        {
            public SearchCriteria()
            {
                Name = "";
                TimeHandling = "";
                Faculty = "";
                Department = "";
                ClassMonitor = "";
                Starosta = "";
             
        }

        public string Name { get; set; }
        public string TimeHandling { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string ClassMonitor { get; set; }
        public string Starosta { get; set; }

    }

}
