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


        public static bool Intersects(this Triangle tri, AABB b)
        {
            if (tri.P1.Inside(b) && tri.P2.Inside(b) && tri.P1.Inside(b)) return true;

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


            return planeBoxOverlap(e0.CrossProduct(v2), v0, b.HalfSize);
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
            double vminx, vminy, vminz;
            double vmaxx, vmaxy, vmaxz;
            if (normal.X > 0.0)
            {
                vminx = -halfboxsize.X - vert.X;
                vmaxx = halfboxsize.X - vert.X;
            }
            else
            {
                vminx = halfboxsize.X - vert.X;
                vmaxx = -halfboxsize.X - vert.X;
            }
            if (normal.Y > 0.0)
            {
                vminy = -halfboxsize.Y - vert.Y;
                vmaxy = halfboxsize.Y - vert.Y;
            }
            else
            {
                vminy = halfboxsize.Y - vert.Y;
                vmaxy = -halfboxsize.Y - vert.Y;
            }

            if (normal.Z > 0.0)
            {
                vminz = -halfboxsize.Z - vert.Z;
                vmaxz = halfboxsize.Z - vert.Z;
            }
            else
            {
                vminz = halfboxsize.Z - vert.Z;
                vmaxz = -halfboxsize.Z - vert.Z;
            }


            if (normal.DotProduct(new Vect3(vminx, vminy, vminz)) > 0.0) return false;
          if (normal.DotProduct(new Vect3(vmaxx, vmaxy, vmaxz)) >= 0.0) return true;
          return false;
        }
    }
}