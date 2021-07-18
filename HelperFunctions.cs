using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("UnitTests")]

namespace Questionnaire_App
{
    internal class HelperFunctions
    {
        // Add To File
        internal static bool AddQuestionnaireToFile(JObject userObj, string jFile, string key)
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
                    o.Add(u.GetUUID(), JObject.Parse(json));
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
        internal static bool AddUserToFile(JObject userObj, string jFile, string key)
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
        internal static JObject JObjectFromQuestionnaire(Questionnaire q)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(q, Formatting.Indented);
            o[q.GetUUID()] = JObject.Parse(json);
            return o;
        }
        internal static JObject JObjectFromUser(IUser user)
        {
            JObject o = new();
            var json = JsonConvert.SerializeObject(user, Formatting.Indented);
            o[user.Uuid] = JObject.Parse(json);
            return o;
        }

        // Convert To Questionnaire
        internal static Questionnaire QuestionnaireFromJObject(JObject o)
        {
            string uuid = "";
            foreach (JProperty p in o.Children())
            {
                JToken value = p.Value;
                if (value.Type == JTokenType.Object)
                {
                    uuid = ((JObject)value).GetValue("uuid").ToString();
                }
            }
            string json = o[uuid].ToString();
            Questionnaire q = JsonConvert.DeserializeObject<Questionnaire>(json);
            return q;
        }

        // Hide Password Input
        internal static string ReadPassword()
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
        internal static void UpdateJson(string file, JObject o, string index)
        {
            string fileContent = File.ReadAllText(file);
            JObject u = JObject.Parse(fileContent);
            var json = JsonConvert.SerializeObject(o, Formatting.Indented);
            u[index] = JObject.Parse(json);
            string output = JsonConvert.SerializeObject(u, Formatting.Indented);
            File.WriteAllText(file, output);
        }

        // Convert To User
        internal static User UserFromJObject(JObject o)
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
        internal static (DateTime, Exception) ValidateDateInput(string s)
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
        internal static string CheckValidTimeframe((DateTime, DateTime) timeframe)
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
