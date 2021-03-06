using EmailExtractRESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text.RegularExpressions;


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

            #region Check if all the opening xml tags have a corresponding closing tag
            // The regular expression to match the opening and lcosing xml tags
            string pattern = @"<.[^(><.)]+>";

            // create a regex object of the pattern and convert the occurances to an array
            Regex rg = new Regex(pattern);
            var matchedTags = rg.Matches(emailText).Cast<Match>().Select(m => m.Value).ToArray();

            Stack<string> tags = new Stack<string>(); 


            // iterate each tag found and add it to the dictionary. If it already exists, increment the count, else add it with count as 1 
            foreach (string tag in matchedTags)
            {
                // remove the forward slash in the closing tags
                string tagName = tag.Replace("/", "");

                if (tags.Count() == 0)
                    tags.Push(tagName);
                else if (tags.Peek() == tagName)
                    tags.Pop();
                else
                    tags.Push(tagName);
            }

            if (tags.Count() != 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The text provided is not a well-formed XML. It has missing closing tags.");

            #endregion

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
