namespace Lines
{
    class Node
    {
        int index, heuristic, movement;
        Node parent;

        public Node()
        {
            parent = null;
            movement = 0;
        }

        public int Heuristic
        {
            get { return heuristic; }
            set { heuristic = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public int Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        public int F { get { return heuristic + movement; } }

        internal Node Parent
        {
            get { return parent; }

            set { parent = value; }
        }
    }
}
