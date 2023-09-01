using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParsing
{
    class Program
    {
        static void Main(string[] args)
        {
            String jsonData = "{\"response\":{\"items\":[\"item\":{\"name\":\"sword\"\"weight\":15},\"item\":{\"name\":\"bow\"\"weight\":10}]}}";
            Dictionary<string, object> json = JsonParser.StringToDictionary(jsonData);

            List<object> list = 
                (List<object>)
                    ((Dictionary<string, object>)
                        ((Dictionary<string, object>)json["response"])
                    ["items"])
                ["item"];

            foreach (Dictionary<string, object> item in list)
            {
                string baseDate = Convert.ToString(item["name"]);
                string baseTime = Convert.ToString(item["weight"]);
            }
        }
    }
}
