using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Veg.Maths;
using Veg.Maths.Geometry;

namespace OctreeTest
{
    [Serializable]
    public class Octree<T>: OctreeNode<T> where T : IOctVoxel
    {
        public Octree(Vect3 center, Double size)
            : base(center, size, 0)
        {

        }
    }
    [Serializable]
    public class Voxel:IOctVoxel
    {
        
    }

    public interface IOctVoxel
    {
        
    }
    [Serializable]
    public class OctreeNode<T> : IOperations<OctreeNode<T>> where T : IOctVoxel
    {
        

        public Vect3 Center { get; private set; }
        public Double Size { get; private set; }
        public int Level { get; private set; }

        public NodeState State { get; private set; }
        public OctreeNode<T>[] Children { get; private set; }

        public AABB AABB { get { return new AABB(Center + new Vect3(-Size / 2.0, -Size / 2.0, -Size / 2.0), Center + new Vect3(Size / 2.0, Size / 2.0, Size / 2.0)); } }

        public OctreeNode(Vect3 center, Double size, int level)
        {
            Center = center;
            Size = size;

            Level = level;
            State = NodeState.Empty;
            Children = null;
        }

        public OctreeNode(Vect3 center, Double size, int level, OctreeNode<T>[] children)
        {
            Center = center;
            Size = size;
            Level = level;
            State = NodeState.Partial;
            Children = children;
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
            State = NodeState.Partial;
        }

        public void Fill()
        {
            Children = null;
            State = NodeState.Filled;
        }

        public void Clear()
        {
            Children = null;
            State = NodeState.Empty;
        }

        public OctreeNode<T> this[int index]
        {
            get {
                if(Children==null || index > 7 || index < 0) throw new Exception();
                return Children[index];
            }
        }


        //public void Test(Func<OctreeNode<T>, IntersectResult> func, int maxLevel)
        //{
        //    if (Level >= maxLevel) return;

        //    switch (func(this))
        //    {
        //        case IntersectResult.Outside:
        //            Clear();
        //            break;
        //        case IntersectResult.Inside:
        //            Fill();
        //            break;
        //        case IntersectResult.Partial:
        //            if (Children == null)
        //            {
        //                Split();
        //            }
        //            if (Children != null)
        //            {
        //                foreach (var node in Children)
        //                {
        //                    node.Test(func, maxLevel);
        //                }
        //            }
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

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

        public OctreeNode<T> Copy()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;
                return (OctreeNode<T>)formatter.Deserialize(ms);
            }
        }

        private OctreeNode<T> Copy(OctreeNode<T> node)
        {
            return new OctreeNode<T>(node.Center,node.Size,node.Level)
                {
                    State = node.State,
                    Children = CopyChildren(node)
                };
        }
        private OctreeNode<T>[] CopyChildren(OctreeNode<T> node)
        {
            return Children.Select(octreeNode => Copy(octreeNode)).ToArray();
        }


        //public void Test(Func<OctreeNode<T>, bool> func, int maxLevel)
        //{
        //    throw new NotImplementedException();
        //    if (Level > maxLevel) return;

        //    if (func(this))
        //    {
        //        if (Level == maxLevel)
        //        {
        //            Fill();
        //        }
        //        else
        //        {
        //            if (Children == null)
        //            {
        //                Split();
        //            }
        //            Parallel.ForEach(Children, child => Test(func, maxLevel));
        //        }
        //    }
        //}



        public OctreeNode<T> Union( OctreeNode<T> b)
        {

            return Test(this,b);
        }

        public OctreeNode<T> Subtract( OctreeNode<T> b)
        {
            throw new NotImplementedException();
        }

        public OctreeNode<T> Intersection( OctreeNode<T> b)
        {
            throw new NotImplementedException();
        }

        public OctreeNode<T> Invert()
        {
            throw new NotImplementedException();
        }


        private static OctreeNode<T> Test(OctreeNode<T> nodea, OctreeNode<T> nodeb)
        {

            if (nodea.State == NodeState.Empty && nodeb.State == NodeState.Empty)
            {
                return new OctreeNode<T>(nodea.Center, nodea.Size, nodea.Level);
            }

            if (
                (nodea.State == NodeState.Filled && nodeb.State == NodeState.Filled) 
                || (nodea.State == NodeState.Filled && nodeb.State == NodeState.Empty)
                || (nodea.State == NodeState.Empty && nodeb.State == NodeState.Filled)
                || (nodea.State == NodeState.Partial && nodeb.State == NodeState.Filled)
                || (nodea.State == NodeState.Filled && nodeb.State == NodeState.Partial)
                )
            {
                var n = new OctreeNode<T>(nodea.Center, nodea.Size, nodea.Level);
                n.Fill();
                return n;
            }
            if ((nodea.State == NodeState.Partial && nodeb.State == NodeState.Partial) && nodea.Children != null && nodeb.Children != null)
            {
                return new OctreeNode<T>(nodea.Center,nodea.Size,nodea.Level, nodea.Children.Zip(nodeb.Children, Test).ToArray());
            }
            if (nodea.State == NodeState.Partial && nodeb.State == NodeState.Empty)
            {
                return nodea;
            }
            if (nodea.State == NodeState.Empty && nodeb.State == NodeState.Partial)
            {
                return nodeb;
            }
            throw new Exception();





            //if (nodea.Children != null)
            //{
            //    var changed = false;
            //    var tempchildren = nodea.Children.Select(n =>
            //        {
            //            var result = Test(n, nodeb);
            //            if (result != n)
            //                changed = true;

            //            return result;
            //        });

            //    if(changed)
            //        return new OctreeNode<T>(nodea.Center, nodea.Size, nodea.Level, tempchildren);

            //}


            //return nodea;

            //if (node.Level > maxLevel) return;

            //var result = triangles.Where(t => t.Intersects(node.AABB)).ToList();
            //if (result.Any())
            //{
            //    if (node.Level == maxLevel)
            //    {
            //        node.Fill();
            //    }
            //    else
            //    {

            //        if (node.Children == null)
            //        {
            //            node.Split();
            //        }
            //        Parallel.ForEach(node.Children, child => Test(child, result));
            //    }

            //}

        }
    }


    public enum NodeState:byte { Empty = 0, Filled = 1, Partial = 2 }



}
