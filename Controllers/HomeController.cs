using DesasterAlleviationFund.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using DesasterAlleviationFund.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using NuGet.Protocol.Plugins;
using Humanizer;

namespace DesasterAlleviationFund.Controllers
{
    public class HomeController : Controller
    {
        string conn = "Server=tcp:st10152693.database.windows.net,1433;Initial Catalog=Shohiwa’s Tunes;Persist Security Info=False;User ID=DjAdmin;Password=Magagula15;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly ILogger<HomeController> _logger;
        public IActionResult Index1()
        {
            return View();
        }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registration(Registration reg)
        {
            if (reg != null)
            {
                if (!string.IsNullOrEmpty(reg.Email) &&
                    !string.IsNullOrEmpty(reg.First_Name) &&
                    !string.IsNullOrEmpty(reg.Last_Name) &&
                    !string.IsNullOrEmpty(reg.Password))
                {
                    // Your database logic here
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        string query = "INSERT INTO Registration VALUES(@Email,@First_Name, @Last_Name, @Password)";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", reg.Email.ToString());
                            cmd.Parameters.AddWithValue("@First_Name", reg.First_Name.ToString());
                            cmd.Parameters.AddWithValue("@Last_Name", reg.Last_Name.ToString());
                            cmd.Parameters.AddWithValue("@Password", reg.Password.ToString());
                            cmd.ExecuteNonQuery();
                            con.Close();
                            TempData["Message"] = "User registered successfully you can Login!!!";

                        }
                    }
                }
                else
                {
                    // Handle missing properties
                    TempData["Message"] = "One or more fields are empty.";
                }
            }
            else
            {
                // Handle null 'reg' object
                TempData["Message"] = "Registration object is null.";
            }

            

            return View();
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignIn(Registration reg)
        {
            using(SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                string query = "SELECT * FROM Registration WHERE Email ='" + reg.Email + "' AND Password = '" + reg.Password + "'";
                DataTable dt=new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                adapter.Fill(dt);
                if(dt.Rows.Count > 0 )
                {
                    // Session["Email"] = reg.Email.ToString();

                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, reg.Email) // Assuming Email is used for authentication
        };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuthenticationScheme");
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync("MyCookieAuthenticationScheme", principal);

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Invalid username or password!!!";
                    return View();
                }
            }
            
        }
        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

            return RedirectToAction("SignIn");
        }
        [HttpGet]
        public IActionResult Monentart_Donations()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Monentart_Donations(Monetory mon)
        {
            string userEmail = User.Identity.Name;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                string query = "INSERT INTO Monetory (Email, DonorName, Date, Amount) VALUES (@Email, @DonorName, @Date, @Amount)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string don = "Anonymous";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Email", userEmail);

                    if (mon.DonorName == "other")
                    {
                        cmd.Parameters.AddWithValue("@DonorName", mon.DonorNameInput);
                    }
                    else if (mon.DonorName == "Anonymous")
                    {
                        cmd.Parameters.AddWithValue("@DonorName", don);
                    }
                    else
                    {
                        TempData["Message"] = "Invalid selection!";
                        return View();
                    }

                    cmd.Parameters.AddWithValue("@Date", mon.Date);
                    cmd.Parameters.AddWithValue("@Amount", mon.Amount);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Donation added successfully!";
                        return RedirectToAction("ListMonetary");
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error adding donation: " + ex.Message;
                        return View();
                    }
                }
            }
        }
        public IActionResult ListMonetary(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conn))
            {
                string userEmail = User.Identity.Name;
                con.Open();
                SqlDataAdapter ad = new SqlDataAdapter("Select * from Monetory where Email = '" + userEmail + "'", con);
                ad.Fill(dt);
            }
            return View(dt);

            
            /*List<Monetory> donations = new List<Monetory>();

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlDataAdapter ad = new SqlDataAdapter("Select * from Monetory where Email = '" + userEmail + "'", con);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    Monetory donation = new Monetory
                    {
                        Email = row["Email"].ToString(),
                        DonorName = row["DonorName"] != DBNull.Value ? row["DonorName"].ToString() : string.Empty,
                        Date = row["Date"] != DBNull.Value ? (DateTime)row["Date"] : DateTime.MinValue,
                        Amount = (decimal)row["Amount"]
                    };
                    donations.Add(donation);
                }
            }

            return View(donations);
            */
        }
        [HttpGet]
        public IActionResult Good_Donations()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Good_Donations(Good good)
        {
            string userEmail = User.Identity.Name;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                string query = "INSERT INTO Good (Email, DonarName, Date, NumItems, Category, Description) VALUES (@Email, @DonarName, @Date, @NumItems, @Category, @Description)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string don = "Anonymous";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Email", userEmail);

                    if (good.DonarName == "other")
                    {
                        cmd.Parameters.AddWithValue("@DonarName", good.DonorNameInput);
                    }
                    else if (good.DonarName == "Anonymous")
                    {
                        cmd.Parameters.AddWithValue("@DonarName", don);
                    }
                    else
                    {
                        TempData["Message"] = "Invalid selection!";
                        return View();
                    }

                    cmd.Parameters.AddWithValue("@Date", good.Date);
                    cmd.Parameters.AddWithValue("@NumItems", good.NumItems);
                    cmd.Parameters.AddWithValue("@Category", good.Category);
                    cmd.Parameters.AddWithValue("@Description", good.Description);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Donation added successfully!";
                        return RedirectToAction("ListMonetary");
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error adding donation: " + ex.Message;
                        return View();
                    }
                }
            }
        }
        public IActionResult ListGoods()
        {

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conn))
            {
                string userEmail = User.Identity.Name;
                con.Open();
                SqlDataAdapter ad = new SqlDataAdapter("Select * from Good where Email = '" + userEmail + "'", con);
                ad.Fill(dt);
            }
            return View(dt);
          
        }
        [HttpGet]
        public IActionResult Disaster()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Disaster(Disaster dis)
        {
            string userEmail = User.Identity.Name;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                string query = "INSERT INTO Disaster (Email, Start_date, End_date, Location, Description, Aid_types)" +
                    " VALUES (@Email, @Start_date, @End_date, @Location, @Description, @Aid_types)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Email", userEmail);
                    cmd.Parameters.AddWithValue("@Start_date", dis.Start_date);
                    cmd.Parameters.AddWithValue("@End_date", dis.End_date);
                    cmd.Parameters.AddWithValue("@Location", dis.Location);
                    cmd.Parameters.AddWithValue("@Description", dis.Description);
                    cmd.Parameters.AddWithValue("@Aid_types", dis.Aid_types);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Donation added successfully!";
                        return RedirectToAction("ListDisaster");
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error adding donation: " + ex.Message;
                        return View();
                    }
                }
            }
        }
        [HttpGet]
        public IActionResult ListDisaster()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conn))
            {
                string userEmail = User.Identity.Name;
                con.Open();
                SqlDataAdapter ad = new SqlDataAdapter("Select * from Disaster where Email = '" + userEmail + "'", con);
                ad.Fill(dt);
            }
            return View(dt);
            /*
            string userEmail = User.Identity.Name;
            List<Disaster> donations = new List<Disaster>();

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlDataAdapter ad = new SqlDataAdapter("Select * from Disaster where Email = '" + userEmail + "'", con);
                DataTable dt = new DataTable();
                ad.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    Disaster donation = new Disaster
                    {
                        Email = row["Email"].ToString(),
                        Start_date = row["Start_date"] != DBNull.Value ? (DateTime)row["Start_Date"] : DateTime.MinValue,
                        End_date = row["End_date"] != DBNull.Value ? (DateTime)row["End_date"] : DateTime.MinValue,
                        Location = row["Location"] != DBNull.Value ? row["Location"].ToString() : string.Empty,
                        Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : string.Empty,
                        Aid_types = row["Aid_types"] != DBNull.Value ? row["Aid_types"].ToString() : string.Empty,

                    };
                    donations.Add(donation);
                }
            }

            return View(donations);
            */
        }

        [HttpGet]
        public IActionResult EditMonetary(int id, Monetory monetary)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "SELECT * FROM Monetory WHERE ID = @ID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@ID", id);
                    adapter.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    monetary.ID = Convert.ToInt32(dt.Rows[0][0].ToString());
                    monetary.Email = dt.Rows[0][1].ToString();
                    monetary.DonorName = dt.Rows[0][2].ToString();
                    monetary.Date = Convert.ToDateTime(dt.Rows[0][3].ToString());
                    monetary.Amount = Convert.ToInt32(dt.Rows[0][4].ToString());
                    return View(monetary);
                }
                else
                {
                    return RedirectToAction("MonetaryList");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult EditMonetary(Monetory mon) 
        {
            string userEmail = User.Identity.Name;

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                string query = "UPDATE Monetory SET Email = @Email, DonorName = @DonorName, Date = @Date, Amount = @Amount WHERE ID = @ID";

                 using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string don = "Anonymous";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("ID", mon.ID);
                    cmd.Parameters.AddWithValue("@Email", userEmail);
                    cmd.Parameters.AddWithValue("@DonorName", don);
                    cmd.Parameters.AddWithValue("@Date", mon.Date);
                    cmd.Parameters.AddWithValue("@Amount", mon.Amount);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Donation added successfully!";
                        return RedirectToAction("ListMonetary");
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error adding donation: " + ex.Message;
                        return View();
                    }

                }
                //return View();
            }
        }
        public IActionResult DeletMonetary(int id)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string query = "DELETE Monetory WHERE ID = @ID";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("Deleted Successfully!!!!!!");
                    }
                    con.Close();
                    return RedirectToAction("ListMonetary", "Home");
                }

            }
            catch (Exception ex)
            {

                return View(ex.Message);
            }
        }
        public IActionResult MonentaryDeatils(int id, Monetory monetary)
        {
            string userEmail = User.Identity.Name;
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "SELECT * FROM Monetory WHERE ID = @ID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@ID", id);
                    adapter.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    monetary.ID = Convert.ToInt32(dt.Rows[0][0].ToString());
                    monetary.Email = dt.Rows[0][1].ToString();
                    monetary.DonorName = dt.Rows[0][2].ToString();
                    monetary.Date = Convert.ToDateTime(dt.Rows[0][3].ToString());
                    monetary.Amount = Convert.ToInt32(dt.Rows[0][4].ToString());
                    return View(monetary);
                }
                else
                {
                    return RedirectToAction("ListMonetary");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public IActionResult EditGoods(int id, Good good)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "SELECT * FROM Good WHERE GoodId = @GoodId";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@GoodId", id);
                    adapter.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    good.GoodId = Convert.ToInt32(dt.Rows[0][0].ToString());
                    good.Email = dt.Rows[0][1].ToString();
                    good.DonarName = dt.Rows[0][2].ToString();
                    good.Date = Convert.ToDateTime(dt.Rows[0][3].ToString());
                    good.NumItems = Convert.ToInt32(dt.Rows[0][4].ToString());
                    good.Category = dt.Rows[0][5].ToString();
                    good.Description = dt.Rows[0][6].ToString();
                    return View(good);
                }
                else
                {
                    return RedirectToAction("ListGoods");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult EditGoods(Good good)
        {
            string userEmail = User.Identity.Name;
            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string query = "UPDATE Good SET Email = @Email, DonarName = @DonarName, Date = @Date, NumItems = @NumItems, Category = @Category, Description = @Description  WHERE GoodId = @GoodId";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@GoodId", good.GoodId);
                        cmd.Parameters.AddWithValue("@Email", userEmail);
                        cmd.Parameters.AddWithValue("@DonarName", good.DonarName);
                        cmd.Parameters.AddWithValue("@Date", good.Date);
                        cmd.Parameters.AddWithValue("@NumItems", good.NumItems);
                        cmd.Parameters.AddWithValue("@Category", good.Category);
                        cmd.Parameters.AddWithValue("@Description", good.Description);

                        cmd.ExecuteNonQuery();
                       // MessageBox.Show("Saved Successfully!!!!!!");
                    }
                    con.Close();
                    return RedirectToAction("ListGoods", "Home");
                }

            }
            catch (Exception ex)
            {

                return View(ex.Message);
            }

        }
        public IActionResult DeletGoods(int id)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string query = "DELETE Good WHERE GoodId = @GoodId";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@GoodId", id);
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("Deleted Successfully!!!!!!");
                    }
                    con.Close();
                    return RedirectToAction("ListGoods", "Home");
                }

            }
            catch (Exception ex)
            {

                return View(ex.Message);
            }
        }
        public IActionResult GoodDeatils(int id, Good good)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "SELECT * FROM Good WHERE GoodId = @GoodId";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@GoodId", id);
                    adapter.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    good.GoodId = Convert.ToInt32(dt.Rows[0][0].ToString());
                    good.Email = dt.Rows[0][1].ToString();
                    good.DonarName = dt.Rows[0][2].ToString();
                    good.Date = Convert.ToDateTime(dt.Rows[0][3].ToString());
                    good.NumItems = Convert.ToInt32(dt.Rows[0][4].ToString());
                    good.Category = dt.Rows[0][5].ToString();
                    good.Description = dt.Rows[0][6].ToString();
                    return View(good);
                }
                else
                {
                    return RedirectToAction("ListGoods");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public IActionResult EditDisaster(int id, Disaster disaster)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "SELECT * FROM Disaster WHERE DisasterID = @DisasterID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@DisasterID", id);
                    adapter.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    disaster.DisasterId = Convert.ToInt32(dt.Rows[0][0].ToString());
                    disaster.Email = dt.Rows[0][1].ToString();
                    disaster.Start_date = Convert.ToDateTime(dt.Rows[0][2].ToString());
                    disaster.End_date = Convert.ToDateTime(dt.Rows[0][3].ToString());
                    disaster.Location = dt.Rows[0][4].ToString();
                    disaster.Description = dt.Rows[0][5].ToString();
                    disaster.Aid_types = dt.Rows[0][6].ToString();
                    return View(disaster);
                }
                else
                {
                    return RedirectToAction("ListDisaster");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult EditDisaster(Disaster disaster)
        {
            string userEmail = User.Identity.Name;
            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string query = "UPDATE Disaster SET Email = @Email, Start_date = @Start_date, End_date = @End_date, Location = @Location, Description = @Description, Aid_types = @Aid_types  WHERE DisasterID = @DisasterID";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@DisasterID", disaster.DisasterId);
                        cmd.Parameters.AddWithValue("@Email", userEmail);
                        cmd.Parameters.AddWithValue("@Start_date", disaster.Start_date);
                        cmd.Parameters.AddWithValue("@End_date", disaster.End_date);
                        cmd.Parameters.AddWithValue("@Location", disaster.Location);
                        cmd.Parameters.AddWithValue("@Description", disaster.Description);
                        cmd.Parameters.AddWithValue("@Aid_types", disaster.Aid_types);

                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("Saved Successfully!!!!!!");
                    }
                    con.Close();
                    return RedirectToAction("ListDisaster", "Home");
                }

            }
            catch (Exception ex)
            {

                return View(ex.Message);
            }
        }
        public IActionResult DeletDisaster(int id)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string query = "DELETE Disaster WHERE DisasterID = @DisasterID";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@DisasterID", id);
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("Deleted Successfully!!!!!!");
                    }
                    con.Close();
                    return RedirectToAction("ListDisaster", "Home");
                }

            }
            catch (Exception ex)
            {

                return View(ex.Message);
            }
        }
        public IActionResult DisasterDetails(int id, Disaster disaster)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    string query = "SELECT * FROM Disaster WHERE DisasterID = @DisasterID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@DisasterID", id);
                    adapter.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    disaster.DisasterId = Convert.ToInt32(dt.Rows[0][0].ToString());
                    disaster.Email = dt.Rows[0][1].ToString();
                    disaster.Start_date = Convert.ToDateTime(dt.Rows[0][2].ToString());
                    disaster.End_date = Convert.ToDateTime(dt.Rows[0][3].ToString());
                    disaster.Location = dt.Rows[0][4].ToString();
                    disaster.Description = dt.Rows[0][5].ToString();
                    disaster.Aid_types = dt.Rows[0][6].ToString();
                    return View(disaster);
                }
                else
                {
                    return RedirectToAction("ListDisaster");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}