using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    public class BoardAnnotation
    {

        private BoardAnnotationType _Type;
        private BoardPoint _Position;
        private string _Text;

        public BoardAnnotation(BoardAnnotationType type, BoardPoint position)
            : this(type, position, "")
        {

        }

        public BoardAnnotation(BoardAnnotationType type, BoardPoint position, string text)
        {
            _Type = type;
            _Position = position;
            _Text = text;
        }

        public BoardAnnotationType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public BoardPoint Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
    }
}
