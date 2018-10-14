using KMR;
using KMR.IO;
using MUR.IO;
using MUR.VmdNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZK;
using SZK.Model;

namespace Beastrack
{
    class Program
    {
        static void Main(string[] args)
        {
            (new App()).Execute();
        }
    }

    class App
    {
        public App()
        {

        }

        public void Execute()
        {
            // KMR モジュールで全ファイル分のList<Keypoint>（平滑化済み）を取ってくる（今はまだ1フレーム分）
            var opr = new OpenPoseReader(@"C:\tmp\");
            _keypointsCollection.Add(1, opr.ReadOneFile("Sample_000000000132_keypoints.json"));

            // foreachで1つずつSZKモジュールで処理
            var dolls = new Dictionary<int, Doll>();
            foreach (var keypoints in _keypointsCollection)
            {
                var estimator = new PoseEstimator();
                dolls.Add(keypoints.Key, estimator.EstimateSingleFrame(keypoints.Value));
            }

            var vmd = new Vmd("Sample", dolls);
            var vw = new VmdWriter(@"C:\tmp\test.vmd");
            vw.Write(vmd);

            // ※必要な入力引数
            // OpenPoseディレクトリ（平面座標取得から一貫して行うなら）
            // OpenPose出力先ディレクトリ
            // Vmd出力ファイルパス
        }

        IDictionary<int, List<Keypoint>> _keypointsCollection;
    }
}
