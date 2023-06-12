using DemoFunc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoFunc.Helpers
{
    public static class CsvTransformer
    {
        private static readonly StringBuilder sb = new StringBuilder(5000 * 100);
        private static char Seperator => ',';
        private static bool UseQuotationOnHeader => true;

        public static void Initialize()
        {
            // By instantiating the StringBuilder with a high capacity, we save
            // time from allocating new memory
            var headers = GetHeaders();

            // HEADERS
            if (!headers.Any())
                throw new Exception("Header contained no elements");

            if (UseQuotationOnHeader)
                AppendLine(headers);
            else
                AppendLineNoQuotes(headers);
        }

        public static void Transform(List<OrderDto> orders)
        {
            // ROWS
            foreach (var order in orders)
            {
                var prodRow = GetOrderRows(order);
                if (!prodRow.Any())
                    throw new Exception("ProdRow contained no elements");

                AppendLine(prodRow);
            }
        }

        public static TransformData Complete()
        {
            var results = TransformData.Success(sb.ToString());
            sb.Clear();
            return results;
        }

        private static void AppendLineNoQuotes(List<string> values)
        {
            sb.Append(values[0].Replace('"', '\'')); // ensure that " does not exist in the string, otherwise will ruin the export

            foreach (var value in values.Skip(1))
            {
                sb.Append(Seperator);
                sb.Append(value.Replace('"', '\'')); // ensure that " does not exist in the string, otherwise will ruin the export
            }

            sb.AppendLine();
        }

        private static void AppendLine(List<string> values)
        {
            sb.Append('"');
            sb.Append(values[0].Replace('"', '\'')); // ensure that " does not exist in the string, otherwise will ruin the export
            sb.Append('"');

            foreach (var value in values.Skip(1))
            {
                sb.Append(Seperator);

                sb.Append('"');
                sb.Append((value ?? "").Replace('"', '\'')); // ensure that " does not exist in the string, otherwise will ruin the export
                sb.Append('"');
            }

            sb.AppendLine();
        }

        private static List<string> GetHeaders()
        {
            return new List<string>(3)
            {
                "OrderId",
                "OrderStatus",
                "TimePlaced",
            };
        }

        /// <summary>
        /// Retrieve the CSV row values according the orders
        /// </summary>
        private static List<string> GetOrderRows(OrderDto orderDto)
        {
            return new List<string>(3)
            {
                orderDto.OrderId.ToString() ?? string.Empty,
                orderDto.OrderStatus.ToString() ?? string.Empty,
                orderDto.TimePlaced.ToString() ?? string.Empty,
            };
        }
    }
}
