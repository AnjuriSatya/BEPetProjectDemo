using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System;
using BEPetProjectDemo.Domain;
using Microsoft.Extensions.Configuration;

namespace BEPetProjectDemo.DAL
{
    public class PatientDAL : IPatientDAL
    { 
        public async Task<IActionResult> GetallPatients(HttpRequestMessage req, IEnumerable<PatientsInfo> patient)
        {
            string getmessage = "Getting all Patients successfully";
            return await Task.FromResult(PatientLogic.CreateResponse(patient, getmessage));
        }
        public async Task<IActionResult> GetPatientsById(HttpRequestMessage req, string id, Microsoft.Azure.Cosmos.Container documentContainer)
        {
            var GetPatient = await documentContainer.ReadItemAsync<PatientsInfo>(id, new PartitionKey(id));
            string getMessage = "Getting a particular patient successfully by Id";
            return PatientLogic.CreateResponse(GetPatient.Resource, getMessage);
        }
        public async Task<IActionResult> CreatePatient(PatientsInfo data, Microsoft.Azure.Cosmos.Container documentContainer)
        {
            try
            {
                var existingPatient = await documentContainer.ReadItemAsync<PatientsInfo>(data.Id, new PartitionKey(data.Id));
                 string errorMessage = "A patient with the same ID already exists.";
                 return PatientLogic.CreateBadResponse(errorMessage); 
            }

            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var createResponse = await documentContainer.CreateItemAsync(data, new PartitionKey(data.Id));
                string responseMessage = "Created a patient successfully";
                return PatientLogic.CreateResponse(data, responseMessage);
            }
        }
        public async Task<IActionResult> UpdatePatient(PatientsInfo UpdPatient, string id, Microsoft.Azure.Cosmos.Container documentContainer)
        {
            await documentContainer.UpsertItemAsync(UpdPatient);
            string UpdateMessage = "Updated a particular patient sucessfully";
            return PatientLogic.CreateResponse(UpdPatient, UpdateMessage);
        }

        public async Task<IActionResult> DeletePatient(HttpRequestMessage req, string id, Microsoft.Azure.Cosmos.Container documentContainer)
        {
            await documentContainer.DeleteItemAsync<PatientsInfo>(id, new PartitionKey(id));
            string message = "Delete a patient successfully";
            return PatientLogic.CreateResponse(null,message);
        }
    }
}
