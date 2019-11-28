using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class JsonDataModel<T>
    {
        public T Data { get; set; }
    }

    public class ListItem
    {
        public int value { get; set; }
        public string display { get; set; }

        public ListItem() { }

        public ListItem(StaticText staticText) 
        {
            this.value = staticText.Id;
            this.display = staticText.Title;
        }
    }
}
