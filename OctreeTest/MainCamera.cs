using System;
using Veg.Maths;

namespace OctreeTest
{
    public abstract class BaseCamera : ICamera
    {


        public Mat4 Model { get; set; }
        public Mat4 View { get; set; }
        public Mat4 Projection { get; set; }

        public Mat4 MVP
        {
            get { return Projection * View * Model; }
        }

        public Vect3 Eye { get; set; }
        public Vect3 Target { get; set; }
        public Vect3 Up { get; set; }

        public double Near { get; protected set; }
        public double Far { get; protected set; }

        public abstract void Update(double delta);
        public abstract void Resize(int width, int height);

    }
    public class OrthographicCamera : BaseCamera
    {
        public double Scale { get; set; }

        public OrthographicCamera()
        {
            Near = -1000f;
            Far = 1000f;
            Target = Vect3.Zero;
            Up = Vect3.UnitY;
            Eye = new Vect3(0, 0, 20);
            Model = Mat4.Identity;
            View = Mat4.LookAt(Eye, Target, Up);
            Projection = Mat4.Identity;
            Scale = 25;
        }
        public override void Update(double delta)
        {

        }

        public override void Resize(int width, int height)
        {
            Projection = CreateOrthographic(-width / 2.0, width / 2.0, -height / 2.0, height / 2.0, Near, Far) * Scalem(Scale);
        }
        public static Mat4 CreateOrthographic(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
        {
            return new Mat4(new[,]
            {
                {2.0f / (xMax - xMin), 0, 0, -((xMax + xMin)/(xMax - xMin))},
                {0, 2.0f / (yMax - yMin), 0, -((yMax + yMin)/(yMax - yMin))},
                {0, 0, -2.0f / (zMax - zMin), -((zMax + zMin)/(zMax - zMin))},
                {0, 0, 0, 1}
            });
        }
        public static Mat4 Scalem(double s)
        {
            return new Mat4(new[,]
            {
                {s, 0, 0, 0},
                {0, s, 0, 0},
                {0, 0, s, 0},
                {0, 0, 0, 1}
            });
        }
    }
    public class MainCamera : ICamera
    {
        private int _width;
        private int _height;

        private Mat4 _view;
        public MainCamera()
        {

            Near = 1f;
            Far = 1000f;
            Model = Mat4.Translate(0, 0, 0);
            Target = Vect3.Zero;
            Up = Vect3.UnitY;

            Eye = new Vect3(0, 0, 500);


            _view = Mat4.LookAt(Eye, Target, Up);

            // _view = Mat4.RotateX(Angle.FromRadians(Math.Atan(Math.Sin(Angle.FromDegrees(-45))))) * (Mat4.LookAt(Eye, Target, Up) * Mat4.RotateZ(Angle.FromDegrees(45)));
            //_view = Mat4.Identity;
        }

        public double Near { get; private set; }
        public double Far { get; private set; }
        public Mat4 Model { get; set; }


        public Mat4 View
        {
            get { return _view; }
            set { _view = value; }
        }

        public Mat4 Projection { get; set; }

        public Mat4 MVP
        {
            get { return Projection * View * Model; }
        }

        public Vect3 Eye { get; set; }

        public Vect3 Target { get; set; }

        public Vect3 Up { get; set; }

        public void Update(double delta)
        {

        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            Projection = Mat4.CreatePerspectiveFieldOfView(Math.PI / 2, _width / (float)_height, Near, Far);

        }
    }
}