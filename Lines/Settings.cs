namespace Lines
{
    static class Settings
    {
        public static int FieldSideCellCount { get { return 11; } }
        public static int FieldCellCount { get { return FieldSideCellCount * FieldSideCellCount; } }
        public static int SpawnBallQtyMin { get { return 3; } }
        public static int SpawnBallQtyMax { get { return 3; } }
    }
}
