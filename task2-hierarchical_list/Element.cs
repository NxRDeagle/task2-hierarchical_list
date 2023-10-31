using System;

namespace task2_hierarchical_list
{
    class Element // Базовый класс объектов иерархического списка
    {
        private static int keyCount = 0;
        private int key, parentKey, level;
        private string type, name;

        protected Element(int parentKey, string type, string name, int level)
        {
            key = keyCount;
            keyCount++;
            this.parentKey = parentKey;
            this.type = type;
            this.name = name;
            this.level = level;
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
