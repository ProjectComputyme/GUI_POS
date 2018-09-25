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
using Computyme.Manager;

// Lets test the change

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

                U2Connection con = new U2Connection();
                U2Command cmd = con.CreateCommand();
                 con.ConnectionString = s;
                con.Open();

                cmd.CommandText = "SELECT PROD,SALE, DESC1 FROM IVMST WHERE UPPER(DESC1) LIKE '%" + Upperterm + "%'";
       
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
      
        public ActionResult UpdateProduct(String Product, string TransactionID,string Quantity, string Cost)
        {
            List<Orders> ShoppingCart = HomeManager.ReadExistingOrders(TransactionID);
            

            // Need to look at quantity too.
            var itemToRemove = (from item in ShoppingCart
                                where item.Serial == Product
                                select item).FirstOrDefault();
            ShoppingCart.Remove(itemToRemove);


            


            if (HomeManager.DeleteProducts(ShoppingCart, TransactionID, Product))
            {

                Orders ModifyCart = new Orders { Cost = Cost, Quantity = Quantity, Serial = Product };
                ShoppingCart.Add(ModifyCart);

                if (HomeManager.InsertOrderItem(ShoppingCart, TransactionID))
                {

                }


            }

            return RedirectToAction("About");
        }


        public ActionResult DeleteProduct(String Product, string TransactionID)
        {
            List<Orders> ShoppingCart = HomeManager.ReadExistingOrders(TransactionID);


            // Need to look at quantity too.
            var itemToRemove = (from item in ShoppingCart
                                where item.Serial == Product
                                select item).FirstOrDefault();
            ShoppingCart.Remove(itemToRemove);

            if (HomeManager.DeleteProducts(ShoppingCart, TransactionID, Product))
            {
                if (HomeManager.InsertOrderItem(ShoppingCart, TransactionID))
                {

                }


            }

            return RedirectToAction("About");
        }

            //AddProduct
            public ActionResult AddProduct(Orders OrderlineItem, string action, string OrderID)
        {

            OrderID = "5001";
            // Need to do a read on all items. 
            List<Orders> ShoppingCart = HomeManager.ReadExistingOrders(OrderID);            
            Orders LineItem = new Orders { Cost = OrderlineItem.Cost, Quantity = OrderlineItem.Quantity, Serial = OrderlineItem.Serial };
            ShoppingCart.Add(LineItem);

            if (HomeManager.InsertOrderItem(ShoppingCart, OrderID))
            {
            }
            else { }
            return RedirectToAction("About");
        }




        public ActionResult PullProcessSar( string SAR_ID, string Type, string disposistion)
        {

            var jsonDataa = "L";
            return Json(jsonDataa, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Getorders(string sidx, string sord, int page, int rows, string Order_ID)
        {

          
            List<Orders> ShoppingCart = Manager.HomeManager.ReadExistingOrders(Order_ID);


            List<Orders> test =  Manager.HomeManager.GetProducts(ShoppingCart);

            int pageIndex = 0;
            int pageSize = 0;
            int totalRecords = 0;
            int totalPages = 0;
        

            pageIndex = Convert.ToInt32(page) - 1;
            pageSize = rows;
            totalRecords = ShoppingCart.Count();
            totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);


            var jsonData = new
            {
                total = (int)Math.Ceiling((float)totalRecords / (float)pageSize), //number of pages
                page = 1, //current page
                pageIndex = Convert.ToInt32(page) - 1,
                pageSize = rows,
                totalRecords = ShoppingCart.Count(),
                records = ShoppingCart.Count(),
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize),//total items
                rows = ShoppingCart
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);



        }




        public ActionResult About()
        {

            Computyme.Models.NewOrder.UniqueOrder OrderID = new Models.NewOrder.UniqueOrder { OrderID = "5001", UserID = "4561" };

            ViewData["TransacionID"] = OrderID;
            return View();
        }

        public ActionResult Contact()
        {
            //try
            //{
            //    U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            //    conn_str.UserID = "user";
            //    conn_str.Password = "pass";
            //    conn_str.Server = "localhost";
            //    conn_str.Database = "HS.SALES";
            //    conn_str.ServerType = "UNIVERSE";
            //    conn_str.AccessMode = "Native";   // FOR UO
            //    conn_str.RpcServiceType = "uvcs"; // FOR UO
            //    conn_str.Pooling = false;
            //    string s = conn_str.ToString();
            //    U2Connection con = new U2Connection();
            //    con.ConnectionString = s;
            //    con.Open();
            //    Console.WriteLine("Connected.........................");



            //    UniSession us1 = con.UniSession;

            //    string RoutineName = "!TIMDAT";
            //    int IntTotalAtgs = 1;
            //    string[] largs = new string[IntTotalAtgs];
            //    largs[0] = "1";
            //    UniDynArray tmpStr2;
            //    UniSubroutine sub = us1.CreateUniSubroutine(RoutineName, IntTotalAtgs);

            //    for (int i = 0; i < IntTotalAtgs; i++)
            //    {
            //        sub.SetArg(i, largs[i]);
            //    }

            //    sub.Call();
            //    tmpStr2 = sub.GetArgDynArray(0);
            //    string result = "\n" + "Result is :" + tmpStr2;
            //    Console.WriteLine("  Response from UniSubRoutineSample :" + result);


            //    con.Close();

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);

            //}
            //finally
            //{
            //    Console.WriteLine("Enter to exit:");
            //    string line = Console.ReadLine();
            //}
        
            return View();
        }
    }
}