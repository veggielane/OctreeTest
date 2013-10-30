using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Veg.Maths;
using Veg.OpenTK;
using Veg.OpenTK.Buffers;
using Veg.OpenTK.Shaders;
using Veg.OpenTK.Vertices;

namespace OctreeTest
{
    public class Window : GameWindow
    {
        private readonly Octree<Voxel> _tree;
        private IShaderProgram _shader;
        private VAO _vao;
        private VAO _vao2;
        private VAO _vaoFilled;
        private CameraUBO _ubo;
        private readonly ICamera _camera;
        STL stl;
        private int maxLevel = 15;

        public Window()
            : base(1280, 720, new GraphicsMode(32, 0, 0, 4), "OpenCAD")
        {
            
            stl = new STL("Models/elephant.stl", Color.Green, STLType.Binary);


            //var max = stl.Elements.Select(e=>e.P1,X)


            ;
            //var x =
            //    stl.Elements.Select(e => e.P1.X)
            //       .Concat(stl.Elements.Select(e => e.P2.X))
            //       .Concat(stl.Elements.Select(e => e.P3.X)).ToList();
            //var xmax = x.Max();
            //var xmin = x.Min();
            //var x = stl.Elements.Max(e => e.P1.X);



            _tree = new Octree<Voxel>(Vect3.Zero, 16.0);
            //_tree.Split();
            

            foreach (var tri in stl.Elements)
            {
                Intersect(_tree, tri);
            }


            VSync = VSyncMode.On;

            _camera = new OrthographicCamera();




            Mouse.WheelChanged += (sender, args) =>
                {
                    _camera.View = _camera.View * Mat4.Translate(0, 0, args.DeltaPrecise * -10.0);
                    //_camera.Eye += new Vect3(0, 0, args.DeltaPrecise * -10.0);
                    // Console.WriteLine(_camera.Eye);
                };
        }


        void Intersect(OctreeNode<Voxel> node, Triangle tri)
        {
            if(node.Level > maxLevel) return;
            if (node.AABB.Intersects(tri))
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
            //GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PointSize(5f);

            _ubo = new CameraUBO();
            _shader = new BasicShaderProgram(_ubo);
   
            GL.PointSize(10);
           // GL.LineWidth(3);

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
            var green = new Color4(Color.PaleVioletRed).ToVector4();
            foreach (var node in nodes)
            {
                var s = node.Size / 2.0 - 0.01;
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
            }


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
}