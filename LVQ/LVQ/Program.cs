using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace LVQ
{
    
    class Program
    {
        //Datasetimizi 0-1 aralığına normalize ederiz
        static void normalizeEt(double[] dizi)
        {
            double max, min;
            max = dizi.Max(); min = dizi.Min();
            double count = dizi.Length;
            for (int i = 0; i < count; i++)
            {
                dizi[i] = (dizi[i] - min) / ((max - min));
            }
        }
        static void Main(string[] args)
        {
            int n = 20000, m = 10000;
            double[][] normalized = new double[24][];
            double[][] normalizedtest = new double[24][];
            double dis1, dis2, alfa = 0.1, islemsayisi1 = 0, islemsayisi2 = 0;
            string[][] train = new string[n][];
            string[][] test = new string[m][];
            string[][] initial = new string[24][];
            string[][] initialt = new string[24][];
            double[][] initialsayi = new double[24][];
            double[][] initialsayit = new double[24][];
            string[][] ilk = new string[24][];
            double[][] ilksayi = new double[24][];
            double accuracy,precision,recall,fscore,fp=0,tp=0,fn=0,tn=0,macrorecall,microrecall,macrofscore,microfscore;
            double accuracy2, precision2, recall2, fscore2, fp2 = 0, tp2 = 0, fn2= 0, tn2 = 0, macroprecision,microprecision;
            double recall22, precision22, fscore22;
            int count = 0,count2=0;

            string[] tahmin = new string[n];
            for (int i = 0; i < n; i++)
            {
                train[i] = new string[24];
            }
            
            for (int i = 0; i < m; i++)
            {
                test[i] = new string[24];
            }

            for (int i = 0; i < 24; i++)
            {
                initialsayit[i] = new double[m];
                ilksayi[i] = new double[2];
                normalized[i] = new double[n];
                normalizedtest[i] = new double[m];
            }

            //train verimizi çekeriz ve 2 boyutlu diziye aktarırız
            string filePath = "train.csv";
            StreamReader sr = new StreamReader(filePath);
            var lines = new List<string[]>();
            int Row = 0;
            while (!sr.EndOfStream)
            {
                string[] Line = sr.ReadLine().Split(';');
                lines.Add(Line);
                Row++;
            }
            var data = lines.ToArray();


            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    train[i][j] = Convert.ToString(data[i][j]);
                }
            }

            //test verimizi çekeriz ve 2 boyutlu diziye aktarırız
            string filePath2 = "test.csv";
            StreamReader sr2 = new StreamReader(filePath2);
            var lines2 = new List<string[]>();
            int Row2 = 0;
            while (!sr.EndOfStream)
            {
                string[] Line = sr2.ReadLine().Split(';');
                lines2.Add(Line);
                Row2++;
            }
            var data2 = lines.ToArray();

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    test[i][j] = Convert.ToString(data2[i][j]);
                }
            }


            /*for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    Console.Write(train[i][j]+" ");
                }
                Console.WriteLine();
            }*/

            //2 boyutlu dizilerimizi LVQ işlemleri için ters hale getiririz
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    normalized[j][i] = Convert.ToDouble(train[i][j]);
                }
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    normalizedtest[j][i] = Convert.ToDouble(test[i][j]);
                }
            }

            //2 boyutlu dizilerimizi normalize ederiz
            for (int i = 0; i < 24; i++)
            {
                normalizeEt(normalized[i]);
                normalizeEt(normalizedtest[i]);
            }

            for (int i = 1; i < 24; i++)
            {
                ilksayi[i][0] = normalized[i][0];
                ilksayi[i][1] = normalized[i][2];
            }

            /*
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    Console.Write(string.Format("{0:0.000}",normalized[j][i])+" ");
                }
                Console.WriteLine();
            }*/

            //eğitim kısmı
            //LVQ için ilk iki farklı sınıfımızın verilerini (weight dizilerini) sırayla karşılaştırarak eğitiriz
            for (int j = 0; j < n; j++)
            {
                dis1 = 0; dis2 = 0;
                for (int i = 0; i < 23; i++)
                {
                    if (ilksayi[i][0] != normalized[i][j])
                    {
                        islemsayisi1 = 1;
                    }
                    else islemsayisi1 = 0;

                    if (ilksayi[i][1] != normalized[i][j])
                    {
                        islemsayisi2 = 1;
                    }
                    else islemsayisi2 = 0;
                    dis1 += (Math.Pow((islemsayisi1), 2));
                    dis2 += (Math.Pow((islemsayisi2), 2));
                }
                if (dis1 < dis2)
                {
                    if (normalized[23][j] != ilksayi[23][0])
                    {
                        for (int k = 0; k < 23; k++)
                        {
                            ilksayi[k][0] = (ilksayi[k][0] - (alfa * (normalized[k][j] - ilksayi[k][0])));
                        }
                        //Console.WriteLine("1--------1");
                    }
                    else
                    {
                        for (int k = 0; k < 23; k++)
                        {
                            ilksayi[k][0] =( ilksayi[k][0] + (alfa * (normalized[k][j] - ilksayi[k][0])));
                        }
                        //Console.WriteLine("1=1");
                    }
                }
                else
                {
                    if (normalized[23][j] != ilksayi[23][1])
                    {
                        for (int k = 0; k < 23; k++)
                        {
                            ilksayi[k][1] = (ilksayi[k][1] - (alfa * (normalized[k][j] - ilksayi[k][1])));
                        }
                        //Console.WriteLine("2-------2");
                    }
                    else
                    {
                        for (int k = 0; k < 23; k++)
                        {
                            ilksayi[k][1] = (ilksayi[k][1] + (alfa * (normalized[k][j] - ilksayi[k][1])));
                        }
                        //Console.WriteLine("2=2");
                    }
                }
                //her seferinde learning rate düşürülür
                alfa = alfa * 0.5;
            }


            //test hesaplama
            for (int j = 0; j < m; j++)
            {
                dis1 = 0; dis2 = 0;
                for (int i = 0; i < 24; i++)
                {
                    if (i==23)
                    {
                        dis1 += ((Math.Pow((normalizedtest[i][j] - ilksayi[i][0]), 2)))/3.1;
                        dis2 += ((Math.Pow((normalizedtest[i][j] - ilksayi[i][1]), 2)))/3.1;
                    }
                    else
                    {
                        dis1 += (Math.Pow((normalizedtest[i][j] - ilksayi[i][0]), 2));
                        dis2 += (Math.Pow((normalizedtest[i][j] - ilksayi[i][1]), 2));
                    }
                }
                if (dis1 < dis2)
                {
                    tahmin[j] = "1";
                    if(Convert.ToDouble(tahmin[j]) == normalizedtest[23][j])
                    {
                        tp++;
                    }
                    else
                    {
                        fp++;
                    }
                }
                else
                {
                    tahmin[j] = "0";
                    if (Convert.ToDouble(tahmin[j]) == normalizedtest[23][j])
                    {
                        tn++;
                    }
                    else
                    {
                        fn++;
                    }
                }
                if (Convert.ToDouble(tahmin[j]) == normalizedtest[23][j])
                {
                    count++;
                }
            }
            Console.WriteLine("_______________________________________________________________");
            /*for (int i = 0; i < m; i++)
            {
                    Console.WriteLine(tahmin[i]);
            }

            
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.Write(ilksayi[i][j] + "         ");
                }
                Console.WriteLine();
            }*/

            //train hesaplama
            for (int j = 0; j < n; j++)
            {
                dis1 = 0; dis2 = 0;
                for (int i = 0; i < 24; i++)
                {
                    if (i == 23)
                    {
                        dis1 += ((Math.Pow((normalized[i][j] - ilksayi[i][0]), 2))) / 3.3;
                        dis2 += ((Math.Pow((normalized[i][j] - ilksayi[i][1]), 2))) / 3.3;
                    }
                    else
                    {
                        dis1 += (Math.Pow((normalized[i][j] - ilksayi[i][0]), 2));
                        dis2 += (Math.Pow((normalized[i][j] - ilksayi[i][1]), 2));
                    }
                }
                if (dis1 < dis2)
                {
                    tahmin[j] = "1";
                    if (Convert.ToDouble(tahmin[j]) == normalized[23][j])
                    {
                        tp2++;
                    }
                    else
                    {
                        fp2++;
                    }
                }
                else
                {
                    tahmin[j] = "0";
                    if (Convert.ToDouble(tahmin[j]) == normalized[23][j])
                    {
                        tn2++;
                    }
                    else
                    {
                        fn2++;
                    }
                }

                if (Convert.ToDouble(tahmin[j]) == normalized[23][j])
                {
                    count2++;
                }
            }



            accuracy = Convert.ToDouble(count) / Convert.ToDouble(m);
            precision = tp / (tp + fp);
            precision22 = tn / (tn + fn);
            recall = tp / (tp + fn);
            recall22 = tn / (tn + fp);
            fscore = 2 * ((precision*recall)/(precision+recall));
            fscore22 = 2 * ((precision22 * recall22) / (precision22 + recall22));
            accuracy2 = Convert.ToDouble(count2) / Convert.ToDouble(n);
            precision2 = tp2 / (tp2 + fp2);
            recall2 = tp2 / (tp2 + fn2);
            fscore2 = 2 * ((precision2 * recall2) / (precision2 + recall2));
            macrorecall = (recall + recall2) / 2;
            macroprecision =(precision+precision2)/2;
            macrofscore = 2 * ((macroprecision * macrorecall) / (macroprecision + macrorecall));
            microrecall =(tp+tp2)/(tp+tp2+fn+fn2);
            microprecision = (tp + tp2) / (tp + tp2 + fp + fp2);
            microfscore =2*((microprecision * microrecall) / (microprecision + microrecall));
            Console.WriteLine("Accuracy=" + accuracy); Console.WriteLine("Recall=" + string.Format("{0:0.000}", recall)); Console.WriteLine("Precision=" + string.Format("{0:0.000}", precision));
            Console.WriteLine("Fscore=" + string.Format("{0:0.000}", fscore)); Console.WriteLine("Macro Recall=" + string.Format("{0:0.000}", macrorecall)); Console.WriteLine("Micro Recall=" + string.Format("{0:0.000}", microrecall));
            Console.WriteLine("Macro F-Score=" + string.Format("{0:0.000}", macrofscore)); Console.WriteLine("Micro F-Score=" + string.Format("{0:0.000}", microfscore));
            Console.WriteLine("Recall2=" + string.Format("{0:0.000}", recall22)); Console.WriteLine("Precision2=" + string.Format("{0:0.000}", precision22));
            Console.WriteLine("Fscore2=" + string.Format("{0:0.000}", fscore22));
            Console.WriteLine("Done!");
            Console.ReadKey(); Console.ReadKey(); Console.ReadKey();
        }
        
        
    }
}
