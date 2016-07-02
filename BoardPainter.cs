using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Go
{
   public class BoardPainter : FrameworkElement
    {
        public static readonly RoutedEvent MovePlayedEvent = EventManager.RegisterRoutedEvent("MovePlayed", RoutingStrategy.Bubble, typeof(MovePlayedEventHandler), typeof(BoardPainter));
        public static readonly DependencyProperty BoardSizeProperty = DependencyProperty.Register("BoardSize", typeof(int), typeof(BoardPainter), new FrameworkPropertyMetadata(9, new PropertyChangedCallback(OnBoardSizeChanged)), new ValidateValueCallback(BoardSizeValidateCallback));
        public static readonly DependencyProperty MouseHoverTypeProperty = DependencyProperty.Register("MouseHoverType", typeof(BoardHoverType), typeof(BoardPainter), new FrameworkPropertyMetadata(BoardHoverType.None, new PropertyChangedCallback(OnMouseHoverTypeChanged)));

        public delegate void MovePlayedEventHandler(object sender, RoutedMovePlayedEventArgs args);

        public event MovePlayedEventHandler MovePlayed
        {
            add { AddHandler(MovePlayedEvent, value); }
            remove { RemoveHandler(MovePlayedEvent, value); }
        }


        private List<Visual> _Visuals = new List<Visual>();
        private Dictionary<BoardPoint, Stone> _StoneList = new Dictionary<BoardPoint, Stone>();
        private ObservableCollection<BoardAnnotation> _AnnotationsList = new ObservableCollection<BoardAnnotation>();
        private Stone _ToPlay = Stone.Black;

        private DrawingVisual _BoardVisual, _StonesVisual, _StarPointVisual, _CoordinatesVisual, _AnnotationVisual, _MouseHoverVisual;
        private Brush _BlackStoneBrush, _WhiteStoneBrush, _BoardBrush, _StoneShadowBrush, _BlackStoneShadowBrush, _WhiteStoneShadowBrush;
        private Pen _BlackStoneAnnotationPen, _WhiteStoneAnnotationPen, _BlackPen;
        private Typeface _BoardTypeface;

        private Rect _GoBoardRect;
        private Rect _GoBoardHitBox;
        private BoardPoint _MousePosition;
        private string[] _Coordinates = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J"};
        private int _Border = 20;
        private int _BoardSize;
        private double _BoardWidthFactor = 14;
        private double _BoardHeightFactor = 15;


        public BoardPainter()
        {
            Resources.Source = new Uri("pack://application:,,,/Go;component/BoardPainterResources.xaml");

            _BlackStoneBrush = (Brush)TryFindResource("blackStoneBrush");
            _WhiteStoneBrush = (Brush)TryFindResource("whiteStoneBrush");
            _BoardBrush = (Brush)TryFindResource("boardBrush");
            _StoneShadowBrush = (Brush)TryFindResource("stoneShadowBrush");
            _WhiteStoneAnnotationPen = (Pen)TryFindResource("whiteStoneAnnotationPen");
            _BlackStoneAnnotationPen = (Pen)TryFindResource("blackStoneAnnotationPen");
            _BlackPen = (Pen)TryFindResource("blackPen");
            _BlackStoneShadowBrush = (Brush)TryFindResource("blackStoneShadowBrush");
            _WhiteStoneShadowBrush = (Brush)TryFindResource("whiteStoneShadowBrush");

            _BoardTypeface = new Typeface("Arial");

            InitializeBoard(this.BoardSize - 1);
        }


        #region Draw Methods

        private void DrawBoard()
        {
           _BoardVisual = new DrawingVisual();

            using (DrawingContext dc = _BoardVisual.RenderOpen())
            {
                dc.DrawRectangle(_BoardBrush, new Pen(Brushes.Black, 0.2), new Rect(0, 0, _BoardSize * _BoardWidthFactor + _Border * 2, _BoardSize * _BoardHeightFactor + _Border * 2));
                dc.DrawRectangle(_BoardBrush, new Pen(Brushes.Black, 0.2), new Rect(_Border, _Border, _BoardSize * _BoardWidthFactor, _BoardSize * _BoardHeightFactor));

                for (int x = 0; x <_BoardSize; x++)
                {
                    for (int y = 0; y < _BoardSize; y++)
                    {
                        dc.DrawRectangle(null, _BlackPen, new Rect(getPosX(x), getPosY(y), _BoardWidthFactor, _BoardHeightFactor));
                    }
                }
            }
        }

        public void DrawStones()
        {
            _BoardVisual.Children.Remove(_StonesVisual);
            _StonesVisual = new DrawingVisual();

            using (DrawingContext dc = _StonesVisual.RenderOpen())
            {
                foreach (var item in _StoneList)
                {
                    double posX = getPosX(item.Key.X);
                    double posY = getPosY(item.Key.Y);

                    dc.DrawEllipse(_StoneShadowBrush, null, new Point(posX + 1, posY + 1), 6.7, 6.7);
                    dc.DrawEllipse(((item.Value == Stone.White) ? _WhiteStoneBrush : _BlackStoneBrush), _BlackPen, new Point(posX, posY), _BoardWidthFactor / 2 - 0.5, _BoardWidthFactor / 2 - 0.5);
                }
            }

            _BoardVisual.Children.Add(_StonesVisual);
        }

        private void DrawStarPoints()
        {
            List<Point> starPointList = new List<Point>();

            if (_BoardSize == 18)
            {
                starPointList.Add(new Point(getPosX(3), getPosY(3)));
                starPointList.Add(new Point(getPosX(3), getPosY(9)));
                starPointList.Add(new Point(getPosX(3), getPosY(15)));
                starPointList.Add(new Point(getPosX(9), getPosY(3)));
                starPointList.Add(new Point(getPosX(9), getPosY(9)));
                starPointList.Add(new Point(getPosX(9), getPosY(15)));
                starPointList.Add(new Point(getPosX(15), getPosY(3)));
                starPointList.Add(new Point(getPosX(15), getPosY(9)));
                starPointList.Add(new Point(getPosX(15), getPosY(15)));
            }
            else if (_BoardSize == 12)
            {
                starPointList.Add(new Point(getPosX(3), getPosY(3)));
                starPointList.Add(new Point(getPosX(3), getPosY(6)));
                starPointList.Add(new Point(getPosX(3), getPosY(9)));
                starPointList.Add(new Point(getPosX(6), getPosY(3)));
                starPointList.Add(new Point(getPosX(6), getPosY(6)));
                starPointList.Add(new Point(getPosX(6), getPosY(9)));
                starPointList.Add(new Point(getPosX(9), getPosY(3)));
                starPointList.Add(new Point(getPosX(9), getPosY(6)));
                starPointList.Add(new Point(getPosX(9), getPosY(9)));
            }
            else if (_BoardSize == 8)
            {
                starPointList.Add(new Point(getPosX(2), getPosY(2)));
                starPointList.Add(new Point(getPosX(2), getPosY(6)));
                starPointList.Add(new Point(getPosX(4), getPosY(4)));
                starPointList.Add(new Point(getPosX(6), getPosY(2)));
                starPointList.Add(new Point(getPosX(6), getPosY(6)));
            }

            _BoardVisual.Children.Remove(_StarPointVisual);
            _StarPointVisual = new DrawingVisual();

            using (DrawingContext dc = _StarPointVisual.RenderOpen())
            {
                starPointList.ForEach(delegate (Point p)
                {
                    dc.DrawGeometry(Brushes.Black, _BlackPen, new EllipseGeometry(p, 1.2, 1.2));
                });
            }

            _BoardVisual.Children.Add(_StarPointVisual);
        }

        private void DrawCoordinates()
        {
            _BoardVisual.Children.Remove(_CoordinatesVisual);
            _CoordinatesVisual = new DrawingVisual();

            using (DrawingContext dc = _CoordinatesVisual.RenderOpen())
            {
                for (int i = 0; i < _BoardSize + 1; i++)
                {
                    double posX = 3;
                    double posY = getPosY(i) - 3;

                    dc.DrawText(new FormattedText((_BoardSize + 1 - i).ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _BoardTypeface, 4, Brushes.Black), new Point(posX, posY));

                    posX = getPosX(i) - 1;
                    posY = getPosY(_BoardSize) + _Border / 2;

                    dc.DrawText(new FormattedText(_Coordinates[i], CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _BoardTypeface, 4, Brushes.Black), new Point(posX, posY));
                }
            }

            _BoardVisual.Children.Add(_CoordinatesVisual);
        }

        private void DrawAnnotations()
        {
            _BoardVisual.Children.Remove(_AnnotationVisual);
            _AnnotationVisual = new DrawingVisual();

            using (DrawingContext dc = _AnnotationVisual.RenderOpen())
            {
                foreach (var anno in _AnnotationsList)
                {
                    Stone stone = _StoneList.ContainsKey(anno.Position) ? _StoneList[anno.Position] : Stone.Empty;
                    Pen annoPen = (stone != Stone.Empty && stone == Stone.Black) ? _BlackStoneAnnotationPen : _WhiteStoneAnnotationPen;
                    Brush annoColor = (stone != Stone.Empty && stone == Stone.Black) ? Brushes.White : Brushes.Black;

                    switch (anno.Type)
                    {
                        case BoardAnnotationType.Circle:
                            dc.DrawEllipse(Brushes.Transparent, annoPen, new Point(getPosX(anno.Position.X), getPosY(anno.Position.Y)), _BoardWidthFactor / 4, _BoardWidthFactor / 4);
                            break;
                        case BoardAnnotationType.Rectangle:
                            dc.DrawRectangle(Brushes.Transparent, annoPen, new Rect(new Point(getPosX(anno.Position.X) - _BoardWidthFactor / 4, getPosY(anno.Position.Y) - _BoardHeightFactor / 4), new Size(_BoardWidthFactor / 2, _BoardHeightFactor / 2)));
                            break;
                        case BoardAnnotationType.Label:
                            FormattedText text = new FormattedText(anno.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _BoardTypeface, 8, annoColor);
                            dc.DrawRectangle(stone == Stone.Empty ? _BoardBrush : Brushes.Transparent, null, new Rect(new Point(getPosX(anno.Position.X) - _BoardWidthFactor / 4, getPosY(anno.Position.Y) - _BoardHeightFactor / 4), new Size(_BoardWidthFactor / 2, _BoardHeightFactor / 2)));
                            dc.DrawText(text, new Point(getPosX(anno.Position.X) - text.Width / 2, getPosY(anno.Position.Y) - text.Height / 2));
                            break;
                        case BoardAnnotationType.Triangle:
                            string first = getPosX(anno.Position.X) + "," + (getPosY(anno.Position.Y) - 5).ToString().Replace(',', '.');
                            string second = (getPosX(anno.Position.X) - 4).ToString().Replace(',', '.') + "," + (getPosY(anno.Position.Y) + 3).ToString().Replace(',', '.');
                            string third = (getPosX(anno.Position.X) + 4).ToString().Replace(',', '.') + "," + (getPosY(anno.Position.Y) + 3).ToString().Replace(',', '.');
                            dc.DrawGeometry(Brushes.Transparent, annoPen, Geometry.Parse("M " + first + " L " + second + " L " + third + " Z"));
                            break;
                        default:
                            break;
                    }
                }
            }

           _BoardVisual.Children.Add(_AnnotationVisual);
        }

        private void DrawMouseHoverVisual()
        {
           _BoardVisual.Children.Remove(_MouseHoverVisual);
           _MouseHoverVisual = new DrawingVisual();

            using (DrawingContext dc = _MouseHoverVisual.RenderOpen())
            {
                switch (MouseHoverType)
                {
                    case BoardHoverType.Stone:
                        if (_MousePosition.Equals(BoardPoint.Empty) || _StoneList.ContainsKey(_MousePosition)) break;
                        double posX = getPosX(_MousePosition.X);
                        double posY = getPosY(_MousePosition.Y);

                        dc.DrawEllipse(((_ToPlay == Stone.White) ? _WhiteStoneShadowBrush : _BlackStoneShadowBrush), null, new Point(posX, posY), _BoardWidthFactor / 2 - 0.5, _BoardWidthFactor / 2 - 0.5);
                        break;
                    case BoardHoverType.None:
                    default:
                        break;
                }
            }

            _BoardVisual.Children.Add(_MouseHoverVisual);
        }

        #endregion


        private void InitializeBoard(int boardSize)
        {
            _Visuals.ForEach(delegate (Visual v) { RemoveVisualChild(v); });

            _Visuals.Clear();
            _StoneList.Clear();
            _AnnotationsList.Clear();
            _AnnotationsList.CollectionChanged += new NotifyCollectionChangedEventHandler(m_AnnotationsList_CollectionChanged);

            _BoardSize = boardSize;

            _GoBoardRect = new Rect(new Size(_BoardSize * _BoardWidthFactor, _BoardSize * _BoardHeightFactor));
            _GoBoardHitBox = _GoBoardRect;
            _GoBoardHitBox.Inflate((_BoardWidthFactor / 2), (_BoardHeightFactor / 2));

            this.Width = _GoBoardRect.Width + _Border * 2;
            this.Height = _GoBoardRect.Height + _Border * 2;

            DrawBoard();
            DrawCoordinates();
            DrawStarPoints();
            DrawStones();
            DrawMouseHoverVisual();

            _Visuals.Add(_BoardVisual);

            _Visuals.ForEach(delegate (Visual v) { AddVisualChild(v); });
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Point pos = e.GetPosition(this);

            if (!_GoBoardHitBox.Contains(new Point(pos.X - _Border, pos.Y - _Border))) return;

            int x = (int)Math.Round((pos.X - _Border) / (_GoBoardRect.Width / _BoardSize));
            int y = (int)Math.Round((pos.Y - _Border) / (_GoBoardRect.Height / _BoardSize));

            RaiseEvent(new RoutedMovePlayedEventArgs(MovePlayedEvent, this, new BoardPoint(x, y), _ToPlay));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point pos = e.GetPosition(this);

            if (!_GoBoardHitBox.Contains(new Point(pos.X - _Border, pos.Y - _Border)))
            {
                _MousePosition = BoardPoint.Empty;
                DrawMouseHoverVisual();
                return;
            }

            int x = (int)Math.Round((pos.X - _Border) / (_GoBoardRect.Width / _BoardSize));
            int y = (int)Math.Round((pos.Y - _Border) / (_GoBoardRect.Height / _BoardSize));

            _MousePosition = new BoardPoint(x, y);
            DrawMouseHoverVisual();
        }

        private void m_AnnotationsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DrawAnnotations();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _Visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _Visuals.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _Visuals[index];
        }

        private double getPosX(double value)
        {
            return _BoardWidthFactor * value + _Border;
        }

        private double getPosY(double value)
        {
            return _BoardHeightFactor * value + _Border;
        }

        private static void OnBoardSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as BoardPainter).InitializeBoard((sender as BoardPainter).BoardSize - 1);
        }

        private static void OnMouseHoverTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
        }

        private static bool BoardSizeValidateCallback(object target)
        {
            if ((int)target < 2 || (int)target > 19)
                return false;

            return true;
        }

        public void Redraw()
        {
            DrawStones();
            DrawAnnotations();
        }

        public int BoardSize
        {
            get { return (int)GetValue(BoardSizeProperty); }
            set { SetValue(BoardSizeProperty, value); }
        }

        public BoardHoverType MouseHoverType
        {
            get { return (BoardHoverType)GetValue(MouseHoverTypeProperty); }
            set { SetValue(MouseHoverTypeProperty, value); }
        }

        public Stone ToPlay
        {
            get { return _ToPlay; }
            set { _ToPlay = value; }
        }

        public Dictionary<BoardPoint, Stone> StoneList
        {
            get { return _StoneList; }
            set
            {
                _StoneList = value;
                DrawStones();
            }
        }

        public ObservableCollection<BoardAnnotation> AnnotationList
        {
            get { return _AnnotationsList; }
            set
            {
                _AnnotationsList = value;
                DrawAnnotations();
            }
        }
    }
}
