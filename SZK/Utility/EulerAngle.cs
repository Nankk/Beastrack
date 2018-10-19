using System;
using System.Numerics;

namespace Beastrack.Utility
{
    public class EulerAngle
    {
        public EulerAngle()
        {

        }
        public EulerAngle(float headingDeg, float pitchDeg, float bankDeg)
        {
            Heading = headingDeg * (float)Math.PI / 180; // Around y-axis
            Pitch = pitchDeg * (float)Math.PI / 180; // Around x-axis
            Bank = bankDeg * (float)Math.PI / 180; // Around z-axis
        }

        public Quaternion ToQuaternion()
        {
            // From the article below:
            // http://akai0ringo.blog.shinobi.jp/%E9%96%8B%E7%99%BA%E4%B8%80%E8%88%AC/%E3%82%AA%E3%82%A4%E3%83%A9%E3%83%BC%E8%A7%92%E3%81%8B%E3%82%89%E3%82%AF%E3%82%A9%E3%83%BC%E3%82%BF%E3%83%8B%E3%82%AA%E3%83%B3%E3%81%AB%E5%A4%89%E6%8F%9B

            float cosY = (float)Math.Cos(Heading / 2f);
            float sinY = (float)Math.Sin(Heading / 2f);
            float cosP = (float)Math.Cos(Pitch / 2f);
            float sinP = (float)Math.Sin(Pitch / 2f);
            float cosR = (float)Math.Cos(Bank / 2f);
            float sinR = (float)Math.Sin(Bank / 2f);
            float x = cosR * sinP * cosY + sinR * cosP * sinY;
            float y = cosR * cosP * sinY - sinR * sinP * cosY;
            float z = sinR * cosP * cosY - cosR * sinP * sinY;
            float w = cosR * cosP * cosY + sinR * sinP * sinY;

            return new Quaternion(x, y, z, w);
        }
        public static EulerAngle FromQuaternion(Quaternion q)
        {
            // From the article below:
            // https://qiita.com/edo_m18/items/5db35b60112e281f840e

            float x = q.X;
            float y = q.Y;
            float z = q.Z;
            float w = q.W;

            float x2 = x * x;
            float y2 = y * y;
            float z2 = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;
            float wx = w * x;
            float wy = w * y;
            float wz = w * z;

            float m00 = 1f - (2f * y2) - (2f * z2);
            float m01 = (2f * xy) + (2f * wz);
            float m10 = (2f * xy) - (2f * wz);
            float m11 = 1f - (2f * x2) - (2f * z2);
            float m20 = (2f * xz) + (2f * wy);
            float m21 = (2f * yz) - (2f * wx);
            float m22 = 1f - (2f * x2) - (2f * y2);

            float heading;
            float pitch;
            float bank;

            if (ApproximatelyEquals(m21, 1f))
            {
                pitch = (float)Math.PI / 2f;
                heading = 0;
                bank = (float)Math.Atan2(m10, m00);
            }
            else if (ApproximatelyEquals(m21, -1f))
            {
                pitch = -(float)Math.PI / 2f;
                heading = 0;
                bank = (float)Math.Atan2(m10, m00);
            }
            else
            {
                pitch = (float)Math.Asin(-m21);
                heading = (float)Math.Atan2(m20, m22);
                bank = (float)Math.Atan2(m01, m11);
            }

            pitch *= 180f / (float)Math.PI;
            heading *= 180f / (float)Math.PI;
            bank *= 180f / (float)Math.PI;

            return new EulerAngle(heading, pitch, bank);
        }

        public float Heading { get; set; }
        public float Pitch { get; set; }
        public float Bank { get; set; }
        public float HeadingDeg
        {
            get
            {
                return Heading * 180f / (float)Math.PI;
            }
        }
        public float PitchDeg
        {
            get
            {
                return Pitch * 180f / (float)Math.PI;
            }
        }
        public float BankDeg
        {
            get
            {
                return Bank * 180f / (float)Math.PI;
            }
        }

        static bool ApproximatelyEquals(float a, float b, float eps = 1e-5f)
        {
            float diff = a - b;
            return -eps <= diff && diff <= eps;
        }
    }
}
