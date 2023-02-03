using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Drawing;

namespace Ado.NetCategoryProductProject.Models
{
    public class DataBase
    {
        string cs  = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;

        public List<Product> Getproducts ()
        {
            List<Product> products= new List<Product>();    
            SqlConnection conn =  new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_GetAllProductList", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader(); 
              

            while(reader.Read())
            {
                Product  p = new Product(); 
                 p.ProductId=Convert.ToInt32( reader.GetValue(0).ToString());
                p.ProductName = reader.GetValue(1).ToString();

              
                p.Prise = Convert.ToInt32( reader.GetValue(2).ToString() );
                p.Description = reader.GetValue(3).ToString();

                p.CategoryId = Convert.ToInt32(reader.GetValue(4).ToString());

                products.Add(p);     
            }
            return products;
            conn.Close();

        }
        public List<Category> GetCategories ()
        {
            List<Category> categories = new List<Category>();

            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_cateryList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                Category c = new Category();
                c.CategoryId = Convert.ToInt32( reader.GetValue(0).ToString());
                c.CategoryName = reader.GetValue(1).ToString();
                c.Status = Convert.ToInt32(reader.GetValue(2).ToString());
                categories.Add(c);

            }
            return categories;
            conn.Close();
            
        }
    }
}