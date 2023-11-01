using System;

namespace task2_hierarchical_list
{
    class File : Element // Класс объектов типа "файл"
    {
        public File(int key, int parentKey, string name) : base(key, parentKey, name, "file")
        { }
    }
}
