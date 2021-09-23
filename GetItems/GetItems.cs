using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GetItems
{
    public static class GetItems
    {
        //[FunctionName("Function1")]
        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    return new OkObjectResult(responseMessage);
        //}

        [FunctionName("GetItems")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        [CosmosDB(databaseName: "outDatabase", collectionName: "MyCollection", SqlQuery = "SELECT top 100 * FROM item order by item._ts desc", ConnectionStringSetting = "ToDoListConnectionString")] IEnumerable<object> drivers,
        ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            if (drivers is null)
            {
                return new NotFoundResult();
            }

            foreach (var driver in drivers)
            {
                log.LogInformation(driver.ToString());
            }

            return new OkObjectResult(drivers);
        }
    }
}
