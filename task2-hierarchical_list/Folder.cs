using System;
using System.Collections.Generic;

namespace task2_hierarchical_list
{
    class Folder : Element // Класс объектов типа "папка"
    {
        public List<Element> Children { get; set; }

        public Folder(int key, int parentKey, string name) : base(key, parentKey, name, "folder")
        {
            Children = new List<Element>();
        }

        public void Add(Element element) // Метод для добавления элементов в папку
        {
            element.Parent = this;
            Children.Add(element);
        }

        public List<int> GetChildren() // Метод, возвращающий дочерние элементы папки
        {
            if (Children.Count == 0)
                return null;
            List<int> result = new List<int>();
            foreach (Element child in Children)
                result.Add(child.Key);
            return result;
        }
    }
}
