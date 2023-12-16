using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDiabetes
{
    public class Utilities
    {
        #region Singleton pattern
        private static Utilities instance;
        public static Utilities Instance
        {
            get { if (instance == null) instance = new Utilities(); return instance; }
            private set { instance = value; }
        }
        private Utilities() { }
        #endregion

        public bool ConvertDouble(string resultString, out double result)
        {
            if (double.TryParse(resultString.Replace('.',','), out result))
                return true;
            return false;
        }    
    }
}