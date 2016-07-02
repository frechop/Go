using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Go
{
   public class RoutedMovePlayedEventArgs : RoutedEventArgs
    {

        public BoardPoint Position { get; set; }
        public Stone StoneColor { get; set; }

        public RoutedMovePlayedEventArgs(RoutedEvent routedEvent, object source, BoardPoint pos, Stone stoneColor)
            : base(routedEvent, source)
        {
            Position = pos;
            StoneColor = stoneColor;
        }
    }
}
