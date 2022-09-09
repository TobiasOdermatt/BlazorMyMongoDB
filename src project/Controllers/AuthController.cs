﻿using BlazorServerMyMongo.Data.DB;
using BlazorServerMyMongo.Data.OTP;
using BlazorServerMyMongo.Objects;
using Microsoft.AspNetCore.Mvc;

namespace BlazorServerMyMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private Object GenerateUUIDObject(string UUID)
        {
            string uuid = UUID;
            return new { uuid = uuid };
        }

        [HttpPost("CreateOTP")]
        //If data is correct UUID will be returned
        public IActionResult CreateOTP(ConnectRequestObject dataJSON)
        {
            DBConnector.Client = null;
            DBController.Client = null;
            OTPManagement otpManagement = new();
            string? decryptedData = otpManagement.DecryptUserData(dataJSON.AuthCookieKey, dataJSON.RandData);
            if (decryptedData is null)
                return NoContent();

            (string username, string password) = otpManagement.getUserData(decryptedData);
            DBConnector connector = new(username, password);

            //If connection is successful, UUID will be returned
            if (DBController.Client != null)
            {
                var uuid = Guid.NewGuid().ToString();
                var uuidObject = GenerateUUIDObject(uuid);

                DateTime localDate = DateTime.Now;
                OTPFileObject newFile = new(localDate, dataJSON.RandData);
                OTPFileManagement fileManger = new();
                fileManger.WriteOTPFile(uuid, newFile);

                fileManger.CleanUpOTPFiles();

                return new JsonResult(uuidObject);
            }
            return NoContent();
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            string? uuid = HttpContext.Request.Cookies["UUID"];
            if (uuid is null)
                return Redirect("/Connect");

            OTPFileManagement fileManger = new();
            DBConnector.Client = null;
            DBController.Client = null;
            fileManger.DeleteOTPFile(uuid);
            HttpContext.Response.Cookies.Delete("UUID");
            HttpContext.Response.Cookies.Delete("Token");
            return Redirect("/Connect");
        }
    }
}
