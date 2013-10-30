using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veg.Maths;

namespace OctreeTest
{
    public class AABB
    {
        private readonly Vect3 _start;
        private readonly Vect3 _end;

        public AABB(Vect3 start, Vect3 end)
        {
            _start = start;
            _end = end;
        }

        public bool Intersects(Triangle triangle)
        {
            //this is temp
            return
                triangle.P1.X > _start.X && triangle.P1.X < _end.X &&
                triangle.P1.Y > _start.Y && triangle.P1.Y < _end.Y &&
                triangle.P1.Z > _start.Z && triangle.P1.Z < _end.Z &&

                triangle.P2.X > _start.X && triangle.P2.X < _end.X &&
                triangle.P2.Y > _start.Y && triangle.P2.Y < _end.Y &&
                triangle.P2.Z > _start.Z && triangle.P2.Z < _end.Z &&

                triangle.P3.X > _start.X && triangle.P3.X < _end.X &&
                triangle.P3.Y > _start.Y && triangle.P3.Y < _end.Y &&
                triangle.P3.Z > _start.Z && triangle.P3.Z < _end.Z;
        }

    }

    public struct Triangle
    {
        public Vect3 P1;
        public Vect3 P2;
        public Vect3 P3;
        public Vect3 Normal;
    }
}
