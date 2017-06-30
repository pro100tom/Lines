namespace Lines
{
    delegate void DefaultEventHandler(object sender);
    delegate void MovementEventHandler(double coordinate, Direction direction);

    enum Direction { Left, Up, Right, Down };
}
