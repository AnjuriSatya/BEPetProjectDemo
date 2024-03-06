using Moq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using BEPetProjectDemo.Common;
using BEPetProjectDemo.DAL;
using BEPetProjectDemo.Domain;
using BEPetProjectDemo.Common.Model;
using System.Reflection.Metadata;
using System.ComponentModel;
using Microsoft.Azure.Cosmos;
using BEPetProjectDemo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Azure;

namespace TestProject1
{
    public class UnitTestCases
    {
        private Mock<Microsoft.Azure.Cosmos.Container> _documentcontainerMock;
        private Mock<CosmosClient> _cosmosclientMock;
        private Mock<IPatientDAL> _patientDalMock;
        private Mock<IPatientDomain> _patientDomainMock;
        private Mock<PatientLogic> _patientLogicMock;
        private Mock<ILogger> _loggerMock;

        public void TestSetup()
        {
            this._documentcontainerMock = new Mock<Microsoft.Azure.Cosmos.Container>();
            this._cosmosclientMock = new Mock<CosmosClient>();
            this._patientDalMock = new Mock<IPatientDAL>();
            this._patientDomainMock = new Mock<IPatientDomain>();
            this._patientLogicMock = new Mock<PatientLogic>();
            this._loggerMock = new Mock<ILogger>();
        }

        [Theory]
        [ClassData(typeof(patientSuceessInfo))]
        public async Task GetPatientById_Returns_SuccessOkResult(PatientsInfo mockInfo)
        {
            //Arrange
            var req = CreateMockRequest(mockInfo);
            TestSetup();
            var mock_id = "2";
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(x => x.GetPatientsById(It.IsAny<HttpRequestMessage>(), It.IsAny<string>()))
               .Returns(Task.FromResult<IActionResult>(new OkObjectResult(mockInfo) { StatusCode = StatusCodes.Status200OK }));
            //Act
            dynamic response = await patientDomain.GetPatientsById(req, mock_id);
            //Assert
            var okObjectResult = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(mockInfo, okObjectResult?.Value);
        }

        [Theory]
        [ClassData(typeof(PatientByIdFailedInfo))]
        public async Task GetPatientById_Returns_BadRequestObjectResult(PatientsInfo mockInfo)
        {
            //Arrange
            var req = CreateMockRequest(mockInfo);
            TestSetup();
            var mock_id = "34";
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(f => f.GetPatientsById(It.IsAny<HttpRequestMessage>(), It.IsAny<string>()))
                .Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(mockInfo) { StatusCode = StatusCodes.Status400BadRequest }));
            //Act
            dynamic response = await patientDomain.GetPatientsById(req, mock_id);
            //Assert
            var badRequestObjectResult = response as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(mockInfo, badRequestObjectResult?.Value);
        }

        [Theory]
        [ClassData(typeof(patientSuceessInfo))]
        public async Task DeletePatient_Returns_SuccessOkResult(PatientsInfo mockInfo)
        {
            //Arrange
            var req = CreateMockRequest(mockInfo);
            TestSetup();
            var mock_id = "1";
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(d => d.DeletePatient(It.IsAny<HttpRequestMessage>(), It.IsAny<string>())).
                Returns(Task.FromResult<IActionResult>(new OkObjectResult(mockInfo) { StatusCode = StatusCodes.Status200OK }));
            //Act
            dynamic response = await patientDomain.DeletePatient(req, mock_id);
            //Assert
            var okObjectResult = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(mockInfo, okObjectResult?.Value);
        }

        [Theory]
        [ClassData(typeof(PatientByIdFailedInfo))]
        public async Task DeletePatient_Returns_BadRequestObjectResult(PatientsInfo mockInfo)
        {
            //Arrange
            var req = CreateMockRequest(mockInfo);
            TestSetup();
            var mock_id = "30";
            var PatientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(d => d.DeletePatient(It.IsAny<HttpRequestMessage>(), It.IsAny<string>()))
                .Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(mockInfo) { StatusCode = StatusCodes.Status400BadRequest }));
            //Act
            dynamic response = await PatientDomain.DeletePatient(req, mock_id);
            //Assert
            var badRequestObjectResult = response as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(mockInfo, badRequestObjectResult?.Value);
        }

        //[Theory]
        //[ClassData(typeof(GetAllPatientsData))]
        //public async Task getAllPatients_Returns_SuccessOkResult(List<PatientsInfo> expectedPatients)
        //{
        //    //Arrange
        //    var req = CreateMockRequest(expectedPatients);
        //    TestSetup();
        //    var patientdomain = new PatientDomain(_patientDalMock.Object);
        //    _patientDalMock.Setup(g => g.GetallPatients(It.IsAny<HttpRequestMessage>()))
        //        .Returns(Task.FromResult<IActionResult>(new OkObjectResult(expectedPatients) { StatusCode = StatusCodes.Status200OK }));
        //    //Act
        //    dynamic response = await patientdomain.GetallPatients(req);
        //    //Assert
        //    var okObjetResult = response as OkObjectResult;
        //    Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        //    Assert.Equal(expectedPatients, okObjetResult?.Value);
        //}

        [Theory]
        [ClassData(typeof(patientSuceessInfo))]
        public async Task CreatePatient_Returns_SuccessOkResult(PatientsInfo newPatient)

        {
            //Arrange
            TestSetup();
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(c => c.CreatePatient(It.IsAny<PatientsInfo>()))
                .Returns(Task.FromResult<IActionResult>(new OkObjectResult(newPatient) { StatusCode = StatusCodes.Status200OK }));
            //Act
            dynamic response = await patientDomain.CreatePatient(newPatient);

            //Assert
            var okObjectResult = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(newPatient, okObjectResult?.Value);
        }

        [Theory]
        [ClassData(typeof(InValidPatientData))]
        public async Task CreatePatient_Returns_BadRequestObjectResult(PatientsInfo inValidInfo)
        {

            //Arrange
            TestSetup();
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(inv => inv.CreatePatient(It.IsAny<PatientsInfo>()))
                .Returns(Task.FromResult<IActionResult>(new BadRequestResult()));
            //Act
            dynamic response = await patientDomain.CreatePatient(inValidInfo);
            //Assert
            var badRequestObjectResult = response as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Theory]
        [ClassData(typeof(patientSuceessInfo))]
        public async Task UpdatePatient_Returns_SuccessOktResult(PatientsInfo updateInfo)
        {
            //Arrange
            TestSetup();
            var mock_id = "1";
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(upd => upd.UpdatePatient(It.IsAny<PatientsInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult<IActionResult>(new OkObjectResult(updateInfo) { StatusCode = StatusCodes.Status200OK }));
            //Act
            dynamic response = await patientDomain.UpdatePatient(updateInfo, mock_id);
            //Assert
            var okObjectResult = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            //Assert.Equal(updateInfo, okObjectResult?.Value);
        }

        [Theory]
        [ClassData(typeof(InValidPatientData))]
        public async Task UpdatePatient_Returns_BadRequestObjectResult_InValid(PatientsInfo updateInfo)
        {
            //Arrange
            TestSetup();
            var mock_id = "1";
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(upd => upd.UpdatePatient(It.IsAny<PatientsInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(updateInfo) { StatusCode = StatusCodes.Status400BadRequest }));
            //Act
            dynamic response = await patientDomain.UpdatePatient(updateInfo, mock_id);
            //Assert
            var badRequestObjectResult = response as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Theory]
        [ClassData(typeof(PatientByIdFailedInfo))]
        public async Task UpdatePatient_Returns_BadRequestObjectResult(PatientsInfo updateInfo)
        {
            //Arrange
            TestSetup();
            var mock_id = "100";
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(upd => upd.UpdatePatient(It.IsAny<PatientsInfo>(), It.IsAny<string>()))
                .Returns(Task.FromResult<IActionResult>(new BadRequestObjectResult(updateInfo) { StatusCode = StatusCodes.Status400BadRequest }));
            //Act
            dynamic response = await patientDomain.UpdatePatient(updateInfo, mock_id);
            //Assert
            var badRequestObjectResult = response as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        public class patientSuceessInfo : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {   
                yield return new object[] { new PatientsInfo { Id = "1",Name = "satya", DOB = "2001-03-02", Age = "22", MobileNumber = "9701643710", Email = "satya23@gmail.com", Gender = "F" } };

            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class PatientByIdFailedInfo : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new PatientsInfo { Id = string.Empty } };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class InValidPatientData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new PatientsInfo { Id = string.Empty, Name = string.Empty, DOB = string.Empty, Age = string.Empty, MobileNumber = string.Empty, Email = string.Empty, Gender = string.Empty } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "Divya123", DOB = "2001-03-02", Age = "22", MobileNumber = "9701643710", Email = "divya23@gmail.com", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "di", DOB = "2000-03-02", Age = "22", MobileNumber = "9702643710", Email = "divya23@123", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = string.Empty, DOB = "2000-03-02", Age = "22", MobileNumber = "9702643710", Email = "divya23@123", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "divya", DOB = "2000-03-02", Age = "22", MobileNumber = "9701", Email = "divya23@123", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "divya", DOB = "2000-03-02", Age = "22", MobileNumber = "97026437101111", Email = "divya23@123", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "divya", DOB = "2000-03-02", Age = "22", MobileNumber = string.Empty, Email = "divya23@123", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "divya", DOB = "2000-03-02", Age = "22", MobileNumber = "9702643710", Email = "divya", Gender = "F" } };
                yield return new object[] { new PatientsInfo { Id = "1", Name = "di", DOB = "2000-03-02", Age = "22", MobileNumber = "9702643710", Email = string.Empty, Gender = "F" } };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class GetAllPatientsData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new List <PatientsInfo>{ new PatientsInfo {Id = "1", Name = "Satya", DOB = "2001-03-02",
                                                                          Age = "22", MobileNumber = "9701643710", Email = "satya23@gmail.com", Gender = "F" },
                 new PatientsInfo  { Id = "2", Name = "Swathi", DOB = "2000-09-12", Age = "23", MobileNumber = "9908908739", Email = "swathi@gmail.com", Gender = "F" } } };

            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        public static HttpRequestMessage CreateMockRequest(object body)
        {
            var req = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"),
            };
            req = SetupHttp(req);
            return req;
        }

        public static HttpRequestMessage SetupHttp(HttpRequestMessage requestMessage)
        {
            var services = new Mock<IServiceProvider>(MockBehavior.Strict);
            var formatter = new XmlMediaTypeFormatter();
            var context = new DefaultHttpContext();
            var contentNegotiator = new Mock<IContentNegotiator>();
            contentNegotiator
                .Setup(c => c.Negotiate(It.IsAny<Type>(), It.IsAny<HttpRequestMessage>(), It.IsAny<IEnumerable<MediaTypeFormatter>>()))
                .Returns(new ContentNegotiationResult(formatter, mediaType: null));
            var options = new WebApiCompatShimOptions();
            if (formatter == null)

            {
                options.Formatters.AddRange(new MediaTypeFormatterCollection());
            }
            else  
            {
                options.Formatters.Add(formatter);
            }
            var optionsAccessor = new Mock<IOptions<WebApiCompatShimOptions>>();
            optionsAccessor.SetupGet(o => o.Value).Returns(options);
            services.Setup(s => s.GetService(typeof(IOptions<WebApiCompatShimOptions>))).Returns(optionsAccessor.Object);
            if (contentNegotiator != null)
            {
                services.Setup(s => s.GetService(typeof(IContentNegotiator))).Returns(contentNegotiator);
            }
            context.RequestServices = CreateServices(contentNegotiator.Object, formatter);
            requestMessage.Options.TryAdd(nameof(HttpContext), context);
            return requestMessage;
        }
        private static IServiceProvider CreateServices(IContentNegotiator contentNegotiator = null, MediaTypeFormatter formatter = null)
        {
            var options = new WebApiCompatShimOptions();
            if (formatter == null)
            {
                options.Formatters.AddRange(new MediaTypeFormatterCollection());
            }
            else
            {
                options.Formatters.Add(formatter);
            }
            var optionsAccessor = new Mock<IOptions<WebApiCompatShimOptions>>();
            optionsAccessor.SetupGet(o => o.Value).Returns(options);
            var services = new Mock<IServiceProvider>(MockBehavior.Strict);
            services.Setup(s => s.GetService(typeof(IOptions<WebApiCompatShimOptions>))).Returns(optionsAccessor.Object);
            if (contentNegotiator != null)
            {
                services.Setup(s => s.GetService(typeof(IContentNegotiator))).Returns(contentNegotiator);
            }
            return services.Object;
        }

        //[Fact]
        //public async Task GetById_Return200Ok()
        //{
        //    TestSetup();
        //    var context = new Mock<HttpContext>();
        //    var req = new Mock<HttpRequestMessage>();
        //    var id = "1";

        //    //cosmosClientMock.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(documentContainerMock.Object);
        //    var patientDomain = new PatientDomain(_patientDalMock.Object);
        //    var httptriggers = new Httptriggers(patientDomain);
        //    //documentcontainer.Setup(x=>x.ReadItemAsync(id,))
        //    _patientDomainMock.Setup(x => x.GetPatientsById(req.Object, id)).Returns(Task.FromResult(It.IsAny<IActionResult>()));
        //    _patientDalMock.Setup(x => x.GetPatientsById(req.Object, id)).Returns(Task.FromResult(It.IsAny<IActionResult>()));
        //    dynamic response = httptriggers.GetPatientsById(req.Object, _loggerMock.Object, id);
        //    Assert.Equal(true, response.value.success);

        //}
        [Fact]
        public async Task GetAllPatients_ReturnsSuccessOk()
        {
            TestSetup();
            var expectedPatients = new List<PatientsInfo> { new PatientsInfo { Id = "1",Name = "satya", DOB = "2000-09-12", Age = "23", MobileNumber = "9908908739", Email = "satrya23@gmail.com", Gender = "F" },
                      new PatientsInfo { Id = "2", Name = "Swathi", DOB = "2000-09-12", Age = "23", MobileNumber = "9908908739", Email = "swathi@gmail.com", Gender = "F" } };
            var httpRequestMessage = new Mock<HttpRequestMessage>();
            var patientDomain = new PatientDomain(_patientDalMock.Object);
            _patientDalMock.Setup(get => get.GetallPatients(It.IsAny<HttpRequestMessage>()))
              .Returns(Task.FromResult<IActionResult>(new OkObjectResult(expectedPatients) { StatusCode = StatusCodes.Status200OK }));
            dynamic response = await patientDomain.GetallPatients(httpRequestMessage.Object);
            //Assert
            var okObjectResult = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(expectedPatients, okObjectResult?.Value);

        }

    }
}