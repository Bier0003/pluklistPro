using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Data;
using System.Xml;
using System.Net.NetworkInformation;
using System.Net;


namespace Plukliste
{


    class ConnectDB

    {
        //method insert //object
        public static void InsertDB(Pluklist plukliste)

        {
            SqlCommand cmd;
            SqlDataAdapter rd;
            DataSet ds;
            SqlDataAdapter da;
            SqlConnection conn = new SqlConnection("Server=DESKTOP-PLCDDAP\\SQLEXPRESS;Initial Catalog=Plukliste;Integrated Security=True;TrustServerCertificate=true");

            int pluklisteId = -1;


            using (cmd = new SqlCommand("INSERT INTO pluklist(Name,Forsendelse,Address) VALUES(@Name,@Forsendelse,@Address); SELECT SCOPE_IDENTITY()", conn))
            {
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = plukliste.Name;
                cmd.Parameters.Add("@Forsendelse", SqlDbType.VarChar).Value = plukliste.Forsendelse;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = plukliste.Adresse;
                conn.Open();
                pluklisteId = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }

            foreach (var pluklistItem in plukliste.Lines)
            {
                using (cmd = new SqlCommand("INSERT INTO Item (Product_ID,Title,Type,Amount) VALUES(@Product_ID,@Title,@Type,@Amount); SELECT SCOPE_IDENTITY()", conn))
                {
                    cmd.Parameters.Add("@Product_ID", SqlDbType.VarChar).Value = pluklistItem.ProductID;
                    cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = pluklistItem.Title;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = pluklistItem.Type;
                    cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = pluklistItem.Amount;
                    conn.Open();

                    var listId = cmd.ExecuteScalar();

                    cmd = new SqlCommand("INSERT INTO ItemLines(Pluklist_id, Item_ID, Amount)VALUES(@Pluklist_id, @Item_id, @Amount)", conn);
                    cmd.Parameters.Add("@Pluklist_id", SqlDbType.Int).Value = pluklisteId;
                    cmd.Parameters.Add("@Item_id", SqlDbType.Int).Value = listId;
                    cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = pluklistItem.Amount;

                    cmd.ExecuteScalar();
                    conn.Close();

                }

            }

        }


    }


}
