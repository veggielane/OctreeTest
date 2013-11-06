using System;
using Veg.Maths;
using Veg.Maths.Geometry;

namespace OctreeTest
{
    public static class IntersectionExtensions
    {        
        
        public static bool Inside(this Vect3 v, AABB b)
        {
            return v.X > b.Min.X && v.X < b.Max.X && v.Y > b.Min.Y && v.Y < b.Max.Y && v.Z > b.Min.Z && v.Z < b.Max.Z;
        }

        public static bool Intersects(this Plane p, AABB b)
        {
            var e = b.Max - b.Center;
            var r = e.X*Math.Abs(p.Normal.X) + e.Y*Math.Abs(p.Normal.Y) + e.Z*Math.Abs(p.Normal.Z);
            var s = p.Normal.DotProduct(b.Center) - p.Distance;
            return Math.Abs(s) <= r;
        }


        public static bool Intersects(this AABB b1, AABB b2)
        {
            var t = b2.Center - b1.Center;
            return Math.Abs(t.X) <= (b1.HalfSize.X + b2.HalfSize.X)
                && Math.Abs(t.Y) <= (b1.HalfSize.Y + b2.HalfSize.Y)
                && Math.Abs(t.Z) <= (b1.HalfSize.Z + b2.HalfSize.Z);
        }

        public static bool Intersects(this Sphere s, AABB b)
        {
            return b.SqDistPointAABB(s.Center) <= Math.Pow(s.Radius,2);
        }

        private static double SqDistPointAABB(this AABB b, Vect3 p)
        {
            var sqDist = 0.0;
            if (p.X < b.Min.X) sqDist += (b.Min.X - p.X) * (b.Min.X - p.X);
            if (p.X > b.Max.X) sqDist += (p.X - b.Max.X) * (p.X - b.Max.X);

            if (p.Y < b.Min.Y) sqDist += (b.Min.Y - p.Y) * (b.Min.Y - p.Y);
            if (p.Y > b.Max.Y) sqDist += (p.Y - b.Max.Y) * (p.Y - b.Max.Y);

            if (p.Z < b.Min.Z) sqDist += (b.Min.Z - p.Z) * (b.Min.Z - p.Z);
            if (p.Z > b.Max.Z) sqDist += (p.Z - b.Max.Z) * (p.Z - b.Max.Z);

            return sqDist;
        }

        public static bool Intersects(this Triangle tri, AABB b)
        {
            if (tri.P1.Inside(b) && tri.P2.Inside(b) && tri.P3.Inside(b)) return true;

            var v0 = tri.P1 - b.Center;
            var v1 = tri.P2 - b.Center;
            var v2 = tri.P3 - b.Center;

            var e0 = v1 - v0;
            var e1 = v2 - v1;
            var e2 = v0 - v2;

            double fex, fey, fez;

            fex = Math.Abs(e0.X);
            fey = Math.Abs(e0.Y);
            fez = Math.Abs(e0.Z);
            if (AXISTEST_X(v0, v2, e0.Z, e0.Y, fez, fey, b.HalfSize)) return false;
            if (AXISTEST_Y(v0, v2, e0.Z, e0.X, fez, fex, b.HalfSize)) return false;
            if (AXISTEST_Z(v1, v2, e0.Y, e0.X, fey, fex, b.HalfSize)) return false;

            fex = Math.Abs(e1.X);
            fey = Math.Abs(e1.Y);
            fez = Math.Abs(e1.Z);

            if (AXISTEST_X(v0, v2, e1.Z, e1.Y, fez, fey, b.HalfSize)) return false;
            if (AXISTEST_Y(v0, v2, e1.Z, e1.X, fez, fex, b.HalfSize)) return false;
            if (AXISTEST_Z(v0, v1, e1.Y, e1.X, fey, fex, b.HalfSize)) return false;

            fex = Math.Abs(e2.X);
            fey = Math.Abs(e2.Y);
            fez = Math.Abs(e2.Z);

            if (AXISTEST_X(v0, v1, e2.Z, e2.Y, fez, fey, b.HalfSize)) return false;
            if (AXISTEST_Y(v0, v1, e2.Z, e2.X, fez, fex, b.HalfSize)) return false;
            if (AXISTEST_Z(v1, v2, e2.Y, e2.X, fey, fex, b.HalfSize)) return false;

            var normal = e0.CrossProduct(e1);
            return new Plane(normal, normal.DotProduct(v0)).Intersects(b);
        }

        private static bool AXISTEST_X(Vect3 va, Vect3 vb, double a, double b, double fa, double fb, Vect3 halfboxsize)
        {
            var p0 = a * va.Y - b * va.Z;
            var p2 = a * vb.Y - b * vb.Z;
            double min, max;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }
            var rad = fa * halfboxsize.Y + fb * halfboxsize.Z;
            return (min > rad || max < -rad);
        }

        private static bool AXISTEST_Y(Vect3 va, Vect3 vb, double a, double b, double fa, double fb, Vect3 halfboxsize)
        {
            var p0 = -a * va.X + b * va.Z;
            var p2 = -a * vb.X + b * vb.Z;
            double min, max;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }
            var rad = fa * halfboxsize.X + fb * halfboxsize.Z;
            return (min > rad || max < -rad);
        }

        private static bool AXISTEST_Z(Vect3 va, Vect3 vb, double a, double b, double fa, double fb, Vect3 halfboxsize)
        {
            var p0 = a * va.X - b * va.Y;
            var p2 = a * vb.X - b * vb.Y;
            double min, max;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }
            var rad = fa * halfboxsize.X + fb * halfboxsize.Y;
            return (min > rad || max < -rad);
        }
       
        private static bool planeBoxOverlap(Vect3 normal, Vect3 vert, Vect3 halfboxsize)
        {
 
            var min = new Vect3(
                normal.X > 0.0 ? -halfboxsize.X - vert.X :halfboxsize.X - vert.X,
                normal.Y > 0.0 ? -halfboxsize.Y - vert.Y :halfboxsize.Y - vert.Y,
                normal.Z > 0.0 ? -halfboxsize.Z - vert.Z :halfboxsize.Z - vert.Z
                );
            var max = new Vect3(
                normal.X > 0.0 ? halfboxsize.X - vert.X :-halfboxsize.X - vert.X,
                normal.Y > 0.0 ? halfboxsize.Y - vert.Y :-halfboxsize.Y - vert.Y,
                normal.Z > 0.0 ? halfboxsize.Z - vert.Z :-halfboxsize.Z - vert.Z
                );

          if (normal.DotProduct(min) > 0.0) return false;
          if (normal.DotProduct(max) >= 0.0) return true;
          return false;
        }
    }


    public interface IOperations<T>
    {
        T Union(T b);
        T Subtract(T b);
        T Intersection(T b);
        T Invert();
    }
}