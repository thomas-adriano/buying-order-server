using buying_order_server.API.v1;
using buying_order_server.Contracts;
using buying_order_server.Data.Entity;
using buying_order_server.DTO.Response;
using buying_order_server.DTO.Request;
using buying_order_server.Infrastructure.Configs;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace buying_order_server.Test.v1
{
    public class PersonsControllerTests
    {
        private readonly Mock<IAppConfigurationRepository> _mockDataManager;
        private readonly AppConfigurationController _controller;

        public PersonsControllerTests()
        {
            var logger = Mock.Of<ILogger<AppConfigurationController>>();

            var mapperProfile = new MappingProfileConfiguration();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            var mapper = new Mapper(configuration);

            _mockDataManager = new Mock<IAppConfigurationRepository>();

            _controller = new AppConfigurationController(_mockDataManager.Object, mapper, logger);
        }

        private IEnumerable<OrderNotification> GetFakePersonLists()
        {
            return new List<OrderNotification>
            {
                new OrderNotification()
                {
                    Id = 1,
                    FirstName = "Vynn Markus",
                    LastName = "Durano",
                    DateOfBirth = Convert.ToDateTime("01/15/2016")
                },
                new OrderNotification()
                {
                    Id = 2,
                    FirstName = "Vianne Maverich",
                    LastName = "Durano",
                    DateOfBirth = Convert.ToDateTime("02/15/2016")
                }
            };
        }

        private CreateOrUpdateAppConfigurationRequest FakeCreateRequestObject()
        {
            return new CreateOrUpdateAppConfigurationRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano",
                DateOfBirth = Convert.ToDateTime("02/15/2016")
            };
        }

        private UpdateAppConfigurationRequest FakeUpdateRequestObject()
        {
            return new UpdateAppConfigurationRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano",
                DateOfBirth = Convert.ToDateTime("02/15/2016")
            };
        }

        private CreateOrUpdateAppConfigurationRequest FakeCreateRequestObjectWithMissingAttribute()
        {
            return new CreateOrUpdateAppConfigurationRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano"
            };
        }

        private CreateOrUpdateAppConfigurationRequest FakeUpdateRequestObjectWithMissingAttribute()
        {
            return new CreateOrUpdateAppConfigurationRequest()
            {
                FirstName = "Vinz",
                LastName = "Durano"
            };
        }

        [Fact]
        public async Task GET_All_RETURNS_OK()
        {

            // Arrange
            _mockDataManager.Setup(manager => manager.GetAllAsync())
               .ReturnsAsync(GetFakePersonLists());

            // Act
            var result = await _controller.Get();

            // Assert
            var persons = Assert.IsType<List<AppConfigurationResponse>>(result);
            Assert.Equal(2, persons.Count);
        }

        [Fact]
        public async Task GET_ById_RETURNS_OK()
        {
            long id = 1;

            _mockDataManager.Setup(manager => manager.GetByIdAsync(id))
               .ReturnsAsync(GetFakePersonLists().Single(p => p.Id.Equals(id)));

            var person = await _controller.Get(id);
            Assert.IsType<AppConfigurationResponse>(person);
        }

        [Fact]
        public async Task GET_ById_RETURNS_NOTFOUND()
        {
            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Get(10));
            Assert.Equal(404, apiException.StatusCode);
        }

        [Fact]
        public async Task POST_Create_RETURNS_BADREQUEST()
        {
            _controller.ModelState.AddModelError("DateOfBirth", "Required");

            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Post(FakeCreateRequestObjectWithMissingAttribute()));
            Assert.Equal(422, apiException.StatusCode);
        }

        [Fact]
        public async Task POST_Create_RETURNS_OK()
        {

            _mockDataManager.Setup(manager => manager.CreateAsync(It.IsAny<OrderNotification>()))
                .ReturnsAsync(It.IsAny<long>());

            var person = await _controller.Post(FakeCreateRequestObject());

            var response = Assert.IsType<ApiResponse>(person);
            Assert.Equal(201, response.StatusCode);
        }

        [Fact]
        public async Task POST_Create_RETURNS_SERVERERROR()
        {
            _mockDataManager.Setup(manager => manager.CreateAsync(It.IsAny<OrderNotification>()))
                .Throws(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _controller.Post(FakeCreateRequestObject()));
        }

        [Fact]
        public async Task PUT_ById_RETURNS_OK()
        {
            _mockDataManager.Setup(manager => manager.UpdateAsync(It.IsAny<OrderNotification>()))
                 .ReturnsAsync(true);

            var person = await _controller.Put(1, FakeUpdateRequestObject());

            var response = Assert.IsType<ApiResponse>(person);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task PUT_ById_RETURNS_NOTFOUND()
        {
            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Put(10, FakeUpdateRequestObject()));
            Assert.Equal(404, apiException.StatusCode);
        }

        [Fact]
        public async Task PUT_ById_RETURNS_BADREQUEST()
        {
            _controller.ModelState.AddModelError("DateOfBirth", "Required");

            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Put(10, FakeUpdateRequestObject()));
            Assert.Equal(422, apiException.StatusCode);
        }

        [Fact]
        public async Task PUT_ById_RETURNS_SERVERERROR()
        {

            _mockDataManager.Setup(manager => manager.UpdateAsync(It.IsAny<OrderNotification>()))
                .Throws(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _controller.Put(10, FakeUpdateRequestObject()));
        }

        [Fact]
        public async Task DELETE_ById_RETURNS_OK()
        {
            long id = 1;

            _mockDataManager.Setup(manager => manager.DeleteAsync(id))
                 .ReturnsAsync(true);

            var result = await _controller.Delete(id);

            var response = Assert.IsType<ApiResponse>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_ById_RETURNS_NOTFOUND()
        {
            var apiException = await Assert.ThrowsAsync<ApiProblemDetailsException>(() => _controller.Delete(1));
            Assert.Equal(404, apiException.StatusCode);
        }

        [Fact]
        public async Task DELETE_ById_RETURNS_SERVERERROR()
        {
            long id = 1;

            _mockDataManager.Setup(manager => manager.DeleteAsync(id))
                .Throws(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _controller.Delete(id));
        }
    }
}
