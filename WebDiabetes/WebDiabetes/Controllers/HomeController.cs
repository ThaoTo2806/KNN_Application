using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDiabetes.Models;

namespace WebDiabetes.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        static int K = 9;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dataset()
        {
            List<ClassDiabetes> _dataDiabetes = ConnectionDiabetesDatabase.Instance.GetDiabetes();
            string str = "";
            foreach (var k in _dataDiabetes)
            {
                str += @"<tr>";
                foreach (var item in k.Attributes)
                {
                    str += @"<td>" + item.ToString() + @"</td>";
                }
                str += @"<td>" + k.Val.ToString() + @"</td></tr>";
            }
            ViewBag.Diabetes = str;
            ViewBag.CountPos = "[" + _dataDiabetes.Count(x => x.Val == 0).ToString() + "]";
            ViewBag.CountNe = "[" + _dataDiabetes.Count(x => x.Val == 1).ToString() + "]";
            double Precision, Recall;
            KNeighbors _knn = new KNeighbors(K);
            _knn.Fit(_dataDiabetes);
            _knn.Accuracy(out Precision, out Recall);
            ViewBag.Precision = "[" + (Precision * 100).ToString().Replace(",", ".") + "," + (100 - Precision * 100).ToString().Replace(",", ".") + "]";
            //ViewBag.Recall = "[" + (Recall * 100).ToString().Replace(",", ".") + "," + (100 - Recall * 100).ToString().Replace(",", ".") + "]";

            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Result(FormCollection fc)
        {
            List<ClassDiabetes> _dataDiabetes = ConnectionDiabetesDatabase.Instance.GetDiabetes();
            double Pregnancies, Glucose, BloodPressure, Age, SkinThickness, DiabetesPedigreeFunction, Insulin, BMI;
            if (!Utilities.Instance.ConvertDouble(fc["Pregnancies"].ToString(), out Pregnancies) || !Utilities.Instance.ConvertDouble(fc["Glucose"].ToString(), out Glucose)
                || !Utilities.Instance.ConvertDouble(fc["BloodPressure"].ToString(), out BloodPressure) || !Utilities.Instance.ConvertDouble(fc["Age"].ToString(), out Age)
                || !Utilities.Instance.ConvertDouble(fc["SkinThickness"].ToString(), out SkinThickness) || !Utilities.Instance.ConvertDouble(fc["DiabetesPedigreeFunction"].ToString(), out DiabetesPedigreeFunction)
                || !Utilities.Instance.ConvertDouble(fc["Insulin"].ToString(), out Insulin) || !Utilities.Instance.ConvertDouble(fc["BMI"].ToString(), out BMI))
            {
                Session["Err"] = "true";
                return RedirectToAction("Test");
            }


            KNeighbors _knn = new KNeighbors(K);
            _knn.Fit(_dataDiabetes);
            ClassDiabetes dataTest = new ClassDiabetes();
            dataTest.Attributes.Add(Pregnancies);
            dataTest.Attributes.Add(Glucose);
            dataTest.Attributes.Add(BloodPressure);
            dataTest.Attributes.Add(SkinThickness);
            dataTest.Attributes.Add(Insulin);
            dataTest.Attributes.Add(BMI);
            dataTest.Attributes.Add(DiabetesPedigreeFunction);
            dataTest.Attributes.Add(Age);
            double Predict_Proba;
            double val = _knn.Predict(dataTest, out Predict_Proba);

            dataTest.Val = (int)val;

            string avgSick, maxSick;
            _knn.GraphString(out avgSick, out maxSick);
            string testString = "[";
            for (int i = 0; i < dataTest.Attributes.Count; i++)
            {
                if (i > 0)
                    testString += ',';
                testString += dataTest.Attributes[i].ToString().Replace(',', '.');
            }
            testString += "]";

            ViewBag.dataTest = dataTest;

            ViewBag.Test = testString;

            ViewBag.avgSick = avgSick;
            ViewBag.maxSick = maxSick;
            ViewBag.Predict_Proba = Predict_Proba;

            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
    }
}