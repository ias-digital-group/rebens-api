using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Address
    {
        public string GetFullAddress()
        {
            if(!string.IsNullOrEmpty(this.Street) && !string.IsNullOrEmpty(this.Number) && !string.IsNullOrEmpty(this.Neighborhood) 
                && !string.IsNullOrEmpty(this.City) && !string.IsNullOrEmpty(this.State) && !string.IsNullOrEmpty(this.Zipcode))
                return $"{this.Street}, {this.Number} - {this.Neighborhood}, {this.City} - {this.State}, {this.Zipcode}";
            return "";
        }
    }
}
