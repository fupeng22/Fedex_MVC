using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQLDAL;
using System.Text;

namespace FedexSystem.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string ValidateLogin(string UserName, string Password)
        {
            string strResult = "{\"result\":\"error\",\"message\":\"登录失败，原因未知\"}";

            UserName = Server.UrlDecode(UserName);
            Password = Server.UrlDecode(Password);
            try
            {
                if (!new T_Users().UserExists(UserName))
                {
                    strResult = "{\"result\":\"error\",\"message\":\"登录失败，该用户名不存在\"}";
                }
                else
                {
                    if (new T_Users().CheckLogin(UserName, Get_MD5(Password)))
                    {
                        Session["Global_UserName"] = UserName;
                        Session.Timeout = 480;
                        strResult = "{\"result\":\"ok\",\"message\":\"登录成功\"}";
                    }
                    else
                    {
                        strResult = "{\"result\":\"error\",\"message\":\"登录失败，用户名、密码不匹配\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                strResult = "{\"result\":\"error\",\"message\":\"登录失败，原因:" + ex.Message + "\"}";
            }

            return strResult;
        }

        //MD5加密
        public string Get_MD5(string strSource)
        {
            byte[] dataToHash = (new System.Text.ASCIIEncoding()).GetBytes(strSource);
            byte[] hashvalue = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(dataToHash);
            StringBuilder ret = new StringBuilder();
            foreach (byte b in hashvalue)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString().ToLower();
        }

        [HttpGet]
        public string LogOut()
        {
            string strRet = "{\"result\":\"error\",\"message\":\"原因未知\"}";

            try
            {
                Session["Global_UserName"] = null;

                strRet = "{\"result\":\"ok\",\"message\":\"" + "成功注销" + "\"}";
            }
            catch (Exception ex)
            {
                strRet = "{\"result\":\"error\",\"message\":\"" + ex.Message + "\"}";
            }

            return strRet;
        }
    }
}
