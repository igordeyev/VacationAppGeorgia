using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace TranslationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TranslationClient client = TranslationClient.CreateFromApiKey("AIzaSyCCLsJiGk03vSe1IGsEoIuhzRrRywZxMJU");

            // Prompt the user to enter their personal information
            Console.OutputEncoding = Encoding.UTF8;

            Console.Write("Enter your first name (ENG): ");
            string name = Console.ReadLine();

            Console.Write("Enter your last name (ENG): ");
            string surname = Console.ReadLine();

            string fullNameEn = $"{name}, {surname}";

            Console.Write("Enter your passport name: ");
            string passportNumber = Console.ReadLine();
            passportNumber = PadString(passportNumber, 12);

            Console.Write("Vacation start date (YYYY-MM-dd): ");
            string startDateString = Console.ReadLine();

            Console.Write("Vacation end date (YYYY-MM-dd): ");
            string endDateString = Console.ReadLine();

            Console.Write("Duration in work days: ");
            string durationDays = Console.ReadLine();

            // Parse the entered dates
            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(startDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(endDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                Console.WriteLine("Invalid date format. Please use the YYYY-MM-dd format.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // Translate the entered name and surname to Russian
            string fullNameRu = TranslateText(client, fullNameEn, "en", "ru");

            // Translate the entered name and surname to Georgian
            string fullNameGe = TranslateText(client, fullNameEn, "en", "ka");

            // Define the dictionary to hold the values to replace
            Dictionary<string, string> valueDictionary = new Dictionary<string, string>
            {
                { "<FULL_NAME_RU______>", PadString(fullNameRu, 20) },
                { "<PASSPORTNR>", PadString(passportNumber, 12) },
                { "<START___>", startDateString },
                { "<END_____>", endDateString },
                { "<FULL_NAME_GE______>", PadString(fullNameGe, 20) },
                { "<D>", PadString(durationDays,3) },
                { "<DATE____>", DateTime.Now.ToString("yyyy-MM-dd") }
            };

            // Load the template string from a file
            string template = File.ReadAllText("template.txt", Encoding.UTF8);

            // Replace the values in the template string
            string replacedString = ReplaceValues(template, valueDictionary);

            string fileName = "Application.txt";

            SaveToFile(fileName, replacedString);

            Console.WriteLine($"Application is saved to file {fileName}.");

            Console.WriteLine($"Press any key to exit.");
            Console.ReadKey();
        }

        static string TranslateText(TranslationClient client, string text, string sourceLanguage, string targetLanguage)
        {
            TranslationResult result = client.TranslateText(text, targetLanguage, sourceLanguage);
            return result.TranslatedText;
        }

        static string ReplaceValues(string template, Dictionary<string, string> valueDictionary)
        {
            foreach (var entry in valueDictionary)
            {
                template = template.Replace(entry.Key, entry.Value);
            }

            return template;
        }

        static void SaveToFile(string fileName, string content)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    writer.Write(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        static string PadString(string text, int length)
        {
            if (text.Length < length)
            {
                return text.PadRight(length);
            }
            else if (text.Length > length)
            {
                return text.Substring(0, length);
            }
            else
            {
                return text;
            }
        }
    }
}
