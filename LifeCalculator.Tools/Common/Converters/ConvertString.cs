using System;

namespace LifeCalculator.Tools.Common.Converters
{
    public static class ConvertString
    {
        public static double ToDouble(string str)
        {
            try{
                return Convert.ToDouble(str);
            }
            catch{
                return 0.0;
            }
        }
    }
}
