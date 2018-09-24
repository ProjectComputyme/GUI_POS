using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using U2.Data.Client;
using U2.Data.Client.UO;
using Computyme.Models;
using System.Configuration;
using System.Data;

namespace Computyme.Managers
{
    public class HomeManager
    {
        //public static bool InsertOrderItem(Orders LineItem, string OrderNumber)
        //{

        //    string s = ConfigurationManager.AppSettings["TESTString"];
        //    U2Connection con = new U2Connection();
        //    con.ConnectionString = s;
        //    con.Open();

        //    UniSession LUniSession = con.UniSession;
        //    U2Command cmd = con.CreateCommand();

        //    int recID = 9999;
        //    int Prod = 1234;
        //    int quantity = 1;
        //    double cost = 9.99;




        //    //Insert 
        //    string lcmd = string.Format("INSERT INTO SHOPPINGCART RECID,PROD,QUAN,COST) VALUES('{0}','{1}','{2}','{3}')", recID, Prod, quantity, cost);
        //    cmd.CommandText = lcmd;

        //    try { int L2 = cmd.ExecuteNonQuery(); }
        //    catch(Exception ex) { return false; }
        //    finally { }

        //    return true;


        //}

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

                // As we add reccords, we will need to read , loop and drop into UniDynArray. 
                // For now, just enter line item. 



               
                //Sale.Insert(1, -1, price2);
                //Sale.Insert(1, -1, price3);

              
                string lCmd = string.Format("INSERT INTO SHOPPINGCART (RECID,PROD,QUAN,SALE) VALUES('{0}','{1}','{2}','{3}')", newrecid, Product, Quantity, Sale);
                //string lCmd = string.Format("INSERT INTO SHOPPINGCART VALUES (newrecid, Product, Quantity, Sale)");
                //string lCmd = string.Format("INSERT INTO SHOPPINGCART (PROD,QUAN,SALE) VALUES('{0}','{1}','{2}')", Product, Quantity, Sale);



                cmd.CommandText = lCmd;
                int l2 = cmd.ExecuteNonQuery();
               

                //print inserted value
                //cmd.CommandText = string.Format("Action=Select;File=SHOPPINGCART;Attributes=RECID,PROD,QUAN,SALE;Where=RECID={0}", newrecid);
                //U2DataAdapter da = new U2DataAdapter(cmd);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
              
                // delete inserted value
                //Console.WriteLine(Environment.NewLine + "Start : Delete new inserted value...............");
                //cmd.CommandText = string.Format("Action=Delete;File=SHOPPINGCART;Where=RECID={0}", newrecid);
                //l2 = cmd.ExecuteNonQuery();
                

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
                return false;
            }

            return true;
        }



        public static bool DeleteOrderItem(Orders LineItem)
        {
            return true;
        }


        }
}