using Ado.NetCategoryProductProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Dynamic;
using System.Threading.Tasks;

namespace Ado.NetCategoryProductProject.Controllers
{
    public class ProductController : Controller
    {
        DataBase db = new DataBase();
        string cs = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
        public async Task<ActionResult> ListOfProduct(int CategoryId, int b=3 , int a=1)
        {
            double count = db.Getproducts().Where( c =>c.CategoryId== CategoryId).Count()  ;

            Session["CategoryId"] = CategoryId;
            ViewBag.TotalPages = Math.Ceiling(count / b);
            List<Product> products = new List<Product>();
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_ProductPaggig", conn);
            cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            cmd.Parameters.AddWithValue("@PageIndex", a);
            cmd.Parameters.AddWithValue("@PageSize", b);
            cmd.CommandType = CommandType.StoredProcedure;
          

            conn.Open();
            SqlDataReader reader =await cmd.ExecuteReaderAsync();


            while (reader.Read())
            {
                Product p = new Product();
                p.ProductId = Convert.ToInt32(reader.GetValue(0).ToString());
                p.ProductName = reader.GetValue(1).ToString();
                p.Prise = Convert.ToInt32(reader.GetValue(2).ToString());
                p.Description = reader.GetValue(3).ToString();
                p.CategoryId = Convert.ToInt32(reader.GetValue(4).ToString());

                products.Add(p);
            }
           
            conn.Close();
            return View(products);
        }

        public ActionResult Edit(int ProductId)
        {

            var data =  db.Getproducts();
           var productdetails=   data.Single(c => c.ProductId == ProductId);

            return View(productdetails);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Product p)
        {
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_UpdateProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductId", p.ProductId);
            cmd.Parameters.AddWithValue("@ProductName", p.ProductName);
            cmd.Parameters.AddWithValue("@Prise", p.Prise);
            cmd.Parameters.AddWithValue("@Description", p.Description);
            
            cmd.Parameters.AddWithValue("@CategoryId", p.CategoryId);
            conn.Open();

            int result = await cmd.ExecuteNonQueryAsync();

            if(result != null)
            {
                 return RedirectToAction("ListOfProduct" ,new { p.CategoryId });  
            }

            return RedirectToAction("ListOfProduct", new { p.CategoryId } );
            conn.Close();

        }

        public ActionResult Details(int ProductId)
        {
            var data = db.Getproducts();
            var productdetails = data.Single(c => c.ProductId == ProductId);
            return View(productdetails);
        }

        public async Task<ActionResult> Delete(int ProductId)
        {
            var data = db.Getproducts().Single(c => c.ProductId == ProductId);
            var CategoryId = data.CategoryId;
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_DeleteProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductId", ProductId);
            conn.Open();
            await  cmd.ExecuteNonQueryAsync();
            conn.Close();
            return RedirectToAction("ListOfProduct", new { CategoryId });


        }

         public ActionResult Create(int CategoryId)
        {
            Session["CategoryId"] = CategoryId;
            return View();  
        }

        [HttpPost]
        public async Task<ActionResult> Create( Product p)
        {
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_addProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;
       
            cmd.Parameters.AddWithValue("@ProductName", p.ProductName);
            cmd.Parameters.AddWithValue("@Prise", p.Prise);
            cmd.Parameters.AddWithValue("@Description", p.Description);

            cmd.Parameters.AddWithValue("@CategoryId", p.CategoryId);
            conn.Open();

            int result = await cmd.ExecuteNonQueryAsync();

           
            conn.Close();

            return RedirectToAction("ListOfProduct", new { p.CategoryId });
        }
    }
}