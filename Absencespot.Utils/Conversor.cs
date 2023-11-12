using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Utils
{
    public static class Conversor
    {
        public static string ToBase64(string text)
        {
            try
            {
                var encodedText = Encoding.ASCII.GetBytes(text);
                var result = System.Convert.ToBase64String(encodedText);
                return result;
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }
        public static string ToString(string data)
        {
            try
            {
                var decodedData = System.Convert.FromBase64String(data);
                var result = System.Text.ASCIIEncoding.ASCII.GetString(decodedData);
                return result;
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
