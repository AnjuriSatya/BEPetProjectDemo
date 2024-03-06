﻿using BEPetProjectDemo.Common.Model;
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
        Task<IActionResult> GetallPatients(HttpRequestMessage req);
        Task<IActionResult> GetPatientsById(HttpRequestMessage req, string id);
        Task<IActionResult> CreatePatient(PatientsInfo data);
        Task<IActionResult> UpdatePatient(PatientsInfo data, string id);
        Task<IActionResult> DeletePatient(HttpRequestMessage req, string id);
    }
}
