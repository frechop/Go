using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    public struct BoardPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public BoardPoint (int x, int y) :this()
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (((BoardPoint)obj).X == X && ((BoardPoint)obj).Y == Y); 
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static BoardPoint Empty = new BoardPoint(-1, -1);
    }
}
