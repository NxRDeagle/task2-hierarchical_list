using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace task2_hierarchical_list
{
    class Program // Основной класс программы
    {
        static string GetChildrenInfo(List<Element> list, Folder folder) // Метод, возвращающий строку с ключами и именами дочерних элементов папки
        {
            string res = string.Empty;
            res += $"'{folder.Name}' children elements:\n";
            if (folder.GetChildren() == null)
                res += "none\n";
            else
                foreach (int key in folder.GetChildren())
                    res += $"key: {key}\t\tname: {list.Where(i => i.Key == key).FirstOrDefault().Name}\n";
            res += "\n";
            return res;
        }

        static string GetCatalogInfo(List<Element> list) // Метод, возвращающий строку с уровнями иерархии и именами элементов всего иерархического списка
        {
            string splitter = "\t", res = string.Empty;
            int i = 1;
            foreach (Element el in list)
            {
                while (i != el.Level)
                {
                    splitter += "\t";
                    i++;
                }
                res += $"{el.Level}{splitter}{el.Name}\n";
                splitter = "\t";
                i = 1;
            }
            return res;
        }

        static void Main()
        {
            string path, line, type, splitter = "\t";
            string[] columns;
            int level, previousLevel, i = 1;

            List<Element> list = new List<Element>();
            Folder root, folder;       
            File file;
            Element previousFolder, currentFolder;
            StreamReader sr;

            while (true) // Цикл для непрерывного ввода
            {
                Console.WriteLine("Введите путь к текстовому файлу, содержащему объекты иерархического списка: ");
                path = Console.ReadLine(); // D:/test_task2.txt
                if (path.Length == 0)
                    break;
                try
                {
                    sr = new StreamReader(path); // Чтение текстового файла
                    line = sr.ReadLine();

                    columns = line.Split("\t");
                    level = Convert.ToInt32(columns[0]);
                    previousLevel = level;

                    root = new Folder(0, columns[1], level); // Инициализация значения корневой папки
                    list.Add(root);
                    folder = root;

                    while (line != null) // Построчное считывание данных
                    {
                        line = sr.ReadLine();
                        if (line == null)
                            break;

                        columns = line.Split("\t"); // Получение информации об уровне и типе объекта
                        level = Convert.ToInt32(columns[0]);
                        while (i != level)
                        {
                            splitter += "\t";
                            i++;
                        }
                        columns = line.Split(splitter);
                        type = columns[1].Split(" ")[0];

                        if (level == 2) // Если уровень иерархии - 2, то элементы будут добавлены к корневой папке
                        {
                            if (type == "Файл")
                            {
                                file = new File(0, columns[1], level);
                                list.Add(file);
                                root.Add(file);
                            }
                            else
                            {
                                folder = new Folder(0, columns[1], level);
                                list.Add(folder);
                                root.Add(folder);
                            }
                        }
                        else
                        {
                            previousFolder = list.Where(i => i.Level == previousLevel - 1 && i.Type == "folder").LastOrDefault();
                            currentFolder = list.Where(i => i.Level == previousLevel && i.Type == "folder").LastOrDefault();

                            if (type == "Файл") // В зависимости от типа объекта и его уровня относительно предыдущего, создаем элементы и добавляем их в список
                            {
                                if (level <= previousLevel)
                                {
                                    file = new File(previousFolder.Key, columns[1], level);
                                    ((Folder)previousFolder).Add(file);
                                }
                                else
                                {
                                    file = new File(currentFolder.Key, columns[1], level);
                                    ((Folder)currentFolder).Add(file);
                                }
                                list.Add(file);
                            }
                            else
                            {
                                if (level <= previousLevel)
                                {
                                    folder = new Folder(previousFolder.Key, columns[1], level);
                                    ((Folder)previousFolder).Add(folder);
                                }
                                else
                                {
                                    folder = new Folder(currentFolder.Key, columns[1], level);
                                    ((Folder)currentFolder).Add(folder);
                                }
                                list.Add(folder);
                            }
                        }

                        i = 1; // Обнуление значений для следующей итерации цикла
                        splitter = "\t";
                        if (type == "Папка") 
                            previousLevel = level;
                    }
                    sr.Close();

                    Console.WriteLine(); // Вывод данных в консоль
                    foreach (Element el in list)
                    {
                        if (el.Type == "folder")
                            Console.WriteLine(GetChildrenInfo(list, (Folder)el));
                    }
                    Console.WriteLine(GetCatalogInfo(list));

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine("\n\n\nНажмите enter для повторного выбора файла.");
                Console.ReadLine();
            }
        }
    }
}
