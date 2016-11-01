namespace Assets.Scripts
{
    public class Square
    {
        public ControlNode TopLeft,
                           TopRight,
                           BottomLeft,
                           BottomRight;

        public int Configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;

            if (topLeft.Active) Configuration += 8;
            if (topRight.Active) Configuration += 4;
            if (bottomRight.Active) Configuration += 2;
            if (bottomLeft.Active) Configuration += 1;
        }
    }
}