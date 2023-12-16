using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDiabetes.Models
{
    public class KNeighbors
    {
        private List<ClassDiabetes> DataList { get; set; }
        private List<double> Label { get; set; }
        private ClassDiabetes Test { get; set; }
        private int n_neighbors { get; set; }
        private List<List<double>> MaxMin { get; set; }

        public KNeighbors(int k)
        {
            this.n_neighbors = k;
        }

        //Tìm giá trị nhỏ nhất trong cột tương ứng
        private double Min_In_Col(int col)
        {
            double min = DataList[0].Attributes[col];
            foreach (var item in DataList)
            {
                if (item.Attributes[col] < min)
                    min = item.Attributes[col];
            }
            return min;
        }

        // Tìm giá trị lớn nhất trong cột tương ứng
        private double Max_In_Col(int col)
        {
            double max = DataList[0].Attributes[col];
            foreach (var item in DataList)
            {
                if (item.Attributes[col] > max)
                    max = item.Attributes[col];

            }
            return max;
        }

        //Kiểm tra xem các giá trị trong cột 
        private bool TyLe(int col)
        {
            foreach (var item in DataList)
            {
                if (item.Attributes[col] >= 10)
                    return false;
            }
            return true;
        }

        // Chuẩn hóa dữ liệu
        public void Scale_Fit_Transform()
        {
            int row = DataList.Count;
            int col = DataList.First().Attributes.Count;
            MaxMin = new List<List<double>>();
            for (int j = 0; j < col; j++)
            {
                double min = 0, max = 0;
                if (!TyLe(j))
                {
                    min = Min_In_Col(j);
                    max = Max_In_Col(j);
                    foreach (var item in DataList)
                    {
                        item.Attributes[j] = ((double)(item.Attributes[j] - min) * 10) / (max - min);
                    }
                }
                List<double> item_MaxMin = new List<double>();
                item_MaxMin.Add(max);
                item_MaxMin.Add(min);
                MaxMin.Add(item_MaxMin);
            }
        }

        public void Fit(List<ClassDiabetes> dataList)
        {
            DataList = dataList;
            Label = new List<double>();
            foreach (var item in DataList)
            {
                if (!Label.Contains(item.Val))
                    Label.Add(item.Val);
            }
            Scale_Fit_Transform();
        }

        private double distance(ClassDiabetes a, ClassDiabetes b)
        {
            double d = 0;
            int col = a.Attributes.Count;
            for (int j = 0; j < col; j++)
                d += Math.Pow(a.Attributes[j] - b.Attributes[j], 2);
            return Math.Sqrt(d);
        }

        public ClassDiabetes Scale_Item(ClassDiabetes dataTest)
        {
            ClassDiabetes tamp = dataTest;
            int len = MaxMin.Count;
            for (int j = 0; j < len; j++)
            {
                if(MaxMin[j][0] != 0 || MaxMin[j][1] != 0)
                    tamp.Attributes[j] = ((double)(tamp.Attributes[j] - MaxMin[j][1]) * 10) / (MaxMin[j][0] - MaxMin[j][1]);
            }
            return tamp;
        }

        public double Predict(ClassDiabetes dataTest, out double Predict_Proba)
        {
            Test = Scale_Item(dataTest);
            //Test = dataTest;
            foreach (var item in DataList)
            {
                item.Distance = distance(item, Test);
            }
            DataList.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            
            int[] count = new int[Label.Count];
            for (int i = 0; i < this.n_neighbors; i++)
            {
                int index_Count = 0;
                foreach (var item in Label)
                {
                    if (DataList[i].Val == item)
                    {
                        count[index_Count]++;
                    }
                    index_Count++;
                }
            }
            int max = count[0];
            double label_max = Label[0];
            for (int i = 1; i < Label.Count; i++)
            {
                if (max < count[i])
                {
                    max = count[i];
                    label_max = Label[i];
                }
            }
            Predict_Proba = (label_max == 1) ? (double)max / this.n_neighbors : 1 - (double)max / this.n_neighbors;
            return label_max;
        }


        public ClassDiabetes Average(int lable)
        {
            int row = DataList.Count;
            int col = DataList.First().Attributes.Count;
            ClassDiabetes item = new ClassDiabetes();
            for (int j = 0; j < col; j++)
            {
                double avg = 0;
                for (int i = 0; i < row; i++)
                {
                    if(DataList[i].Val == lable)
                        avg += DataList[i].Attributes[j];
                }
                avg /= (double)row;
                item.Attributes.Add(avg);
            }
            return item;
        }

        public ClassDiabetes Min(int lable)
        {
            ClassDiabetes test = DataList.Find(x => x.Val == 1);
            if (test == null) return null;
            ClassDiabetes min = new ClassDiabetes();
            min.Attributes = test.Attributes;
            int n = min.Attributes.Count;
            for(int j = 0; j < n; j++)
            {
                foreach (var item in DataList)
                {
                    if (item.Attributes[j] < min.Attributes[j] && item.Val == lable)
                        min.Attributes[j] = item.Attributes[j];
                }
            }    
            return min;
        }

        public ClassDiabetes Max(int lable)
        {
            ClassDiabetes test = DataList.Find(x => x.Val == 0);
            if (test == null) return null;
            ClassDiabetes max = new ClassDiabetes();
            max.Attributes = test.Attributes;
            int n = max.Attributes.Count;
            for (int j = 0; j < n; j++)
            {
                foreach (var item in DataList)
                {
                    if (item.Attributes[j] > max.Attributes[j] && item.Val == lable)
                        max.Attributes[j] = item.Attributes[j];
                }
            }
            return max;
        }

        public void ConfusionMatrix(out int TruePositive, out int TrueNegative, out int FalsePositive, out int FalseNegative)
        {
            TruePositive = TrueNegative = FalseNegative = FalsePositive = 0;
            foreach (var item in DataList)
            {
                double pre;
                int label_item = (int)Predict(item, out pre);
                if (item.Val == 1 && label_item == 1)
                    TruePositive++;
                else if (item.Val == 0 && label_item == 0)
                    TrueNegative++;
                else if (item.Val == 1 && label_item == 0)
                    FalseNegative++;
                else if (item.Val == 0 && label_item == 1)
                    FalsePositive++;
                
            }
        }

        public void Accuracy(out double Precision, out double Recall)
        {
            int TruePositive, TrueNegative, FalseNegative, FalsePositive;
            ConfusionMatrix(out TruePositive, out TrueNegative, out FalsePositive, out FalseNegative);
            Precision = (double)TruePositive / (TruePositive + FalsePositive);
            Recall = (double)TruePositive / (TruePositive + FalseNegative);
        }

        public void GraphString(out string avgSick, out string maxSick)
        {
            ClassDiabetes _avgSick = Average(1);
            ClassDiabetes _maxSick = Max(1);
            avgSick = maxSick = @"[";
            int count_col = DataList.First().Attributes.Count;
            for (int i = 0; i < count_col; i++)
            {
                if (i > 0)
                {
                    avgSick += ',';
                    maxSick += ',';
                }
                avgSick += _avgSick.Attributes[i].ToString().Replace(',', '.');
                maxSick += _maxSick.Attributes[i].ToString().Replace(',', '.');
            }
            avgSick += "]";
            maxSick += "]";
        }


    }
}