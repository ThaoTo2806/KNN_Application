using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDiabetes.Models
{
    public class ClassDiabetes
    {
        public List<double> Attributes { get; set; }
        public int Val { get; set; }
        public double Distance { get; set; }

        public ClassDiabetes()
        {
            Attributes = new List<double>();
        }
    }
}