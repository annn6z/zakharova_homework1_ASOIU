using System;

namespace DamerauLevenshteinDistance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Программа вычисления расстояния Дамерау-Левенштейна ===");
            Console.WriteLine("Для выхода введите 'exit' в качестве первой строки\n");

            // Небольшое тестирование для проверки
            RunTests();

            while (true)
            {
                Console.Write("\nВведите первую строку: ");
                string str1 = Console.ReadLine();

                if (string.IsNullOrEmpty(str1))
                {
                    Console.WriteLine("Строка не может быть пустой. Попробуйте снова.");
                    continue;
                }

                if (str1.ToLower() == "exit")
                {
                    Console.WriteLine("Программа завершена.");
                    break;
                }

                Console.Write("Введите вторую строку: ");
                string str2 = Console.ReadLine();

                if (string.IsNullOrEmpty(str2))
                {
                    Console.WriteLine("Строка не может быть пустой. Попробуйте снова.");
                    continue;
                }

                // Вычисление и вывод результата
                int distance = DamerauLevenshteinDistance(str1, str2);
                Console.WriteLine($"Результат: расстояние между '{str1}' и '{str2}' = {distance}");
            }
        }

        /// <summary>
        /// Вычисление расстояния Дамерау-Левенштейна
        /// </summary>
        public static int DamerauLevenshteinDistance(string str1Param, string str2Param)
        {
            // Проверка на null
            if (str1Param == null || str2Param == null)
                return -1;

            int str1Len = str1Param.Length;
            int str2Len = str2Param.Length;

            // Если хотя бы одна строка пустая
            if (str1Len == 0) return str2Len;
            if (str2Len == 0) return str1Len;

            // Приведение строк к верхнему регистру (для регистронезависимого сравнения)
            string str1 = str1Param.ToUpper();
            string str2 = str2Param.ToUpper();

            // Объявление матрицы
            int[,] matrix = new int[str1Len + 1, str2Len + 1];

            // Инициализация нулевой строки и нулевого столбца
            for (int i = 0; i <= str1Len; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= str2Len; j++)
                matrix[0, j] = j;

            // Вычисление расстояния Дамерау-Левенштейна
            for (int i = 1; i <= str1Len; i++)
            {
                for (int j = 1; j <= str2Len; j++)
                {
                    // Стоимость замены (0 если символы совпадают, 1 если разные)
                    int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;

                    // Минимум из:
                    // - удаление (matrix[i-1, j] + 1)
                    // - вставка (matrix[i, j-1] + 1)  
                    // - замена (matrix[i-1, j-1] + cost)
                    matrix[i, j] = Math.Min(
                        Math.Min(
                            matrix[i - 1, j] + 1,      // удаление
                            matrix[i, j - 1] + 1       // вставка
                        ),
                        matrix[i - 1, j - 1] + cost     // замена
                    );

                    // Проверка на транспозицию (перестановку соседних символов)
                    // Это и есть поправка Дамерау
                    if (i > 1 && j > 1 &&
                        str1[i - 1] == str2[j - 2] &&
                        str1[i - 2] == str2[j - 1])
                    {
                        matrix[i, j] = Math.Min(
                            matrix[i, j],
                            matrix[i - 2, j - 2] + cost  // транспозиция
                        );
                    }
                }
            }

            // Возвращается нижний правый элемент матрицы
            return matrix[str1Len, str2Len];
        }

        /// <summary>
        /// Метод для тестирования алгоритма
        /// </summary>
        static void RunTests()
        {
            Console.WriteLine("=== ТЕСТИРОВАНИЕ АЛГОРИТМА ===");

            // Тест 1: одинаковые строки
            Test("ИВАНОВ", "ИВАНОВ", 0, "Одинаковые строки");

            // Тест 2: транспозиция (перестановка соседних букв)
            Test("ИВАНОВ", "ИВАНВО", 2, "Транспозиция (должно быть 2 по Левенштейну, 1 по Дамерау-Левенштейну - проверьте сами)");

            // Тест 3: пример из методички
            Test("ИВАНОВ", "БАННВО", 4, "Пример из методички");

            // Тест 4: вставка символа
            Test("пример", "1пример", 1, "Вставка в начало");
            Test("пример", "при1мер", 1, "Вставка в середину");
            Test("пример", "пример1", 1, "Вставка в конец");

            // Тест 5: транспозиция с разным регистром
            Test("приМер", "прМир", 1, "Транспозиция с разным регистром");

            // Тест 6: удаление символа
            Test("пример", "ример", 1, "Удаление символа");

            // Тест 7: замена символа
            Test("пример", "премер", 1, "Замена символа");

            Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО ===\n");
        }

        static void Test(string s1, string s2, int expected, string description)
        {
            int result = DamerauLevenshteinDistance(s1, s2);
            string status = (result == expected) ? "✓" : "✗";
            Console.WriteLine($"{status} {description}: '{s1}' -> '{s2}' = {result} (ожидалось {expected})");
        }
    }
}