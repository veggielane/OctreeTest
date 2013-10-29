using System;
using System.Collections.Generic;
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
        private CameraUBO _ubo;
        private readonly ICamera _camera;

        public Window(Octree<Voxel> tree)
            : base(1280, 720, new GraphicsMode(32, 0, 0, 4), "OpenCAD")
        {
            _tree = tree;

            VSync = VSyncMode.On;

            _camera = new OrthographicCamera();

            Mouse.WheelChanged += (sender, args) =>
                {
                    _camera.View = _camera.View * Mat4.Translate(0, 0, args.DeltaPrecise * -10.0);
                    //_camera.Eye += new Vect3(0, 0, args.DeltaPrecise * -10.0);
                    // Console.WriteLine(_camera.Eye);
                };
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
            GL.LineWidth(3);
            _vao = new VAO(_shader, new VBO(new List<Vertex>(FetchData(_tree))) { BeginMode = BeginMode.Quads });

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
            _vao.Render();

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            //_vao2.Render();

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