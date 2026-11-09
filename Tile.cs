using System;

namespace GADE_POE_HEENA_JANA
{
    [Serializable]
    public abstract class Tile
    {
        private Position position;

        public enum TileType
        {
            Empty,
            Wall,
            Hero,
            Exit,
            Enemy,
            Pickup
        }

        public Position Position
        {
            get { return position; }
            set { position = value; }
        }

        public int X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public int Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public abstract char Display { get; }

        protected Tile(Position position)
        {
            this.position = position;
        }

        public override string ToString()
        {
            return Display.ToString();
        }
    }
}
