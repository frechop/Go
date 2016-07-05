using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    class Chain
    {
        private HashSet<BoardPoint> _stones;
        private HashSet<BoardPoint> _liberties;

        public Chain(HashSet<BoardPoint> stones, HashSet<BoardPoint> liberties)
        {
            this._stones = stones;
            this._liberties = liberties;
        }

        public HashSet<BoardPoint> getLiberties()
        {
            return _liberties;
        }

        public bool inAtari()
        {
            return _liberties.Count() == 1;
        }

        public HashSet<BoardPoint> getStones()
        {
            return _stones;
        }

    }
}
