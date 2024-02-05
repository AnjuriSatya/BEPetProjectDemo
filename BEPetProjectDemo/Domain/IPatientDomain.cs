using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BEPetProjectDemo.Domain
{
    public interface IPatientDomain
    {
        Task<IActionResult> GetallPatients(HttpRequestMessage req, IEnumerable<PatientsInfo> patient);
        Task<IActionResult> GetPatientsById(HttpRequestMessage req, string id, Microsoft.Azure.Cosmos.Container documentContainer);
        Task<IActionResult> CreatePatient(PatientsInfo data, Microsoft.Azure.Cosmos.Container documentContainer);
        Task<IActionResult> UpdatePatient(PatientsInfo data, string id, Microsoft.Azure.Cosmos.Container documentContainer);
        Task<IActionResult> DeletePatient(HttpRequestMessage req, string id, Microsoft.Azure.Cosmos.Container documentContainer);
    }
}
