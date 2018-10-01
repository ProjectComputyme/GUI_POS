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


        public ActionResult Finalize(string OrderID)
        {

            HttpCookie cookie = Request.Cookies["ComputymeOrder"];

            if (Request.Cookies["ComputymeOrder"]["OrderID"] == null)
            {
                //// no cookie found, create it
                //cookie = new HttpCookie("SurveyCookie");
                //cookie.Values["surveyPage"] = "1";
                //cookie.Values["surveyId"] = "1";
                //cookie.Values["surveyTitle"] = "Definietly not an NSA Survey....";
                //cookie.Values["lastVisit"] = DateTime.UtcNow.ToString();
            }
            else
            {
                // update the cookie values 
          
               // Response.Cookies["OrderStatus"].Value = "Complete";
                Response.Cookies["ComputymeOrder"]["OrderStatus"] = "Complete";
                // cookie.Values["OrderStatus"] = "Complete";
            }

            return RedirectToAction("About");


            //HttpCookie myCookie = new HttpCookie("ComputymeOrder");
            //myCookie["OrderID"] = NewOrderID;
            //myCookie["OrderStatus"] = "New";
            //myCookie.Expires = DateTime.Now.AddDays(1d);
            //Response.Cookies.Add(myCookie);

            //string OrderStatus;
            //if (Request.Cookies["ComputymeOrder"]["OrderID"] != null)
            //{
            //    OrderStatus = Request.Cookies["ComputymeOrder"]["OrderStatus"];

            //    if (OrderStatus == "New")
            //    {
            //        NewOrderID = Request.Cookies["ComputymeOrder"]["OrderID"];

            //    }



        }
            

        //file for customers -- IVCUST
        //NAME_1 = NAME of customer
        //ADDR_1 = Street address of customer - 
        //ADDR_2 = Second address of customer
        //CSZ = City, St Zip of customer
        //Phone_1 = Primary Phone number of customer
        public ActionResult GetCustomer(string term)
        {
           
            List<Computyme.Models.Customer.Customer> CustomerFound = new List<Computyme.Models.Customer.Customer>();

            string Upperterm = term.ToUpper();
            try
            {

                string s = ConfigurationManager.AppSettings["TESTString"];

                U2Connection con = new U2Connection();
                U2Command cmd = con.CreateCommand();
                con.ConnectionString = s;
                con.Open();

                cmd.CommandText = "SELECT NAME_1,ADDR_1, ADDR_2,CSZ, PHONE_1,EMAIL  FROM IVCUST WHERE UPPER(NAME_1) LIKE '%" + Upperterm + "%'";

                U2DataReader dr = cmd.ExecuteReader();

                string cell = "111-111-1111";
                string email = "None";
                while (dr.Read())
                {
                    try
                    {
                        if (dr["Phone_1"] != null)
                        {
                            cell = (dr["Phone_1"] ?? "").ToString();
                        }
                    }
                    catch { cell = "111-111-1111"; }

                    try
                    {
                        if (dr["EMAIL"] != null)
                        {
                            email = (dr["EMAIL"] ?? "").ToString();
                        }
                    }
                    catch { email = "None"; }




                    string Fullstring = (dr["NAME_1"] ?? "").ToString();
                    string LName   = Fullstring.Substring(0,Fullstring.IndexOf(",") );
                    string FName = Fullstring.Replace(LName, "").Replace(",","");
                    string address = (dr["ADDR_1"] ?? "").ToString();

                    Computyme.Models.Customer.Customer CustomerList = new Computyme.Models.Customer.Customer
                    {


                        Fname = FName,
                        Lname = LName,
                        Address = (dr["ADDR_1"] ?? "").ToString(),
                        Cell = cell,
                        Email = email
                    };

                    CustomerFound.Add(CustomerList);
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


            return Json(CustomerFound, JsonRequestBehavior.AllowGet);

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

           // OrderID = OrderID;
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
          
            string NewOrderID = null; //rnd.Next(1, 10000).ToString();   // creates a number between 1 and 6

            string s = ConfigurationManager.AppSettings["TESTString"];

            U2Connection con = new U2Connection();
            U2Command cmd = con.CreateCommand();
            con.ConnectionString = s;
            con.Open();

            cmd.CommandText = "SELECT MAX(RECID) FROM SHOPPINGCART";
            U2DataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string MaxID = dr["RECID"].ToString();

                NewOrderID = (Convert.ToInt32(MaxID) + 1).ToString();
              
            }
            con.Close();



            if (Request.Cookies["ComputymeOrder"] == null)
            {
                //Drop cookie
                HttpCookie myCookie = new HttpCookie("ComputymeOrder");
                myCookie["OrderID"] = NewOrderID;
                myCookie["OrderStatus"] = "New";
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);
            }
            else { 
            string OrderStatus;
            //if (Request.Cookies["ComputymeOrder"]["OrderID"] != null  )
            if (Request.Cookies["ComputymeOrder"]["OrderID"] != null  )
            {

                OrderStatus = Request.Cookies["ComputymeOrder"]["OrderStatus"];

                if (OrderStatus == "New")
                {
                    NewOrderID = Request.Cookies["ComputymeOrder"]["OrderID"];

                }
            }
            else
            {
                //Drop cookie
                HttpCookie myCookie = new HttpCookie("ComputymeOrder");
                myCookie["OrderID"] = NewOrderID;
                myCookie["OrderStatus"] = "New";
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);
            }
                // Check if it exists.
            }

            Computyme.Models.NewOrder.UniqueOrder OrderID = new Models.NewOrder.UniqueOrder { OrderID = NewOrderID, UserID = "4561" };

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