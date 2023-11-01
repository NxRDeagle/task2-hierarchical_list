using System;

namespace task2_hierarchical_list
{
    class Element // Базовый класс объектов иерархического списка
    {
        private int key, parentKey, level;
        private string name, type;

        protected Element(int key, int parentKey, string name, string type)
        {
            this.key = key;
            this.parentKey = parentKey;
            this.name = name;
            this.type = type;
        }

        public int Key
        {
            get => key;
        }
        public int ParentKey
        {
            get => parentKey;
            set => parentKey = value;
        }
        public string Name
        {
            get => name;
            set => name = value;
        }
        public string Type
        {
            get => type;
        }

        public int Level
        {
            get => level;
            set => level = value;
        }

        public Folder Parent { get; set; }
    }
}
