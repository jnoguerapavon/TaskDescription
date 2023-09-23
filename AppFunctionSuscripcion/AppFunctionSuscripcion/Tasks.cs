using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AppFunctionSuscripcion
{
    public class Tasks
    {
        private readonly ILogger _logger;

        public Tasks(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Tasks>();
        }

        public static readonly List<Task> Items = new List<Task>();


    

        [Function("CreateTasks")]
        public static async Task<IActionResult> CreateTask(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "post", Route = "tasks")]
            HttpRequestData req)
        {
           
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TaskCreateModel>(requestBody);

            var task = new Task() { TaskDescription = input.TaskDescription };
            Items.Add(task);
            return new OkObjectResult(task);
        }





        [Function("GetAllTasks")]
        public static IActionResult GetAllTasks(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", Route = "tasks")]
            HttpRequestData req)
        {
           
            return new OkObjectResult(Items);
        }









        [Function("GetTaskById")]        
        public static IActionResult GetTaskById(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", Route = "tasks/{id}")]
            HttpRequestData req, string id)
        {
            var task = Items.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(task);
        }




        [Function("UpdateTasks")]
        public static async Task<IActionResult> UpdateTask(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "put", Route = "tasks/{id}")]
            HttpRequestData req, string id)
        {
            var task = Items.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TaskUpdateModel>(requestBody);

            task.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.TaskDescription))
            {
                task.TaskDescription = updated.TaskDescription;
            }
            return new OkObjectResult(task);
        }



        [Function("DeleteTask")]
        public static IActionResult DeleteTask(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "delete", Route = "tasks/{id}")]
            HttpRequestData req, string id)
        {
            var task = Items.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return new NotFoundResult();
            }
            Items.Remove(task);
            return new OkResult();
        }









    }
}
