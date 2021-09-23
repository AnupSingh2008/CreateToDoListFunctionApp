using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CreateToDoListFunctionApp
{
    public static class createcosmodb
    {
        [FunctionName("createcosmodb")]
        public static async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
         [CosmosDB(
            databaseName: "ToDoList",
            collectionName: "items",
            ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<dynamic> documentsOut,
         ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (!string.IsNullOrEmpty(name))
            {
                log.LogInformation("Saving document.....");
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = System.Guid.NewGuid().ToString(),
                    name = name
                });
                return (ActionResult)new OkResult();
            }
            else
            {
                log.LogInformation("Invalid Request...");
                documentsOut = null;
                return (ActionResult)new BadRequestResult();
            }
        }
    }
}
