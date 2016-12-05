using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using TrafficSign.Imaging;

namespace TrafficSignTrain
{
    class Program
    {
        static void Main(string[] args)
        {
            Imaging.ImgDataLoader loader = new Imaging.ImgDataLoader(5, new System.Drawing.Size(40, 40));
            loader.LoadAll(@"D:\stud_repo\astu\train\red circle\");
            ANNTrainer trainer = new ANNTrainer();
            trainer.Train(loader.DataSet);

            //List<TrainSample> all = new List<TrainSample>();
            //for (int i = 0; i < 5; i++)
            //{
            //    var sm0 = TrainHog.ComputeForClass(string.Format("{0}{1}", @"D:\stud_repo\astu\train\red circle\",i), i);
            //    all.AddRange(sm0);
            //}
            ////var neg = TrainHog.ComputeForClass( @"D:\stud_repo\astu\train\neg\", 5);
            ////all.AddRange(neg);

            //ANNTrainer trainer = new ANNTrainer();
            //trainer.Train(all);


            
        }
    }
}
