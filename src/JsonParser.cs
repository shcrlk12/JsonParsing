using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParsing
{
    class JsonParser
    {
        static Stack<Dictionary<string, object>> dictStack = new Stack<Dictionary<string, object>>();
        static Stack<string> keyStack = new Stack<string>();

        public static Dictionary<string, object> StringToDictionary(string jsonString)
        {
            int i = 0;
            return StringToDictionary(jsonString, ref i);
        }

        public static Dictionary<string, object> StringToDictionary(string jsonString, ref int i)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            char c;

            while (i < jsonString.Length)
            {
                c = jsonString[i];
                if (c == '{')
                {
                    Dictionary<string, object> newDict = new Dictionary<string, object>();
                    dictStack.Push(newDict);
                }
                else if (c == '}')
                {
                    i++;

                    if (dictStack.Count > 1)
                    {
                        return dictStack.Pop();
                    }

                    continue;
                }
                else if (c == ':')
                {
                    i = jsonString.LastIndexOf("\"", i - 2);
                    keyStack.Push(ParseString(jsonString, ref i));

                    i++;
                    c = jsonString[i];
                    object value;
                    if (c == '\"')
                    {
                        value = ParseString(jsonString, ref i);
                    }
                    else if (c == '[')
                    {
                        value = ParseArray(jsonString, ref i);
                    }
                    else if (c == '{')
                    {
                        value = ParseObject(jsonString, ref i);
                    }
                    else
                    {
                        value = ParseValue(jsonString, ref i);
                    }

                    if (dictStack.Count > 0)
                    {
                        string key = keyStack.Peek();
                        dictStack.Peek().Add(key, value);
                        keyStack.Pop();
                    }
                    continue;
                }
                i++;
            }

            result = dictStack.Pop();
            return result;
        }

        private static Dictionary<string, object> ParseObject(string jsonString, ref int i)
        {
            return StringToDictionary(jsonString, ref i);
        }

        private static List<object> ParseArray(string jsonString, ref int i)
        {
            List<object> array = new List<object>();
            char c;
            i++;

            while (i < jsonString.Length)
            {
                c = jsonString[i];
                if (c == '{')
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    dictStack.Push((Dictionary<string, object>)obj);
                }
                else if (c == '}')
                {
                    array.Add(dictStack.Pop());
                }
                else if (c == ']')
                {
                    i++;
                    break;
                }
                else if (c == ':')
                {
                    i = jsonString.LastIndexOf("\"", i - 2);
                    keyStack.Push(ParseString(jsonString, ref i));

                    i++;
                    c = jsonString[i];
                    object value;
                    if (c == '\"')
                    {
                        value = ParseString(jsonString, ref i);
                    }
                    else if (c == '[')
                    {
                        value = ParseArray(jsonString, ref i);
                    }
                    else if (c == '{')
                    {
                        value = ParseObject(jsonString, ref i);
                    }
                    else
                    {
                        value = ParseValue(jsonString, ref i);
                    }

                    if (dictStack.Count > 0)
                    {
                        string key = keyStack.Peek();
                        dictStack.Peek().Add(key, value);
                        keyStack.Pop();
                    }

                    continue;
                }
                i++;
            }

            return array;
        }

        private static object ParseValue(string jsonString, ref int i)
        {
            string valueString = "";
            char c;
            while (i < jsonString.Length)
            {
                c = jsonString[i];
                if (c == ',' || c == ' ' || c == '}' || c == ']')
                {
                    break;
                }
                valueString += c;
                i++;
            }

            object value;
            bool boolValue;
            int intValue;
            double doubleValue;

            if (bool.TryParse(valueString, out boolValue))
            {
                value = boolValue;
            }
            else if (int.TryParse(valueString, out intValue))
            {
                value = intValue;
            }
            else if (double.TryParse(valueString, out doubleValue))
            {
                value = doubleValue;
            }
            else
            {
                value = valueString;
            }

            return value;
        }


        private static string ParseString(string jsonString, ref int i)
        {
            string str = "";
            char c;
            i++;
            while (i < jsonString.Length)
            {
                c = jsonString[i];
                if (c == '\"')
                {
                    i++;
                    break;
                }
                else if (c == '\\')
                {
                    i++;
                    c = jsonString[i];
                    if (c == 'u')
                    {
                        string hex = "";
                        for (int j = 0; j < 4; j++)
                        {
                            hex += jsonString[++i];
                        }
                        str += (char)Convert.ToInt32(hex, 16);
                    }
                    else if (c == '\"' || c == '\\' || c == '/')
                    {
                        str += c;
                    }
                    else if (c == 'b')
                    {
                        str += '\b';
                    }
                    else if (c == 'f')
                    {
                        str += '\f';
                    }
                    else if (c == 'n')
                    {
                        str += '\n';
                    }
                    else if (c == 'r')
                    {
                        str += '\r';
                    }
                    else if (c == 't')
                    {
                        str += '\t';
                    }
                }
                else
                {
                    str += c;
                }
                i++;
            }

            return str;
        }
    }
}
