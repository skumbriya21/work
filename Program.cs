using System;
using System.Collections.Generic;
using System.IO;

namespace Lab1
{
    public struct GeneticData
    {
        public static List<char> letters = new List<char>() { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };

        public string name;
        public string organism;
        public string formula; // структура которая будет хранить инфу о всех объектах-протеинах

        public static string Decode(string formula) // расшифровывает формулу или отрывок
        {
            string decoded = string.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i])) // если цифра, то начинается раскодировка и к заполняемой строке добавляется н-ное количество аминокислот
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    for (int j = 0; j < conversion - 1; j++) decoded = decoded + letter;
                }
                else decoded = decoded + formula[i]; // не число = просто добавляется кислота к decoded
            }
            return decoded;
        }

        public bool IsValid() // проверяет, правильные ли кислоты в формуле, используется в мейне
        {
            foreach (char ch in Decode(formula))
            {
                if (!letters.Contains(ch)) return false;
            }
            return true;
        }
    }

    class Program
    {
        static List<GeneticData> data = FileParser.ReadGeneticData(); // читает файл с протеинами и возвращает их список

        static string GetFormula(string proteinName) // находит по имени протеина его формулу
        {
            foreach (GeneticData item in data) // перебирает список протеинов
            {
                if (item.name.Equals(proteinName)) return item.formula; 
            }
            return "";
        }

        static List<int> Search(string amino_acid) // ищет по отрывку протеин, возвращает список, а не один элемент, потому что
                                                   // фактически совпать могло несколько вариантов, просто в проверочных файлах на дисках такого не происходит
        {
            List<int> list = new List<int>();
            string decoded = GeneticData.Decode(amino_acid); // расшифровывает передаваемый отрывок формулы, потому что может быть зашифрован
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        static int Diff(string name1, string name2) // находит различия аминокислот дада
        {
            int size, counter = 0; // size для того, чтобы определить, сколько сравнивать элементов, потому что одна формула может быть короче другой
            // counter считает количество различающихся
            string formula1 = GetFormula(name1), formula2 = GetFormula(name2);
            if (formula1 == "" && formula2 == "") // обе не найдены 
            {
                return -3;
            }
            else if (formula2 == "") // вторая не найдена
            {
                return -2;
            }
            else if (formula1 == "") // первая не найдена
            {
                return -1;
            }
            if (formula1.Length > formula2.Length)
            { // сверяют длины формул, чтобы получить size и не выйти за пределы одной из формул
                size = formula2.Length;
                counter += formula1.Length - formula2.Length;
            }
            else
            {
                size = formula1.Length;
                counter += formula2.Length - formula1.Length;
            }
            for (int i = 0; i < size; i++) // сравнивает size-ое количество кислот, остальные сразу заносятся в counter как различающиеся
            {
                if (formula1[i] != formula2[i]) counter++;
            }
            return counter;
        }

        static List<int> numberOfAcids = new List<int>(new int[20]); // для Mode. Со списком letters получается как таблица, каждый элемент
        // здесь соответствует какой-то кислоте и делает +1, когда проверяется в Mode
        static int Mode(string name)
        {
            string formula = GetFormula(name); // нашли формулу протеина
            int number = 0, index = 0; // number будет хранить число самой частой кислоты, индекс соответственно её индекс
            if (formula == "") return -1; // если не найдено, функция прекращается
            for (int i = 0; i < 20; i++)
            {
                numberOfAcids[i] = 0; // заполняет в первый раз/подготавливает список, чтобы там все были нулями до начала поиска самой частой
            }
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < formula.Length; j++)
                {
                    if (formula[j] == GeneticData.letters[i]) numberOfAcids[i]++; // перебирает каждую кислоту в формуле, прибавляет +1 в список при совпадении
                }
                if (numberOfAcids[i] > number) number = numberOfAcids[i];  // приравнивает к числу кислоты если оно больше встречавшегося раньше
            }
            for (int i = 0; i < 20; i++)
            {
                if (numberOfAcids[i] == number) // находит индекс
                {
                    index = i;
                    break;
                }
            }
            return index; // возвращает только индекс, поэтому список numberOfAcids вне функции, чтобы следующая могла к нему обращаться по индексу
        }

        public static void ReadHandleCommands() // читает команды из файла и заполняет сразу файл вывода
        {
            StreamReader reader = new StreamReader(FileParser.CommandsDataFile); // поток для чтения
            int counter = 0; // для нумерации команд и удобного заполнения файла вывода

            StreamWriter writer = new StreamWriter(FileParser.OutputDataFile); // поток для написания
            Console.WriteLine("Kulikovskaya Alina\nGenetic Searching"); // исправила на своё?
            writer.WriteLine("Skumbriya\nGenetic Searching");

            while (!reader.EndOfStream)
            {
                Console.WriteLine("========================================================================");
                writer.WriteLine("========================================================================");
                string line = reader.ReadLine(); counter++; // читает команду
                string[] command = line.Split('\t'); // делит команду на составляющие и приравнивает дальше
                if (command[0].Equals("search")) // если первая составляющая search то
                {
                    Console.WriteLine($"{counter.ToString("D3")}   {"search"}   {GeneticData.Decode(command[1])}"); // можно убрать расшифровку, она сразу добавлена в fileparser
                    writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + GeneticData.Decode(command[1]));
                    List<int> index = Search(command[1]); // ищет
                    Console.WriteLine("organism\t\t\t\tprotein");
                    writer.WriteLine("organism\t\t\t\tprotein");
                    if (index.Count > 0) // полученный лист выводит и заносит в файл
                    {
                        for (int i = 0; i < index.Count; i++)
                        {
                            Console.WriteLine($"{data[index[i]].organism}    {data[index[i]].name}");
                            writer.WriteLine(data[index[i]].organism + "\t\t" + data[index[i]].name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("NOT FOUND");
                        writer.WriteLine("NOT FOUND");
                    }
                }
                if (command[0].Equals("diff"))
                {
                    Console.WriteLine($"{counter.ToString("D3")}   {"diff"}   {command[1]}   {command[2]}");
                    writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + command[1] + "\t" + command[2]);
                    Console.WriteLine("amino-acids difference: ");
                    writer.WriteLine("amino-acids difference: ");
                    if (Diff(command[1], command[2]) >= 0) // ну тут понятно, в функции написано за что -1 -2 -3 отвечают
                    {
                        Console.WriteLine(Diff(command[1], command[2]));
                        writer.WriteLine(Diff(command[1], command[2]));
                    }
                    else if (Diff(command[1], command[2]) == -1)
                    {
                        Console.WriteLine("MISSING: " + command[1]);
                        writer.WriteLine("MISSING: " + command[1]);
                    }
                    else if (Diff(command[1], command[2]) == -2)
                    {
                        Console.WriteLine("MISSING: " + command[2]);
                        writer.WriteLine("MISSING: " + command[2]);
                    }
                    else
                    {
                        Console.WriteLine("MISSING: " + command[1] + "\t" + command[2]);
                        writer.WriteLine("MISSING: " + command[1] + "\t" + command[2]);
                    }
                }
                if (command[0].Equals("mode"))
                {
                    Console.WriteLine($"{counter.ToString("D3")}   {"mode"}   {command[1]}");
                    writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + command[1]);
                    Console.WriteLine("amino-acid occurs: ");
                    writer.WriteLine("amino-acid occurs: ");
                    if (Mode(command[1]) == -1)
                    {
                        Console.WriteLine("MISSING: " + command[1]);
                        writer.WriteLine("MISSING: " + command[1]);
                    }
                    else
                    {
                        Console.WriteLine(GeneticData.letters[Mode(command[1])] + "\t\t" + numberOfAcids[Mode(command[1])]);
                        writer.WriteLine(GeneticData.letters[Mode(command[1])] + "\t\t" + numberOfAcids[Mode(command[1])]);
                    }
                }
            }
            writer.WriteLine("========================================================================");
            reader.Close();
            writer.Close();
        }

        static void Main()
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].IsValid() == false)
                {
                    Console.WriteLine("ERROR. Wrong formula of the " + (i + 1) + "-st protein"); // если кислота неправильная завершает работу программы вообще
                    return;
                }
            }
            ReadHandleCommands(); // начинает код
        }
    }
}