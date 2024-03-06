using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System;
using BEPetProjectDemo.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BEPetProjectDemo.Common.Model;
using BEPetProjectDemo.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos.Linq;

namespace BEPetProjectDemo.DAL
{
    public class PatientDAL : IPatientDAL
    {
        private const string DatabaseName = "PatientsDetails";
        private const string CollectionName = "Patients";
        private const string Connection = "CosmosDBConnectionString";
        private readonly CosmosClient _cosmosClient;
        private readonly Container documentContainer;
        public PatientDAL(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            documentContainer = _cosmosClient.GetContainer(DatabaseName, CollectionName);
        }
        public async Task<IActionResult> GetallPatients(HttpRequestMessage req)
        {
            var patientsQuery = documentContainer.GetItemLinqQueryable<PatientsInfo>();
            var patients = new List<PatientsInfo>();
            var iterator = patientsQuery.ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                var response =await iterator.ReadNextAsync();
                patients.AddRange(response);
            }

            var getmessage = Constants.GetMessage;
            return  PatientLogic.CreateResponse(patients, getmessage);
        }
        public async Task<IActionResult> GetPatientsById(HttpRequestMessage req, string id)
        {
            var GetPatient = await documentContainer.ReadItemAsync<PatientsInfo>(id, new PartitionKey(id));
            var getMessage = Constants.GetById;
            return PatientLogic.CreateResponse(GetPatient.Resource, getMessage);
        }
        public async Task<IActionResult> CreatePatient(PatientsInfo data)
        {
            try
            {
                var existingPatient = await documentContainer.ReadItemAsync<PatientsInfo>(data.Id, new PartitionKey(data.Id));
                var errorMessage = Constants.ExistingPatient;
                 return PatientLogic.CreateBadResponse(errorMessage); 
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var createResponse = await documentContainer.CreateItemAsync(data, new PartitionKey(data.Id));
                string responseMessage = "Created" + Constants.PatientSucessfully;
                return PatientLogic.CreateResponse(data, responseMessage);
            }
        }
        public async Task<IActionResult> UpdatePatient(PatientsInfo data, string id)
       {
            PatientsInfo UpdPatient = await documentContainer.ReadItemAsync<PatientsInfo>(id, new PartitionKey(id));
            if (data.MobileNumber != null)
            {
                UpdPatient.MobileNumber = data.MobileNumber;
            }
            if (data.Name != null)
            {
                UpdPatient.Name = data.Name;
            }
            await documentContainer.UpsertItemAsync(UpdPatient);
            string UpdateMessage = "Updated"+ Constants.PatientSucessfully;
            return PatientLogic.CreateResponse(UpdPatient, UpdateMessage);
        }
        
        public async Task<IActionResult> DeletePatient(HttpRequestMessage req, string id)
        {
            await documentContainer.DeleteItemAsync<PatientsInfo>(id, new PartitionKey(id));
             string  message = "Delete" + Constants.PatientSucessfully;
            return PatientLogic.CreateResponse(null,message);
        }
    }
}
