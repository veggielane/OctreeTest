using System;
using System.Text;
using System.Threading.Tasks;
using Veg.Maths;

namespace OctreeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new Octree<Voxel>(Vect3.Zero, 256.0);
            tree.Split();
            tree[0].Split();
            tree[0][0].Split();
            tree[0][0][0].Split();
            tree[0][0][0][0].Split();
            tree[0][0][0][0][0].Split();
            tree[0][0][0][0][0][0].Split();
            tree[0][0][0][0][0][0][0].Split();

            using (var win = new Window(tree))
            {
                win.Run();
                Console.ReadLine();
            }
        }
    }
}
