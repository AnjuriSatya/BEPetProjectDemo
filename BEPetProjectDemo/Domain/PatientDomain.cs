using BEPetProjectDemo.Common.Model;
using BEPetProjectDemo.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BEPetProjectDemo.Domain
{
    public class PatientDomain:IPatientDomain
    {
        private readonly IPatientDAL _patientDAL;
        public PatientDomain(IPatientDAL patientDAL)
        {
            _patientDAL = patientDAL;
        }
        public async Task<IActionResult> GetallPatients(HttpRequestMessage req)
        {
         return await _patientDAL.GetallPatients(req);
        }
        public async Task<IActionResult> GetPatientsById(HttpRequestMessage req, string id)
        {
            return await _patientDAL.GetPatientsById(req, id);
        }

        public async Task<IActionResult> CreatePatient(PatientsInfo data)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(data, new ValidationContext(data), validationResults, true))
            {
                string invalidDataMessage = validationResults.Select(v => v.ErrorMessage).FirstOrDefault();
                return PatientLogic.CreateBadResponse(invalidDataMessage);
            }
            return await _patientDAL.CreatePatient(data);
        }
        public async Task<IActionResult> UpdatePatient(PatientsInfo data, string id)
        {
            return await _patientDAL.UpdatePatient(data, id);
        }
        public async Task<IActionResult> DeletePatient(HttpRequestMessage req, string id)
        {
            return await _patientDAL.DeletePatient(req, id);
        }
    }
}

