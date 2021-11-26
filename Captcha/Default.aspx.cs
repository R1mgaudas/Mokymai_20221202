using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Services;

public partial class _Default : System.Web.UI.Page
{
    protected static string ReCaptcha_Key = "<RECaptcha Site Key>";
    protected static string ReCaptcha_Secret = "<RECaptcha Secret Key>";

    [WebMethod]
    public static string VerifyCaptcha(string response)
    {
        string url = "https://www.google.com/recaptcha/api/siteverify?secret=" + ReCaptcha_Secret + "&response=" + response;
        return (new WebClient()).DownloadString(url);
    }
}