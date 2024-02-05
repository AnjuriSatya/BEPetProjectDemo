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
using BEPetProjectDemo.Domain;
using System.Security.Policy;
using System.Net;

namespace BEPetProjectDemo
{
    public class Httptriggers
    {
        private const string DatabaseName = "PatientsDetails";
        private const string CollectionName = "Patients";
        private readonly CosmosClient _cosmosClient;
        private readonly Container documentContainer;
        private readonly IPatientDomain _patientDomain;
        public Httptriggers(CosmosClient cosmosClient,IPatientDomain patientDomain)
        {
            _cosmosClient = cosmosClient;
            documentContainer = _cosmosClient.GetContainer("PatientsDetails", "Patients");
            _patientDomain = patientDomain;
        }
        [FunctionName(HTTPFunctions.Get)]
        public async Task<IActionResult> GetallPatients(
                [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.GET, Route = HTTPRoutes.GetRoute)] HttpRequestMessage req,
                [CosmosDB(
                DatabaseName,
                CollectionName,
                Connection ="CosmosDBConnectionString")]
                IEnumerable<PatientsInfo> patient,
                ILogger log)
            
        {
            log.LogInformation("Getting list of all Patients ");
            return await _patientDomain.GetallPatients(req, patient);
        }

        [FunctionName(HTTPFunctions.GetById)]
        public async Task<IActionResult> GetPatientsById(
           [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.GET,Route = HTTPRoutes.GetByIdRoute)]
            HttpRequestMessage req, ILogger log, string id)
        {
            log.LogInformation($"Getting Patient with ID: {id}");
            try
            {
                return await _patientDomain.GetPatientsById(req, id,documentContainer);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                log.LogError($"Error getting patient with ID");
                string errorMessage = "This particular id patient does not existing";
                return PatientLogic.CreateBadResponse(errorMessage);
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
                return await _patientDomain.CreatePatient(data, documentContainer);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                string errorMessage = "Failed to create a patient";
                return PatientLogic.CreateBadResponse(errorMessage);
            }
        }
        [FunctionName(HTTPFunctions.Update)]
        public async Task<IActionResult> UpdatePatient(
          [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.PUT, Route = HTTPRoutes.UpdateRoute)] HttpRequestMessage req,
           ILogger log, string id)
        {
            log.LogInformation($"Update a Patient in the list with ID: {id}");
            string requestData = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PatientsInfo>(requestData);
            try
            {
                return await _patientDomain.UpdatePatient(data, id, documentContainer);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string errorMessage = "This patient is not existing to update";
                return PatientLogic.CreateBadResponse(errorMessage);
            }
        }
       [FunctionName(HTTPFunctions.Delete)]
        public async Task<IActionResult> DeletePatient(
               [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.DELETE, Route = HTTPRoutes.DeleteRoute)]HttpRequestMessage req,
               ILogger log, string id)
        {
            log.LogInformation($"Deleting Patient from the list with ID: {id}");
            try
            {
                return await _patientDomain.DeletePatient(req, id, documentContainer);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string errorMessage = "The patient is not in existing to delete ";
                return PatientLogic.CreateBadResponse(errorMessage);
            }
        }
    }
}
