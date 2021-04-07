using EmailExtractRESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace EmailExtractRESTfulAPI.Controllers
{
    public class gstController : ApiController
    {
        // GET: api/gst
        public HttpResponseMessage Get(string emailText)
        {
            char[] delimeter = { '<' };
            double total = 0;

            // list of the the xml tags to be searched
            List<string> lstTags = new List<string> { "<cost_centre>", "<total>" };

            #region Get the Cost Centre Value
            string costCentreValue = "UNKNOWN";

            // if the email text contains the <cost_centre> tag, then extract the value, else it is the default value of UNKNOWN
            if (emailText.Contains(lstTags[0]))
            {
                costCentreValue = emailText.Substring(emailText.IndexOf(lstTags[0]) + lstTags[0].Length + 1).Split(delimeter)[0].Trim();
            }
            #endregion

            #region Get the Total amount excluding GST (assuming the GST to be 15%)
            try
            {
                total = double.Parse(emailText.Substring(emailText.IndexOf(lstTags[1]) + lstTags[1].Length).Split(delimeter)[0].Trim()) / 1.15;
            }
            catch (System.FormatException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The text format is incorrect. It is missing <total> tag");
            }
            #endregion

            #region Create the response object including the Cost Centre, Total without GST and the GST amount
            GstInfo gstObj = new GstInfo
            {
                CostCentre = costCentreValue,
                Total = Math.Round(total, 2),
                Gst = Math.Round(0.15 * total, 2)
            };
            #endregion

            return Request.CreateResponse(HttpStatusCode.OK, gstObj);
        }
    }
}
