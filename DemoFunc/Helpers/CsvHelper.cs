using System.Collections.Generic;
using System.Text;

namespace DemoFunc.Helpers
{
    public static class CsvHelper
    {
        public static string ToCsv()
        {
            return string.Empty;
        }

        public static string ToCSV(List<OrderDto> models, string delimiter = ",")
        {
            StringBuilder csv = new StringBuilder(160 + 100 * models.Count); // Its efficient to initialize StringBuilder to a higher value to prevent less memory
            csv.AppendLine("\"OrderId\",\"OrderStatus\",\"TimePlaced\"");

            void AppendWithQuote(object o)
            {
                // Using append for seperate string is more performant
                // that concatenating strings before using append
                csv.Append("\"");
                csv.Append((o ?? "").ToString());
                csv.Append("\"");
            }

            void AppendWithQuoteDelim(object o)
            {
                AppendWithQuote(o);
                csv.Append(delimiter);
            }

            foreach (var prod in models)
            {
                AppendWithQuoteDelim(prod.OrderId); // OrderId
                AppendWithQuoteDelim(prod.OrderStatus); // OrderStatus
                AppendWithQuoteDelim(prod.TimePlaced); // TimePlaced

                // New Line
                csv.AppendLine();
            }

            return csv.ToString();
        }

    }
}
