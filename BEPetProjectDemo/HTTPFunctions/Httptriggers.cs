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
using BEPetProjectDemo.Common.Model;
using BEPetProjectDemo.Common;
namespace BEPetProjectDemo
{
    public class Httptriggers
    {
        private readonly IPatientDomain _patientDomain;
        public Httptriggers(IPatientDomain patientDomain)
        {
            _patientDomain = patientDomain;
        }
        [FunctionName(HTTPFunctions.Get)]
        public async Task<IActionResult> GetallPatients(
                [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.GET, Route = HTTPRoutes.GetRoute)] HttpRequestMessage req,
                ILogger log)

        {
            log.LogInformation("Getting list of all Patients ");
            return await _patientDomain.GetallPatients(req);
        }

        [FunctionName(HTTPFunctions.GetById)]
        public async Task<IActionResult> GetPatientsById(
           [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.GET,Route = HTTPRoutes.GetByIdRoute)]
            HttpRequestMessage req, ILogger log, string id)
        {
            log.LogInformation($"Getting Patient with ID: {id}");
            try
            {
                return await _patientDomain.GetPatientsById(req, id);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                log.LogError($"Error getting patient with ID");
                return PatientLogic.CreateBadResponse(Constants.GetByIdFailed);
            }
        }

        [FunctionName(HTTPFunctions.Create)]
        public async Task<IActionResult> CreatePatient(
         [HttpTrigger(AuthorizationLevel.Anonymous, HTTPMethods.POST, Route = HTTPRoutes.CreateRoute)] HttpRequestMessage req,
          ILogger log)
        {
            log.LogInformation("Creating a patient");
            string requestData = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PatientsInfo>(requestData);

            try
            {
                return await _patientDomain.CreatePatient(data);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return PatientLogic.CreateBadResponse(Constants.CreateFailed);
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
                return await _patientDomain.UpdatePatient(data, id);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string errorMessage = Constants.FailedMessage +"Update";
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
                return await _patientDomain.DeletePatient(req, id);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string errorMessage = Constants.FailedMessage + "Delete";
                return PatientLogic.CreateBadResponse(errorMessage);
            }
        }
    }
}
