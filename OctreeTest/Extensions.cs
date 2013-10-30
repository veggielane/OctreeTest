using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctreeTest
{
    public static class Extensions
    {
        public static IEnumerable<OctreeNode<T>> Flatten<T>(this OctreeNode<T> e) where T : IOctVoxel
        {
            yield return e;
            if (e.Children != null)
            {
                foreach (var child in e.Children)
                {
                    foreach (var c in Flatten(child))
                    {
                        yield return c;
                    }
                }
            }
        }
    }
}
