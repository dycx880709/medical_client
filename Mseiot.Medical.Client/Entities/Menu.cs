using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Medical.Client.Entities
{
    public class Menu
    {
        public string Name { get; set; }
        public string Identify { get; set; }
        public bool Reusability { get; set; }
        public List<Menu> Children { get; set; } = new List<Menu>();

        public bool HasChildren 
        {
            get
            {
                if (Children == null || Children.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
