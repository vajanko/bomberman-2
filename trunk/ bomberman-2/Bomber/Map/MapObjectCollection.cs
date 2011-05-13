using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Bomber
{
    class MapObjectCollection:IEnumerable
    {
        #region Data

        private int width;
        private int height;
        private MapObject[][] components;

        #endregion

        #region Properities

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public MapObject this[int x, int y]
        {
            get
            {
                if (validIndex(x, y)) return components[y][x];
                return null;
            }
            set
            {
                if (validIndex(x, y))
                    components[y][x] = value;
            }
        }
        private bool validIndex(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }

        #endregion

        public void Add(MapObject item, Point pos)
        {
            this[pos.X, pos.Y] = item;
        }
        public void Clear()
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    components[i][j] = null;
        }

        #region Constructors

        public MapObjectCollection(int width, int height)
        {
            this.width = width;
            this.height = height;
            // alocate memory
            components = new MapObject[height][];
            for (int i = 0; i < height; i++)
                components[i] = new MapObject[width];
            Clear();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (components[i][j] != null)
                        yield return components[i][j];
        }

        #endregion
    }
}
