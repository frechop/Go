using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Go
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BoardPainter_MovePlayed(object sender, RoutedMovePlayedEventArgs e)
        {
            if (!BoardPainter.StoneList.ContainsKey(e.Position))
            {
                BoardPainter.StoneList.Add(new BoardPoint(e.Position.X, e.Position.Y), e.StoneColor);
                BoardPainter.ToPlay = e.StoneColor ^ Stone.White;
            }
            BoardPainter.Redraw();
        }

        private void rdRectangle_Checked(object sender, RoutedEventArgs e)
        {
            BoardPainter.MouseHoverType = BoardHoverType.None;
        }

        private void rdStone_Checked(object sender, RoutedEventArgs e)
        {
            if (BoardPainter != null)
                BoardPainter.MouseHoverType = BoardHoverType.Stone;
        }
    }
}
