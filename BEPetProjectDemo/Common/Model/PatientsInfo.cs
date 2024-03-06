using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BEPetProjectDemo.Common.Model
{
    public class PatientsInfo
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name cannot be longer than 100 characters, Not less than 3 characters ")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name should contain alphabets only")]
        public string Name { get; set; }
        [JsonProperty("Age")]


        public string Age { get; set; }
        [JsonProperty("DOB")]
        public string DOB { get; set; }
        [JsonProperty("Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [JsonProperty("MobileNumber")]
        [Required(ErrorMessage = "Mobile Number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number should have 10 digits")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Mobile number should contain numeric digits only")]
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
    }
}
