using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using U2.Data.Client;
using U2.Data.Client.UO;
using Computyme.Models;
using System.Configuration;
using System.Data;

namespace Computyme.Manager
{
    public class HomeManager
    {
       

        public static bool InsertOrderItem(List<Orders> LineItem, string OrderNumber)
        {
            try
            {
                U2ConnectionStringBuilder conn_bldr = new U2ConnectionStringBuilder();
                conn_bldr.UserID = "Demo";
                conn_bldr.Password = "Demo";
                conn_bldr.Server = "localhost";
                conn_bldr.ServerType = "universe";
                conn_bldr.Database = "pwdemo";
                conn_bldr.AccessMode = "Native";
                conn_bldr.RpcServiceType = "uvcs";


          

                string lConnStr = conn_bldr.ConnectionString;
                U2Connection lConn = new U2Connection();
                lConn.ConnectionString = lConnStr;
                lConn.Open();
                UniSession lUniSession = lConn.UniSession;             
                U2Command cmd = lConn.CreateCommand();

                //Unique sale ID or invoice Number. Do we add userid as well?
                // We will work with the assumption that it's one order - 5001
                int newrecid = 5001;


                UniDynArray Product = new UniDynArray(lUniSession);
                UniDynArray Quantity = new UniDynArray(lUniSession);
                UniDynArray Sale = new UniDynArray(lUniSession);



                foreach (var order in LineItem)
                {
                    string ProdID = order.Serial.ToString();
                    string Quan = order.Quantity.ToString();                 
                    string price = order.Cost.ToString();
    
                        Product.Insert(1, -1, ProdID);
                        Quantity.Insert(1, -1, Quan);
                        Sale.Insert(1, -1, price);
            
                }

              
                string lCmd = string.Format("INSERT INTO SHOPPINGCART (RECID,PROD,QUAN,SALE) VALUES('{0}','{1}','{2}','{3}')", newrecid, Product, Quantity, Sale);

                cmd.CommandText = lCmd;
                int l2 = cmd.ExecuteNonQuery();

                lConn.Close();
          

            }
            catch (Exception e2)
            {
                string lErr = e2.Message;
                if (e2.InnerException != null)
                {
                    lErr += lErr + e2.InnerException.Message;
                }
                return false;
            }

            return true;
        }

        public static List<Orders> ReadExistingOrders(String OrderID)
        {            
            List<Orders> Cart = new List<Orders>();
            
            OrderID = "5001";
            string connection = ConfigurationManager.AppSettings["MVWriter"];

            U2Connection con = new U2Connection();          
            con.ConnectionString = connection;
            con.Open();
            U2Command cmd = con.CreateCommand();
            cmd.CommandText = "SELECT PROD, QUAN, COST FROM UNNEST SHOPPINGCART ON CARTS WHERE RECID = '" + OrderID + "'"; 
            DataSet ds = new DataSet();
            U2DataAdapter da = new U2DataAdapter(cmd);
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                Orders ExistingLineItems = new Orders { Cost = dr["COST"].ToString(), Quantity = dr["QUAN"].ToString(), Serial = dr["PROD"].ToString() };
                Cart.Add(ExistingLineItems);

            }

            con.Close();


            return Cart;
        }

        public static bool  DeleteProducts(List<Orders> Cart,string OrderID, string Product)
        {
            ///     // First Insert, then Print, then Delete
            try
            {
                Console.WriteLine(Environment.NewLine + "Start...");
                U2ConnectionStringBuilder conn_bldr = new U2ConnectionStringBuilder();
                conn_bldr.UserID = "Demo";
                conn_bldr.Password = "Demo";
                conn_bldr.Server = "localhost";
                conn_bldr.ServerType = "universe";
                conn_bldr.Database = "pwdemo";
                conn_bldr.AccessMode = "Native";
                conn_bldr.RpcServiceType = "uvcs";
                string lConnStr = conn_bldr.ConnectionString;
                U2Connection lConn = new U2Connection();
                lConn.ConnectionString = lConnStr;
                lConn.Open();
                UniSession lUniSession = lConn.UniSession;
              
                U2Command cmd = lConn.CreateCommand();

                // delete inserted value
             
                cmd.CommandText = string.Format("Action=Delete;File=SHOPPINGCART;Where=newrecid={0}", OrderID);
                int l2 = cmd.ExecuteNonQuery();
               

                //close connection
                lConn.Close();
         

            }
            catch (Exception e2)
            {
                string lErr = e2.Message;
                if (e2.InnerException != null)
                {
                    lErr += lErr + e2.InnerException.Message;
                }
              
            }
           // public static bool InsertOrderItem(List<Orders> LineItem, string OrderNumber)

    
                return true;
        }


        public static List<Orders> GetProducts(List<Orders> Cart)
        {

            List<Orders> shoppingCart = Cart;


            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = "Demo";
            conn_str.Password = "Demo";
            conn_str.Server = "localhost";
            conn_str.Database = "pwdemo";
            conn_str.ServerType = "UNIVERSE";
            conn_str.Pooling = false;
            string s = conn_str.ToString();
            U2Connection con1 = new U2Connection();
            con1.ConnectionString = s;
            con1.Open();
            Console.WriteLine("Connected.........................");
            U2Command cmd1 = con1.CreateCommand();

            foreach (var Product in shoppingCart)
            {
                cmd1.CommandText = "SELECT [SALE] FROM IVMST WHERE ID=" + Product.Serial.ToString();
                U2DataReader dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {
                    Product.ProdDescription = string.Format(dr1["SALE"].ToString());
                }
            }



            con1.Close();

            return shoppingCart;

        }

        public static bool DeleteOrderItem(Orders LineItem)
        {
            return true;
        }


        }
}