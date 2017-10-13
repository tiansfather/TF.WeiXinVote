namespace TF.Controllers
{
    using System;
    using System.Web.Mvc;
    using TF;

    public class VCodeController : Controller
    {
        public ActionResult GetImg()
        {
            int width = 100;
            int height = 40;
            int fontSize = 20;
            string code = string.Empty;
            byte[] fileContents = ValidateCode.CreateValidateGraphic(out code, 4, width, height, fontSize);
            base.Session["VCode"] = code;
            return base.File(fileContents, "image/jpeg");
        }
    }
}

