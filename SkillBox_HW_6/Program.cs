using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace HomeWork6
{
    internal class Program
    {
        private static string outText = "Группа 1: 1";
        private static int countGroup = 1;
        private static string inputPath = "Number.txt";
        private static string outputPath = "OutputGroup.txt";
        private static string compressed = "OutputGroup.zip";

        private static void Main(string[] args)
        {
            RequestPath();

            var inNumber = GetInputNumber();

            Console.Write("Посчитать только количество групп? (Да/Нет): ");
            var yesNo = Console.ReadLine();
            yesNo.ToLower();
            
            if (Equals(yesNo, "да"))
            {
                NumberOfGroups(inNumber);
                OutNumberOfGroups(inNumber, true);
            }
            else
            {
                SplitIntoGroup(inNumber);
                OutNumberOfGroups(inNumber, false);
            }

            Archiving();

            Console.ReadKey();
        }


        /// <summary>
        ///     Путь к файлу. Запрашиваем путь к файлу с числом N
        /// </summary>
        private static void RequestPath()
        {
            Console.WriteLine(
                "Введите название файла с числом N и его расширение (txt) или весь путь к данному файлу:");
            inputPath = Console.ReadLine();

            CheckTxtRequestPath();

            outputPath = inputPath.Insert(inputPath.LastIndexOf('\\') + 1, "Output");
            compressed = outputPath.Remove(outputPath.LastIndexOf('.') + 1) + "zip";
        }

        /// <summary>
        ///     Провека на наличе и расширение файла.
        /// </summary>
        private static void CheckTxtRequestPath()
        {
            var o = File.Exists(inputPath);

            if (inputPath[inputPath.Length - 1] != 't' && inputPath[inputPath.Length - 2] != 'x' &&
                inputPath[inputPath.Length - 3] != 't')
            {
                o = false;
            }

            if (inputPath.LastIndexOf('.') == -1 || o == false)
            {
                Console.Clear();
                Console.WriteLine(
                    "Такого файла не существует или у файла нет или неправильное расширение. Установите расширение (txt) " +
                    "или создайте файл с данным расширением.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        /// <summary>
        ///     Метод нахождения ТОЛЬКО количества групп
        /// </summary>
        /// <param name="inNumber">Число N из файла.</param>
        private static void NumberOfGroups(long inNumber)
        {
            var n = 1;
            while (n < inNumber)
            {
                n *= 2;
                countGroup++;
            }
        }

        /// <summary>
        ///     Архивация полученного файла
        /// </summary>
        private static void Archiving()
        {
            string yesNo;
            Console.Write("Создать архив данного файла? (Да/Нет): ");
            yesNo = Console.ReadLine();
            yesNo.ToLower();
            if (Equals(yesNo, "да"))
            {
                ArchivingFile();
                Console.Clear();
                Console.WriteLine(
                    $"Архивация файла {outputPath} завершина.\nРазмер файла до архивации: {new FileInfo(outputPath).Length} байт;" +
                    $"\nРазмер файла после архивации: {new FileInfo(compressed).Length} байт.");
            }
        }

        /// <summary>
        ///     Метод логики архивации
        /// </summary>
        private static void ArchivingFile()
        {
            using (FileStream ss = new FileStream(outputPath, FileMode.OpenOrCreate))
            {
                using (FileStream ts = File.Create(compressed)) //поток для записи сжатого файла
                {
                    using (GZipStream cs = new GZipStream(ts, CompressionMode.Compress)) //Поток архивации
                    {
                        ss.CopyTo(cs); //Копирование основного потока (файла) в другой (архивный)
                    }
                }
            }
        }

        /// <summary>
        ///     Вывод Количества групп.
        /// </summary>
        /// <param name="inNumber">Число N</param>
        /// <param name="a">Условие на добавление строки в пустой файл или файл с данными</param>
        private static void OutNumberOfGroups(long inNumber, bool a)
        {
            string yesNo;
            Console.Clear();
            Console.WriteLine($"Количесвто групп чисел для N = {inNumber} равно {countGroup}.");
            Console.Write("Записать количество групп на диск? (Да/Нет): ");
            yesNo = Console.ReadLine();
            yesNo.ToLower();
            if (Equals(yesNo, "да"))
            {
                if (a)
                {
                    File.WriteAllText(outputPath, $"Количесвто групп чисел для N = {inNumber} равно {countGroup}.");
                }
                else
                {
                    File.AppendAllText(outputPath,
                        $"\n\n\nКоличесвто групп чисел для N = {inNumber} равно {countGroup}.");
                }
            }
        }

        /// <summary>
        ///     Разбивает числа по группам в диапазоне от 1 до N
        /// </summary>
        /// <param name="inNumber"></param>
        private static void SplitIntoGroup(long inNumber)
        {
            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                int newLine = 1;
                sw.Write(outText);
                for (int i = 2; i <= inNumber; i++)
                {
                    outText = string.Empty;

                    if (i == newLine * 2)
                    {
                        newLine = i;
                        countGroup++;
                        outText += $"\n\nГруппа {countGroup}: {i}";
                    }
                    else
                    {
                        outText += $" {i}";
                    }
                    sw.Write(outText);
                }
            }
        }

        /// <summary>
        ///     Чтение числа из файла с проверкой
        /// </summary>
        /// <returns>Число N из файла</returns>
        private static long GetInputNumber()
        {
            long inNumber = 0;

            var inputNumber = File.ReadAllText(inputPath);
            inputNumber = inputNumber.Trim();

            for (int i = 0; i < inputNumber.Length; i++)
            {
                if (inputNumber[i] == ' ')
                {
                    inputNumber = inputNumber.Substring(0, i);
                    break;
                }
            }

            if (Equals(inputNumber, ""))
            {
                Error();
            }
            else
            {
                if (Regex.IsMatch(inputNumber, @"^[\p{N}]+$") == false)
                {
                    Error();
                }
                else
                {
                    inNumber = long.Parse(inputNumber);

                    if (inNumber < 1 || inNumber > 1_000_000_000)
                    {
                        Error();
                    }
                }
            }

            return inNumber;
        }

        /// <summary>
        ///     Не соблюдение условия или пустой файл.
        /// </summary>
        private static void Error()
        {
            outText =
                "Файл пустой или число не удовлетворяет условиям. Введите в файл число в диапозоне: 1 <= N <= 1 000 000 000";

            File.WriteAllText(outputPath, outText);
            Console.WriteLine(outText);
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}