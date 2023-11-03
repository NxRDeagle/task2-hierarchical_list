using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace task2_hierarchical_list
{
    class Program // Основной класс программы
    {
        static bool CheckLine(string line) // Метод проверки входных данных на корректность формата
        {
            string[] columns = line.Split(";");
            if (columns.Length != 4)
                return false;
            if (columns[3] != "0" && columns[3] != "1")
                return false;
            int k, p;
            if (!int.TryParse(columns[0], out k) || !int.TryParse(columns[1], out p))
                return false;
            return true;
        }

        static Folder GetRoot(List<Element> list) // Метод инициализации корневой папки
        {
            Folder root;
            if (!list.Contains(list.Where(i => i.Key == 0).FirstOrDefault()))
            {
                root = new Folder(0, -1, "Корневая папка");
                list.Add(root);
            }
            else root = (Folder)list.Where(i => i.Key == 0).FirstOrDefault();
            return root;
        }

        static void CheckKeys(ref List<Element> list, ref List<string> errorLines) // Метод проверки ключей, удаляет из рабочего списка ошибочные элементы
        {
            Element element, fileChild;
            for (int i = 0; i < list.Count - 1; i++)
            {
                element = list[i];
                fileChild = list.Where(i => i.Key == element.ParentKey && i.Type == "file").FirstOrDefault();
                if (!list.Contains(list.Where(i => i.Key == element.ParentKey).FirstOrDefault()))
                {
                    errorLines.Add($"Для элемента с ключом '{element.Key}' не был найден родительский элемент под ключом '{element.ParentKey}'");
                    list.Remove(element);
                    i--;
                }
                else if (list.Contains(fileChild))
                {
                    errorLines.Add($"Для элемента с ключом '{element.Key}' родительским элементом был назначен файл");
                    list.Remove(element);
                    i--;
                }
            }
        }

        static void SetChildren(List<Element> list) // Метод определения дочерних элементов
        {
            Element[] children;
            foreach (Element element in list)
            {
                if (element.Type != "folder") continue;
                else
                {
                    children = list.Where(i => i.ParentKey == element.Key).ToArray();
                    if (children.Length > 0)
                    {
                        foreach (var child in children)
                            if (list.Contains(child))
                                ((Folder)element).Add(child);
                    }
                }
            }
        }

        static void SetLevels(Element element, int level) // Метод распределения иерархических уровней
        {
            if (element == null) return;
            element.Level = level;
            if (element.Type == "folder")
            {
                if (((Folder)element).Children != null)
                {
                    foreach (Element child in ((Folder)element).Children)
                        SetLevels(child, level + 1);
                }
            }
        }

        static void GetHierarchicalList(Element element, ref List<Element> hierarchicalList) // Метод для получения иерархического списка элементов
        {
            hierarchicalList.Add(element);
            if (element.Type == "folder")
                if (((Folder)element).Children != null)
                {
                    foreach (Element child in ((Folder)element).Children)
                        GetHierarchicalList(child, ref hierarchicalList);
                }
        }

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
        static string GetErrors(List<string> errorLines) // Метод, возвращающий строку с ошибками во входных данных
        {
            string res = string.Empty;
            if (errorLines.Count > 0)
            {
                res += "\nВ ходе обработки входных данных были обнаружены ошибки:\n";
                foreach (var item in errorLines)
                    res += item + "\n";
            }
            return res;
        }

        static void Main()
        {
            string path, line = string.Empty, name, type;
            string[] columns;
            int key, parentKey;

            List<Element> list = new List<Element>();
            List<Element> hierarchicalList = new List<Element>();
            List<string> errorLines = new List<string>();

            Folder root, folder;
            File file;
            StreamReader sr;

            while (true) // Цикл для непрерывного ввода
            {
                Console.WriteLine("Введите путь к текстовому файлу, содержащему объекты иерархического списка: ");
                path = Console.ReadLine(); // D:/test_task2.txt  D:/test_task2_1.txt
                if (path.Length == 0)
                    break;

                try
                {
                    sr = new StreamReader(path); // Чтение текстового файла
                    while (line != null) // Построчное считывание данных
                    {
                        line = sr.ReadLine();
                        if (line == null)
                            break;

                        if (!CheckLine(line)) // Проверка входных данных
                        {
                            errorLines.Add($"{line} <- неверный формат строки");
                            continue;
                        }
                        else
                        {
                            columns = line.Split(";");
                            key = int.Parse(columns[0]);
                            parentKey = int.Parse(columns[1]);
                            if (key == parentKey)
                            {
                                errorLines.Add($"{line} <- родительский ключ элемента не может быть равен основному ключу элемента");
                                continue;
                            }
                            name = columns[2];
                            type = columns[3];
                            if (key == 0 && type == "1")
                            {
                                errorLines.Add($"{line} <- попытка назначить файлу роль корневой папки");
                                continue;
                            }

                            if (!list.Contains(list.Where(i => i.Key == key).FirstOrDefault()))
                            {
                                if (type == "1") // Создание объектов и добавление в список при правильных входных данных
                                {
                                    file = new File(key, parentKey, name);
                                    list.Add(file);
                                }
                                else
                                {
                                    folder = new Folder(key, parentKey, name);
                                    list.Add(folder);
                                }
                            }
                            else errorLines.Add($"{line} <- попытка повторного использования ключа элемента");
                        }
                    }
                    sr.Close();

                    root = GetRoot(list); // Задаем корневую папку

                    CheckKeys(ref list, ref errorLines); // Ищем ошибки связанные с ключами

                    SetChildren(list); // Добавляем к элементам дочерние 

                    SetLevels(root, 1); // Высчитываем значения иерархических уровней

                    GetHierarchicalList(root, ref hierarchicalList); // Получаем иерархический список, отсортированный по вложенности элементов


                    Console.WriteLine(); // Выводим данные в консоль
                    foreach (Element el in hierarchicalList)
                    {
                        if (el.Type == "folder")
                            Console.WriteLine(GetChildrenInfo(hierarchicalList, (Folder)el));
                    }
                    
                    Console.WriteLine(GetCatalogInfo(hierarchicalList));

                    Console.WriteLine(GetErrors(errorLines));

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    list.Clear();
                    hierarchicalList.Clear();
                    errorLines.Clear();
                    folder = null;
                    file = null;
                    root = null;
                    line = string.Empty;
                }

                Console.WriteLine("\nНажмите enter для повторного выбора файла.");
                Console.ReadLine();
            }
        }
    }
}
