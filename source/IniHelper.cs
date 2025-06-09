using System.Collections.Generic;
using System.IO;

namespace Shop.source
{
    /// <summary>
    /// Статичний клас IniHelper надає методи для роботи з INI-файлами конфігурації.
    /// Дозволяє завантажувати налаштування, отримувати шлях до бази даних, параметри логування та розмір вікна.
    /// </summary>
    public static class IniHelper
    {
        /// <summary>
        /// Завантажує INI-файл і повертає всі налаштування у вигляді словника.
        /// </summary>
        /// <param name="filePath">Шлях до INI-файлу.</param>
        /// <returns>Словник налаштувань у форматі "Секція:Ключ" - "Значення".</returns>
        public static Dictionary<string, string> LoadIniFile(string filePath)
        {
            var settings = new Dictionary<string, string>();

            // Перевірка наявності INI-файлу
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The INI-File '{filePath}' was not found.");
            }

            string currentSection = "";
            foreach (var line in File.ReadLines(filePath))
            {
                string trimmedLine = line.Trim();

                // Пропустити порожні рядки та коментарі
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                {
                    continue;
                }

                // Початок секції
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Trim('[', ']');
                }
                else
                {
                    // Пара "Ключ=Значення"
                    var keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        settings[$"{currentSection}:{keyValue[0].Trim()}"] = keyValue[1].Trim();
                    }
                }
            }

            return settings;
        }

        /// <summary>
        /// Отримує шлях до файлу бази даних з INI-файлу.
        /// </summary>
        /// <param name="iniFilePath">Шлях до INI-файлу.</param>
        /// <returns>Шлях до файлу бази даних або порожній рядок, якщо не знайдено.</returns>
        public static string GetDatabaseFile(string iniFilePath)
        {
            var settings = LoadIniFile(iniFilePath);
            return settings.TryGetValue("Database:File", out var filename) ? filename : string.Empty;
        }

        /// <summary>
        /// Отримує параметр, чи потрібно показувати діалоги логування, з INI-файлу.
        /// </summary>
        /// <param name="iniFilePath">Шлях до INI-файлу.</param>
        /// <returns>true, якщо параметр Logging:ShowDialogs = ON, інакше false.</returns>
        public static bool GetLogMessagesDialogs(string iniFilePath)
        {
            var settings = LoadIniFile(iniFilePath);

            if (settings.TryGetValue("Logging:ShowDialogs", out var logMessages))
            {
                if (logMessages == "ON")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Отримує розмір головного вікна з INI-файлу.
        /// </summary>
        /// <param name="iniFilePath">Шлях до INI-файлу.</param>
        /// <returns>Кортеж з шириною та висотою вікна.</returns>
        public static (int Width, int Height) GetWindowSize(string iniFilePath)
        {
            var settings = LoadIniFile(iniFilePath);
            int width = 1280; // Ширина за замовчуванням
            int height = 960; // Висота за замовчуванням
            if (settings.TryGetValue("Window:Width", out var widthStr) && int.TryParse(widthStr, out var parsedWidth))
            {
                width = parsedWidth;
            }
            if (settings.TryGetValue("Window:Height", out var heightStr) && int.TryParse(heightStr, out var parsedHeight))
            {
                height = parsedHeight;
            }
            return (width, height);
        }
    }
}
