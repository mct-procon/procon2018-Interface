using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    public class Point : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point() { }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int CompareTo(Object other)
        {
            var point = (Point)other;
            if(this.X == point.X && this.Y == point.Y) return 0;
            return 1;
        }
    }
}