using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Veg.Maths;
using Veg.Maths.Geometry;

namespace OctreeTest
{
    public enum STLType
    {
        ASCII,
        Binary
    }


    public class STL
    {
        public readonly Color Color;
        public readonly List<Triangle> Elements = new List<Triangle>();

        public STL(string filename, Color color, STLType format = STLType.Binary)
        {
            Color = color;
            switch (format)
            {
                case STLType.ASCII:
                    LoadASCII(filename);
                    break;
                case STLType.Binary:
                    LoadBinary(filename);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }
        private void LoadBinary(string filename)
        {
            using (var br = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                br.ReadBytes(80); //header
                var count = (int)br.ReadUInt32();
                for (var i = 0; i < count; i++)
                {
                    var normal = new Vect3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    var p1 = new Vect3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    var p2 = new Vect3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    var p3 = new Vect3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    br.ReadUInt16(); //attrib
                    Elements.Add(new Triangle { P1 = p1, P2 = p2, P3 = p3, Normal = normal });
                }
            }
        }
        private void LoadASCII(string filename)
        {
            Vect3 normal = null;
            var points = new Vect3[3];
            int i = 0;
            const NumberStyles style = NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.Number;
            foreach (var split in File.ReadLines(filename).Select(line => line.Trim().ToLower().Split(' ')))
            {
                switch (split[0])
                {
                    case "solid":
                        break;
                    case "facet":
                        normal = new Vect3(Double.Parse(split[2], style), Double.Parse(split[3], style),
                                           Double.Parse(split[4], style));
                        break;
                    case "outer":
                        break;
                    case "vertex":
                        points[i++] = new Vect3(Double.Parse(split[1], style), Double.Parse(split[2], style),
                                                Double.Parse(split[3], style));
                        break;
                    case "endloop":
                        break;
                    case "endfacet":
                        Elements.Add(new Triangle { P1 = points[0], P2 = points[1], P3 = points[2], Normal = normal });
                        i = 0;
                        break;
                    case "endsolid":
                        break;
                }
            }
        }
    }
}