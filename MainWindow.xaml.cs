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
                BoardPoint currentPoint = new BoardPoint(e.Position.X, e.Position.Y);
                BoardPainter.StoneList.Add(currentPoint, e.StoneColor);

                getNeighbors(currentPoint);

                BoardPainter.ToPlay = e.StoneColor ^ Stone.White;
                AnalysisBoard.ToPlay = e.StoneColor ^ Stone.White;
            }
            BoardPainter.Redraw();
            AnalysisBoard.Redraw();
        }

        public void getNeighbors(BoardPoint currentPoint)
        {
            if (currentPoint.X > 0)
            {
                Stone leftStone;
                List<BoardPoint> boardPoints = BoardPainter.StoneList.Keys.ToList();
                BoardPoint leftPoint = boardPoints.Where(x => x.X == currentPoint.X - 1 && x.Y == currentPoint.Y).FirstOrDefault();
                if (leftPoint != null)
                {
                    leftStone = BoardPainter.StoneList[leftPoint];

                    if (leftStone == BoardPainter.StoneList[currentPoint] && !leftPoint.IsInChain)
                    {
                        BoardPainter.Chains.Add(new List<BoardPoint> { currentPoint, leftPoint });
                        leftPoint.IsInChain = true;
                        currentPoint.IsInChain = true;
                    }
                    else if (leftPoint.IsInChain)
                    {
                        setChain(leftPoint, currentPoint);
                    }
                }
            }

        }

        public void setChain(BoardPoint neighborPoint, BoardPoint currentPoint)
        {
            foreach (List<BoardPoint> chain in BoardPainter.Chains)
            {
                if (chain.Contains(neighborPoint))
                {
                    chain.Add(currentPoint);
                }
            }
        }

    }
}
