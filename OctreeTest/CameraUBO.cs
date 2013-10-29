using OpenTK;
using Veg.OpenTK;
using Veg.OpenTK.Buffers;

namespace OctreeTest
{
    public class CameraUBO : BaseUBO<CameraUBO.CameraData>
    {

        public struct CameraData
        {
            public Matrix4 MVP;
            public Matrix4 Model;
            public Matrix4 View;
            public Matrix4 Projection;
            public Matrix4 NormalMatrix;
        }

        public CameraUBO()
            : base("Camera", 0)
        {

        }

        public void Update(ICamera camera)
        {
            var normal = (camera.Model * camera.View).ToMatrix4();
            normal.Invert();
            normal.Transpose();
            Data = new CameraData
                {
                    MVP = camera.MVP.ToMatrix4(),
                    Model = camera.Model.ToMatrix4(),
                    View = camera.View.ToMatrix4(),
                    Projection = camera.Projection.ToMatrix4(),
                    NormalMatrix = normal
                };
            Update();
        }
    }
}