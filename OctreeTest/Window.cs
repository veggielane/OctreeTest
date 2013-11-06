using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Veg.Maths;
using Veg.Maths.Geometry;
using Veg.OpenTK;
using Veg.OpenTK.Buffers;
using Veg.OpenTK.Camera;
using Veg.OpenTK.Shaders;
using Veg.OpenTK.Vertices;

namespace OctreeTest
{
    public enum IntersectResult{Outside,Inside,Partial};
    public class Window : GameWindow
    {
        private readonly OctreeNode<Voxel> _tree;
        private IShaderProgram _shader;
        private VAO _vao;
        private VAO _vao2;
        private VAO _vaoFilled;
        private CameraUBO _ubo;
        private readonly ICamera _camera;
        STL stl;
        private int maxLevel = 7;

        public Window()
            : base(1280, 720, new GraphicsMode(32, 0, 0, 4), "OpenCAD")
        {
            
            stl = new STL("Models/elephant.stl", Color.Green, STLType.Binary);


            var s1 = new Sphere {Center = Vect3.Zero, Radius = 4};
            var s2 = new Sphere {Center = new Vect3(0,5,0), Radius = 4};
            var t1 = new Octree<Voxel>(Vect3.Zero, 32.0);
            var t2 = new Octree<Voxel>(Vect3.Zero, 32.0);

            Test2(t1, node => s1.Intersects(node.AABB));
            Test(t2, stl.Elements);

            _tree = t1.Union(t2);

            //_tree.Test(node => sphere.Intersects(node.AABB),maxLevel);
            //Test2(t, node => sphere.Intersects(node.AABB));

      
            //t[0].Clear();
            //t[0].Clear();
            //Test(_tree,stl.Elements);
            //create from stl
            //foreach (var tri in stl.Elements)
            //{
            //    Intersect(_tree, tri);
            //}


            VSync = VSyncMode.On;

            _camera = new Camera();
            Mouse.WheelChanged += (sender, args) =>
                {
                    _camera.View = _camera.View * Mat4.Translate(0, 0, args.DeltaPrecise * -10.0);
                    //_camera.Eye += new Vect3(0, 0, args.DeltaPrecise * -10.0);
                    // Console.WriteLine(_camera.Eye);
                };
        }



        void Test2(OctreeNode<Voxel> node, Func<OctreeNode<Voxel>, bool> func)
        {
            if (node.Level > maxLevel) return;

            if (func(node))
            {
                if (node.Level == maxLevel)
                {
                    node.Fill();
                }
                else
                {

                    if (node.Children == null)
                    {
                        node.Split();
                    }
                    Parallel.ForEach(node.Children, child => Test2(child, func));
                }
            }

  
            //else
            //{
            //    var result = triangles.Where(t => func(node)).ToList();
            //    if (result.Any())
            //    {
            //        if (node.Children == null)
            //        {
            //            node.Split();
            //        }
            //        Parallel.ForEach(node.Children, child => Test2(child, result, func));
            //    }
            //}
        }

        void Test(OctreeNode<Voxel> node, IEnumerable<Triangle> triangles)
        {
            if (node.Level > maxLevel) return;

            var result = triangles.Where(t => t.Intersects(node.AABB)).ToList();
            if (result.Any())
            {
                if (node.Level == maxLevel)
                {
                    node.Fill();
                }
                else
                {

                    if (node.Children == null)
                    {
                        node.Split();
                    }
                    Parallel.ForEach(node.Children, child => Test(child, result));
                }

            }
            
        }


        void Intersect(OctreeNode<Voxel> node, Triangle tri)
        {
            if(node.Level > maxLevel) return;
            if (tri.Intersects(node.AABB))
            {
                if (node.Level == maxLevel)
                {
                    node.Fill();
                }
                else
                {
                    if (node.Children == null)
                    {
                        node.Split();
                    }
                    foreach (var child in node.Children)
                    {
                        Intersect(child,tri);
                    }
                }
            }


        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PointSize(5f);

            _ubo = new CameraUBO();
            _shader = new BasicShaderProgram(_ubo);

            _vao = new VAO(_shader, new VBO(new List<Vertex>(FetchData(_tree))) { BeginMode = BeginMode.Quads });

            var green = new Color4(0.156f, 0.627f, 0.353f, 1.0f).ToVector4();
            var data = new List<Vertex>();

            foreach (var element in stl.Elements)
            {
                data.Add(new Vertex { Colour = green, Position = element.P1.ToVector3() });
                data.Add(new Vertex { Colour = green, Position = element.P2.ToVector3() });
                data.Add(new Vertex { Colour = green, Position = element.P3.ToVector3() });
            }

            _vao2 = new VAO(_shader, new VBO(data) { BeginMode = BeginMode.Triangles });



            var filled = _tree.Flatten().Where(o => o.State == NodeState.Filled).ToArray();


            _vaoFilled = new VAO(_shader, new VBO(new List<Vertex>(FetchDataSolid(filled))) { BeginMode = BeginMode.Quads });

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine("Error at OnLoad: " + err);
        }

        private IEnumerable<Vertex> FetchData(OctreeNode<Voxel> node)
        {
            var green = new Color4(0.156f, 0.627f, 0.353f, 1.0f).ToVector4();
            var s = node.Size/2.0;

            //+x
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, -s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, -s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, s, -s)).ToVector3() };
            //-x
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, -s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, -s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, s, -s)).ToVector3() };
            //+y
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, s, -s)).ToVector3() };
            //-y
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, -s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, -s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, -s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, -s, -s)).ToVector3() };
            //+z
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, -s, s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, -s, s)).ToVector3() };
            //-z
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(-s, -s, -s)).ToVector3() };
            yield return new Vertex { Colour = green, Position = (node.Center + new Vect3(s, -s, -s)).ToVector3() };


            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    foreach (var vertex in FetchData(child))
                    {
                        yield return vertex;
                        
                    }
                }
            }
            
        }

        private IEnumerable<Vertex> FetchDataSolid(IEnumerable<OctreeNode<Voxel>> nodes)
        {

            foreach (var node in nodes)
            {
                var s = node.Size/2.0;// -0.01;
                //+x
                yield return new Vertex { Colour = new Color4(Color.PaleVioletRed).ToVector4(), Position = (node.Center + new Vect3(s, s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleVioletRed).ToVector4(), Position = (node.Center + new Vect3(s, -s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleVioletRed).ToVector4(), Position = (node.Center + new Vect3(s, -s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleVioletRed).ToVector4(), Position = (node.Center + new Vect3(s, s, -s)).ToVector3() };
                //-x
                yield return new Vertex { Colour = new Color4(Color.PaleGreen).ToVector4(), Position = (node.Center + new Vect3(-s, s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleGreen).ToVector4(), Position = (node.Center + new Vect3(-s, -s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleGreen).ToVector4(), Position = (node.Center + new Vect3(-s, -s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleGreen).ToVector4(), Position = (node.Center + new Vect3(-s, s, s)).ToVector3() };
                //+y
                yield return new Vertex { Colour = new Color4(Color.PaleTurquoise).ToVector4(), Position = (node.Center + new Vect3(s, s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleTurquoise).ToVector4(), Position = (node.Center + new Vect3(-s, s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleTurquoise).ToVector4(), Position = (node.Center + new Vect3(-s, s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleTurquoise).ToVector4(), Position = (node.Center + new Vect3(s, s, s)).ToVector3() };
                //-y                              
                yield return new Vertex { Colour = new Color4(Color.PaleGoldenrod).ToVector4(), Position = (node.Center + new Vect3(s, -s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleGoldenrod).ToVector4(), Position = (node.Center + new Vect3(-s, -s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleGoldenrod).ToVector4(), Position = (node.Center + new Vect3(-s, -s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.PaleGoldenrod).ToVector4(), Position = (node.Center + new Vect3(s, -s, -s)).ToVector3() };
                //+z                               
                yield return new Vertex { Colour = new Color4(Color.NavajoWhite).ToVector4(), Position = (node.Center + new Vect3(s, s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.NavajoWhite).ToVector4(), Position = (node.Center + new Vect3(-s, s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.NavajoWhite).ToVector4(), Position = (node.Center + new Vect3(-s, -s, s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.NavajoWhite).ToVector4(), Position = (node.Center + new Vect3(s, -s, s)).ToVector3() };
                //-z                               
                yield return new Vertex { Colour = new Color4(Color.Silver).ToVector4(), Position = (node.Center + new Vect3(s, -s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.Silver).ToVector4(), Position = (node.Center + new Vect3(-s, -s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.Silver).ToVector4(), Position = (node.Center + new Vect3(-s, s, -s)).ToVector3() };
                yield return new Vertex { Colour = new Color4(Color.Silver).ToVector4(), Position = (node.Center + new Vect3(s, s, -s)).ToVector3() };



            }


        }

        public Bitmap GrabScreenshot()
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _camera.View *= Mat4.RotateZ(Angle.FromDegrees(0.6));
            _camera.View *= Mat4.RotateY(Angle.FromDegrees(0.4));
            _camera.View *= Mat4.RotateX(Angle.FromDegrees(0.2));
            _ubo.Update(_camera);

        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(new Color4(0.137f, 0.121f, 0.125f, 0f));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //_vao.Render();

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //_vao2.Render();
            _vaoFilled.Render();
            SwapBuffers();
            //GrabScreenshot().Save("test.png");
            ErrorCode err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine("Error at Swapbuffers: " + err.ToString());
            Title = String.Format(" FPS:{0} Mouse<{1},{2}>", 1.0 / e.Time, Mouse.X, Height - Mouse.Y);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.Resize(Width, Height);
        }
    }


    public class Camera:OrthographicCamera
    {
        public Camera()
        {
            Scale = 40;
        }
    }
}