using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veg.Maths;

namespace OctreeTest
{

    //public class AABB
    //{


    //    public Vect3 Start { get; private set; }
    //    public Vect3 End { get; private set; }

    //    public Vect3 Center { get { return (Start + End)*0.5; } }

    //    

    //    public AABB(Vect3 start, Vect3 end)
    //    {
    //        Start = start;
    //        End = end;
    //    }



    //    //public IntersectResult Intersects(Sphere s)
    //    //{
    //    //    double sqDist = 0.0;
    //    //    int inside = 0;
    //    //    if (s.Center.X < _start.X)
    //    //        sqDist += (_start.X - s.Center.X) * (_start.X - s.Center.X);
    //    //    else if (s.Center.X > _end.X)
    //    //        sqDist += (s.Center.X - _end.X) * (s.Center.X - _end.X);
    //    //    else if (s.Center.X >= _start.X + s.Radius && s.Center.X <= _end.X - s.Radius)
    //    //        inside++;

    //    //    if (s.Center.Y < _start.Y)
    //    //        sqDist += (_start.Y - s.Center.Y) * (_start.Y - s.Center.Y);
    //    //    else if (s.Center.Y > _end.Y)
    //    //        sqDist += (s.Center.Y - _end.Y) * (s.Center.Y - _end.Y);
    //    //    else if (s.Center.Y >= _start.Y + s.Radius && s.Center.Y <= _end.Y - s.Radius)
    //    //        inside++;

    //    //    if (s.Center.Z < _start.Z)
    //    //        sqDist += (_start.Z - s.Center.Z) * (_start.Z - s.Center.Z);
    //    //    else if (s.Center.Z > _end.Z)
    //    //        sqDist += (s.Center.Z - _end.Z) * (s.Center.Z - _end.Z);
    //    //    else if (s.Center.Z >= _start.Z + s.Radius && s.Center.Z <= _end.Z - s.Radius)
    //    //        inside++;

    //    //    if(inside == 3) return IntersectResult.False;

    //    //    if(sqDist > s.Radius * s.Radius) return IntersectResult.True;

    //    //    return IntersectResult.Partial;
    //    //}


    //    //public bool Intersects(Triangle triangle)
    //    //{

    //    //    //this is temp

    //    //}



    //}



}
