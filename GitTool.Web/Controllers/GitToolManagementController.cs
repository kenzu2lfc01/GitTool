using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using GitTool.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace GitTool.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitToolManagementController : ControllerBase
    {
        private const string URL = @"https://tallyit.atlassian.net/rest/greenhopper/1.0/xboard/work/allData.json?rapidViewId=61&forceConsistency=true&etag=61,0,779f87da-575a-44f4-abab-9a1c3a7c9930:1629022201611,[],[],809816871";

        [HttpGet]
        public string PostFetchAllTicketsFromJira()
        {
            var headers = Request.Headers;
            StringValues cookie;
            if (headers.ContainsKey("CookieCustom"))
            {
                headers.TryGetValue("CookieCustom", out cookie);
            }

            var result = string.Empty;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("Cookie", cookie.ToString());


            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                result = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            client.Dispose();
            return result;
        }

        [HttpPost("create-branch")]
        public ActionResult CreateBranch([FromBody] CreateBranchNameModel model)
        {
            if (string.IsNullOrEmpty(model.ProjectDirectory)
                || string.IsNullOrEmpty(model.TicketType)
                || string.IsNullOrEmpty(model.TicketCode)
                || string.IsNullOrEmpty(model.TicketName)) return BadRequest();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/C cd " + model.ProjectDirectory;
            cmd.StartInfo.Arguments += $"& git checkout -b {model.TicketType}/{model.TicketCode}-{model.TicketName}";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();

            while (!cmd.StandardOutput.EndOfStream)
            {
                string line = cmd.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            return Ok();
        }
    }
}