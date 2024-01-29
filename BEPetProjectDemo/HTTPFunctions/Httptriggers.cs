using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using System.Net.Http;

namespace BEPetProjectDemo
{
    public class Httptriggers
    {
        private const string DatabaseName = "PatientsDetails";
        private const string CollectionName = "Patients";
        private readonly CosmosClient _cosmosClient;
        private Container documentContainer;
        public Httptriggers(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            documentContainer = _cosmosClient.GetContainer("PatientsDetails", "Patients");
        }
        [FunctionName(HTTPFunctions.Get)]
        public async Task<IActionResult> GetallPatients(
                [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.GET, Route = HTTPRoutes.GetRoute)] HttpRequestMessage req,
                [CosmosDB(
                DatabaseName,
                CollectionName,
                Connection ="CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<PatientsInfo> patient,
                ILogger log)
        {
            log.LogInformation("Getting list of all Patients ");
            string getmessage = "Getting all Patients successfully";
            return await Task.FromResult(PatientLogic.GenerateResponse(patient, getmessage));
        }
        [FunctionName(HTTPFunctions.GetById)]
        public async Task<IActionResult> GetPatientsById(
           [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.GET,Route = HTTPRoutes.GetByIdRoute)]
             HttpRequestMessage req, ILogger log, string id)
        {
            try
            {
                log.LogInformation($"Getting Patient with ID: {id}");
                string requestData = await req.Content.ReadAsStringAsync();
                var item = await documentContainer.ReadItemAsync<PatientsInfo>(id, new PartitionKey(id));
                string getMessage = "Getting a particular patient successfully by Id";
                return PatientLogic.GenerateResponse(item.Resource, getMessage);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                log.LogError($"Error getting patient with ID");
                string errorMessage = "This particular id patient does not existing";
                return PatientLogic.GenerateBadResponse(errorMessage);
            }
        }
        [FunctionName(HTTPFunctions.Create)]
        public async Task<IActionResult> CreatePatient(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = HTTPRoutes.CreateRoute)] HttpRequestMessage req,
          ILogger log)
        {
            log.LogInformation("Creating a patient");
            string requestData = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PatientsInfo>(requestData);
            try
            {           
                var existingPatient = await documentContainer.ReadItemAsync<PatientsInfo>(data.Id, new PartitionKey(data.Id));
                if (existingPatient!=null) 
                {
                    string ErrorMessage = "A patient with the same ID already exists.";
                    return PatientLogic.GenerateBadResponse(ErrorMessage);
                }  
               
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(data, new ValidationContext(data), validationResults, true))
                {
                    string invalidDataMessage = validationResults.Select(v => v.ErrorMessage).FirstOrDefault();
                    return PatientLogic.GenerateBadResponse(invalidDataMessage);
                }
            }
            await documentContainer.CreateItemAsync(data, new PartitionKey(data.Id));
            string responsemessage = "Created an patient successfully";
            return PatientLogic.GenerateResponse(data, responsemessage);
        }
        [FunctionName(HTTPFunctions.Update)]
        public async Task<IActionResult> UpdatePatient(
          [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.PUT, Route = HTTPRoutes.UpdateRoute)] HttpRequestMessage req,
           ILogger log, string id)
        {
            try
            {
                string requestData = await req.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UpdatePatient>(requestData);
                PatientsInfo patient = await documentContainer.ReadItemAsync<PatientsInfo>(id, new PartitionKey(id));
                patient.Name = data.Name ?? patient.Name;
                patient.Age = data.Age ?? patient.Age;
                patient.DOB = data.DOB ?? patient.DOB;
                patient.Email = data.Email ?? patient.Email;
                patient.Phone = data.Phone ?? patient.Phone;
                await documentContainer.UpsertItemAsync(patient);
                string UpdateMessage = "Updated a particular patient sucessfully";
                return PatientLogic.GenerateResponse(patient, UpdateMessage);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string errorMessage = "This patient is not existing to update";
                return PatientLogic.GenerateBadResponse(errorMessage);
            }
        }

        [FunctionName(HTTPFunctions.Delete)]
        public async Task<IActionResult> DeletePatient(
               [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.DELETE, Route = HTTPRoutes.DeleteRoute)] HttpRequestMessage req,
               ILogger log, string id)
        {
            try
            {
                log.LogInformation($"Deleting Patient from the list with ID: {id}");
                string requestData = await req.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PatientsInfo>(requestData);
                await documentContainer.DeleteItemAsync<PatientsInfo>(id, new PartitionKey(id));
                string message = "Delete a patient successfully";
                return PatientLogic.GenerateResponse(data, message);

            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                log.LogError($"Delete patient with ID");
                string errorMessage = "The patient is not in existing to delete ";
                return PatientLogic.GenerateBadResponse(errorMessage);
            }
        }
    }
}
