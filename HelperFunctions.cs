using System;
using ConsoleTools;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Questionnaire_App
{
    class HelperFunctions
    {
        // Add To File
        public static bool AddQuestionnaireToFile(JObject userObj, string jFile, string key)
        {
            bool success = false;
            string writeJson;
            if (File.Exists(jFile))
            {
                string fileContent = File.ReadAllText(jFile);
                JObject o = JObject.Parse(fileContent);
                Questionnaire u = QuestionnaireFromJObject(userObj);
                var json = JsonConvert.SerializeObject(u, Formatting.Indented);
                if (!o.ContainsKey(key))
                {
                    o.Add(u.uuid, JObject.Parse(json));
                    success = true;
                    writeJson = JsonConvert.SerializeObject(o, Formatting.Indented);
                }
                else return success;
            }
            else
            {
                writeJson = JsonConvert.SerializeObject(userObj, Formatting.Indented);
                success = true;
            }
            File.WriteAllText(jFile, writeJson);
            return success;
        }
        public static bool AddUserToFile(JObject userObj, string jFile, string key)
        {
            bool success = false;
            string writeJson;
            if (File.Exists(jFile))
            {
                string fileContent = File.ReadAllText(jFile);
                JObject o = JObject.Parse(fileContent);
                User u = UserFromJObject(userObj);
                var json = JsonConvert.SerializeObject(u, Formatting.Indented);
                if (!o.ContainsKey(key))
                {
                    o.Add(u.Uuid, JObject.Parse(json));
                    success = true;
                    writeJson = JsonConvert.SerializeObject(o, Formatting.Indented);
                }
                else return success;
            }
            else
            {
                writeJson = JsonConvert.SerializeObject(userObj, Formatting.Indented);
                success = true;
            }
            File.WriteAllText(jFile, writeJson);
            return success;
        }

        // Convert To JObject
        public static JObject JObjectFromQuestionnaire(Questionnaire q)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(q, Formatting.Indented);
            o[q.uuid] = JObject.Parse(json);
            return o;
        }
        public static JObject JObjectFromUser(IUser user)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(user, Formatting.Indented);
            o[user.Uuid] = JObject.Parse(json);
            return o;
        }

        // Convert To Questionnaire
        public static Questionnaire QuestionnaireFromJObject(JObject o)
        {
            string uuid = "";
            foreach (JProperty p in o.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    uuid = ((JObject)value).GetValue("uuid").ToString();
                    Console.WriteLine(uuid);
                }
            }
            string json = o[uuid].ToString();
            Questionnaire q = JsonConvert.DeserializeObject<Questionnaire>(json);
            return q;
        }

        // Hide Password Input
        public static string ReadPassword()
        {
            string password = "";

            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password[0..^1];
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            return password;
        }

        // Update JSON after Questionnaire
        public static void UpdateJson(string file, JObject o, string index)
        {
            string fileContent = File.ReadAllText(file);
            JObject u = JObject.Parse(fileContent);
            var json = JsonConvert.SerializeObject(o, Formatting.Indented);
            u[index] = JObject.Parse(json);
            string output = JsonConvert.SerializeObject(u, Formatting.Indented);
            File.WriteAllText(file, output);
        }

        // Convert To User
        public static User UserFromJObject(JObject o)
        {
            string uuid = "";
            foreach (JProperty p in o.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    uuid = ((JObject)value).GetValue("Uuid").ToString();
                }
            }
            string json = o[uuid].ToString();
            User u = JsonConvert.DeserializeObject<User>(json);
            return u;
        }

        // Check for Valid Date
        public static (DateTime, Exception) ValidateDateInput(string s)
        {
            Exception ex;
            string[] formats = { "dd/MM/yy" , "MM/dd/yyyy hh:mm:ss"};
            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                ex = null;
            }
            else
            {
                ex = new Exception("invalid date.");
            }
            return (dt, ex);
        }

        // Check if Questionnaire is Blocked
        public static string CheckValidTimeframe((DateTime, DateTime) timeframe)
        {
            string result;

            // startdate is later than today
            if (timeframe.Item1 > DateTime.Now)
            {
                result = "early";
            }
            //enddate is earlier than today
            else if (timeframe.Item2 < DateTime.Now)
            {
                result = "late";
            }
            else result = "valid";
            
            return result;
        }
    }
}
