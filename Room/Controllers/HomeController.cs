using Microsoft.AspNetCore.Mvc;
using Room.Models;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Dapper;

namespace Room.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DbConnection");
            providerName = "System.Data.SqlClient";
        }

        public string ConnectionString { get; }
        public string providerName { get; }

        public IDbConnection Connection
        {
            get { return new SqlConnection(ConnectionString); }
        }



        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            RoomDataViewModel model = new RoomDataViewModel();

            List<RoomData> data = new List<RoomData>();
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    data = dbConnection.Query<RoomData>("Sp_GetRoomCode", commandType: CommandType.StoredProcedure).ToList();
                    dbConnection.Close();
                    
                    model.RoomCodeList = data;
                }
            }
            catch (Exception ex)
            {
                var errormsg = ex.Message;
                

            }


            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}