using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Room.Models;
using System.Data;
using System.Data.SqlClient;

namespace Room.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RoomApiController(IConfiguration configuration)
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


        [HttpGet]
        [Route("GetRoomIdAsync")]
        public async Task<ActionResult> GetRoomIdAsync()
        {

            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync("http://194.233.69.20:3001/api/roomcode"))
                    {
                        dynamic apiResponse = await response.Content.ReadAsStringAsync();
                        dynamic jResult = JsonConvert.DeserializeObject(apiResponse).room_code;
                        string test = jResult;
                        return Ok(test);
                        ////return new JsonResult(new
                        ////{
                        ////    jResult,
                        ////});
                    }
                }
                catch (Exception ex)
                {

                    return Ok(ex.Message);
                }
            }

        }


        [HttpPost]
        [Route("InsertRoomId")]
        public JsonResult InsertRoomId(string roomId)
        {
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    var response = dbConnection.Query<RoomData>("Sp_InsertRoomId", new { @roomId = roomId },
                        commandType: CommandType.StoredProcedure);

                    if (response != null)
                    {
                        var a = response.FirstOrDefault()?.RoomId;

                        return new JsonResult(new
                        {
                            Message = "Save Successfully",
                            RoomId = response.FirstOrDefault()?.RoomId,
                        });

                    }
                    dbConnection.Close();

                    return new JsonResult(new
                    {
                        Message = response?.FirstOrDefault(),
                    });



                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Message = ex.Message,
                });
            }
        }
    }
}
