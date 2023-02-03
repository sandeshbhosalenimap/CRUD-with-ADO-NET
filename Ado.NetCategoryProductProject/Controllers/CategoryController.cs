using Ado.NetCategoryProductProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Ado.NetCategoryProductProject.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        DataBase db = new DataBase();

        string cs = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
        public async Task<ActionResult> Index(int  b = 3 , int a=1)
        {
             double  count= db.GetCategories().Count();


            ViewBag.TotalPages = Math.Ceiling(count / b);

            List<Category> categories = new List<Category>();
             SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("Sp_CategoryPagging", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", a);
            cmd.Parameters.AddWithValue("@pagiSize", b);

            conn.Open();
             SqlDataReader reader =  await cmd.ExecuteReaderAsync();


             while (reader.Read())
            {
                Category c = new Category();
                c.CategoryId = Convert.ToInt32(reader.GetValue(0).ToString());
                c.CategoryName = reader.GetValue(1).ToString();
                c.Status = Convert.ToInt32(reader.GetValue(2).ToString());
                 categories.Add(c);
            }
            conn.Close();
            return View(categories);


        }

        
        public async Task<ActionResult> Deactive( int CategoryId)
        {
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_ActiveDeactive", conn);
            cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            cmd.Parameters.AddWithValue("@Status", 0);

            cmd.CommandType = CommandType.StoredProcedure;


            conn.Open();
           await cmd.ExecuteNonQueryAsync();
            conn.Close();

            return  RedirectToAction("Index");  
        }
       
        public async Task<ActionResult> Active(int CategoryId)
        {
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_ActiveDeactive", conn);
            cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            cmd.Parameters.AddWithValue("@Status", 1);

            cmd.CommandType = CommandType.StoredProcedure;


            conn.Open();
            await cmd.ExecuteNonQueryAsync();
            conn.Close();   

            return RedirectToAction("Index");
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public  async Task<ActionResult> Create( Category c)
        {
            SqlConnection conn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("sp_addCategory", conn);
            cmd.CommandType = CommandType.StoredProcedure;
           
            cmd.Parameters.AddWithValue("@CategoryName", c.CategoryName);
            cmd.Parameters.AddWithValue("@Status", c.Status);
          
            conn.Open();

            await cmd.ExecuteNonQueryAsync();

             
          return View();    
        }





    }
}