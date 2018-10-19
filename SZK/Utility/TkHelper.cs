using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZK.Utility
{
    public class TkHelper
    {
        public static Vector2 Project(Vector3d vector)
        {
            // Add w element for conversion
            Vector4d v4 = new Vector4d(vector);
            v4.W = 1.0;

            // Get conversion matrices
            Matrix4d modelView;
            GL.GetDouble(GetPName.ModelviewMatrix, out modelView);
            Matrix4d projection;
            GL.GetDouble(GetPName.ProjectionMatrix, out projection);
            int[] vp = new int[4];
            GL.GetInteger(GetPName.Viewport, vp);
            Matrix4 viewport = Matrix4.Identity;

            // NOTE: Matrix4 type assumes the result obtained by "vector * matrix", instead of "matrix * vector".
            viewport.Row0.X = vp[2] / 2;
            viewport.Row1.Y = -vp[3] / 2;
            viewport.Row3.X = vp[2] / 2;
            viewport.Row3.Y = vp[3] / 2;

            // Convert to window coordinate system
            v4 = Vector4d.Transform(v4, modelView);
            v4 = Vector4d.Transform(v4, projection);
            v4 = Vector4d.Multiply(v4, 1.0 / v4.W);  // Manual normalization required
            v4 = (Vector4d)Vector4.Transform((Vector4)v4, viewport);

            // Return only x and y
            return new Vector2((int)v4.X, (int)v4.Y);
        }

    }
}
