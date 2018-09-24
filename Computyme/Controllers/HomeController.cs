using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using U2.Data.Client;
using U2.Data.Client.UO;
using System.Data;
using Computyme.Models;
using Computyme.Models.APMST;
using System.Configuration;

namespace Computyme.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            List<APMST> sample = new List<APMST>();

            try
            {

                string s = ConfigurationManager.AppSettings["TESTString"];
                U2Connection con = new U2Connection();
                con.ConnectionString = s;
                con.Open();

                U2Command cmd = con.CreateCommand();
                cmd.CommandText = "SELECT PROD,SALE, DESC1 FROM IVMST"; // FNAME = SingleValue, PRICE = multivalue

                

                DataSet ds = new DataSet();
                U2DataAdapter da = new U2DataAdapter(cmd);
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    //APMST loadRecord = new APMST {  NAME = dr["NAME"].ToString() };
                    APMST loadRecord = new APMST { NAME = dr["PROD"].ToString() };

                    sample.Add(loadRecord);
                }


                con.Close();

            }
            catch (Exception e)
            {
               // Console.WriteLine(e.Message);
                if (e.InnerException != null)
                {

                }

            }
            finally
            {
               // string line = Console.ReadLine();
            }


            return View(sample);
        }



        public ActionResult GetProduct(string term)
        {

            List<Product> ProductsFound = new List<Product>();

            string Upperterm = term.ToUpper();
            try
            {

                string s = ConfigurationManager.AppSettings["TESTString"];

                //UniSession us1 = con.UniSession;
                //UniSubroutine sub = us1.CreateUniSubroutine(RoutineName, IntTotalAtgs);

                U2Connection con = new U2Connection();
                U2Command cmd = con.CreateCommand();
                 con.ConnectionString = s;
                con.Open();

                cmd.CommandText = "SELECT PROD,SALE, DESC1 FROM IVMST WHERE UPPER(DESC1) LIKE '%" + Upperterm + "%'";
                //tcl 
                //cmd.CommandText = "LIST IVMST SALE DESC WITH DESC LIKE ..." + term + "...";
                U2DataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {

                    Product ProductList = new Product { DESC1 = dr["DESC1"].ToString(),
                                                        PROD = dr["PROD"].ToString() ,
                                                        SALE  = dr["SALE"].ToString()
                    };

                    ProductsFound.Add(ProductList);
                }


                con.Close();



            }
            catch (Exception e)
            {
                // Console.WriteLine(e.Message);
                if (e.InnerException != null)
                {

                }

            }
            finally
            {
                // string line = Console.ReadLine();
            }


            return Json(ProductsFound, JsonRequestBehavior.AllowGet);

        }


        //AddProduct
        public ActionResult AddProduct(Orders OrderlineItem)
        {

            var jsonDataa = "L";
            return Json(jsonDataa, JsonRequestBehavior.AllowGet);
        }




        public ActionResult PullProcessSar(string SAR_ID, string Type, string disposistion)
        {

            var jsonDataa = "L";
            return Json(jsonDataa, JsonRequestBehavior.AllowGet);
        }

            public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            try
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
                conn_str.UserID = "user";
                conn_str.Password = "pass";
                conn_str.Server = "localhost";
                conn_str.Database = "HS.SALES";
                conn_str.ServerType = "UNIVERSE";
                conn_str.AccessMode = "Native";   // FOR UO
                conn_str.RpcServiceType = "uvcs"; // FOR UO
                conn_str.Pooling = false;
                string s = conn_str.ToString();
                U2Connection con = new U2Connection();
                con.ConnectionString = s;
                con.Open();
                Console.WriteLine("Connected.........................");



                UniSession us1 = con.UniSession;

                string RoutineName = "!TIMDAT";
                int IntTotalAtgs = 1;
                string[] largs = new string[IntTotalAtgs];
                largs[0] = "1";
                UniDynArray tmpStr2;
                UniSubroutine sub = us1.CreateUniSubroutine(RoutineName, IntTotalAtgs);

                for (int i = 0; i < IntTotalAtgs; i++)
                {
                    sub.SetArg(i, largs[i]);
                }

                sub.Call();
                tmpStr2 = sub.GetArgDynArray(0);
                string result = "\n" + "Result is :" + tmpStr2;
                Console.WriteLine("  Response from UniSubRoutineSample :" + result);


                con.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            finally
            {
                Console.WriteLine("Enter to exit:");
                string line = Console.ReadLine();
            }
        
            return View();
        }
    }
}