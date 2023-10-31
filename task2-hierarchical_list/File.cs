using System;

namespace task2_hierarchical_list
{
    class File : Element // Класс объектов типа "файл"
    {
        public File(int parentKey, string name, int level) : base(parentKey, "file", name, level)
        { }
    }
}
