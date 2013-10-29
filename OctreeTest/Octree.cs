using System;
using System.Text;
using Veg.Maths;

namespace OctreeTest
{
    public class Octree<T>: OctreeNode<T> where T : IOctVoxel
    {
        public Octree(Vect3 center, Double size)
            : base(center, size, 0)
        {

        }
    }

    public class Voxel:IOctVoxel
    {
        
    }

    public interface IOctVoxel
    {
        
    }

    public class OctreeNode<T>
        where T : IOctVoxel
    {
        

        public Vect3 Center { get; private set; }
        public Double Size { get; private set; }
        public int Level { get; private set; }

        public NodeState State { get; private set; }
        public OctreeNode<T>[] Children { get; private set; }

        public OctreeNode(Vect3 center, Double size, int level)
        {
            Center = center;
            Size = size;

            Level = level;
            State = NodeState.Empty;
            Children = null;
        }

        public void Split()
        {
            var newSize = Size / 2.0;
            var half = Size / 4.0;

            Children = new[]
            {
                //top-front-right
                new OctreeNode<T>(Center + new Vect3(+half, +half, +half),newSize,Level + 1),
                //top-back-right
                new OctreeNode<T>(Center + new Vect3(-half, +half, +half),newSize,Level + 1),
                //top-back-left
                new OctreeNode<T>(Center + new Vect3(-half, -half, +half),newSize,Level + 1),
                //top-front-left
                new OctreeNode<T>(Center + new Vect3(+half, -half, +half),newSize,Level + 1),
                //bottom-front-right
                new OctreeNode<T>(Center + new Vect3(+half, +half, -half),newSize,Level + 1),
                //bottom-back-right
                new OctreeNode<T>(Center + new Vect3(-half, +half, -half),newSize,Level + 1),
                //bottom-back-left
                new OctreeNode<T>(Center + new Vect3(-half, -half, -half),newSize,Level + 1),
                //bottom-front-left
                new OctreeNode<T>(Center + new Vect3(+half, -half, -half),newSize,Level + 1)
            };
        }

        public OctreeNode<T> this[int index]
        {
            get {
                if(Children==null || index > 7 || index < 0) throw new Exception();
                return Children[index];
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Octree<{0},{1},{2}> [", Center.X, Center.Y, Center.Z);
            if (Children != null)
            {
                foreach (var node in Children)
                {
                    sb.AppendLine(node.ToString());
                }
            }
            sb.AppendLine("]");
            return sb.ToString();
        }
    }

    public enum NodeState:byte { Empty = 0, Filled = 1, Partial = 2 }
}
