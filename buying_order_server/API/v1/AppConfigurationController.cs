﻿using buying_order_server.Contracts;
using buying_order_server.Data.Entity;
using buying_order_server.DTO.Request;
using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.AspNetCore.Authorization;

namespace buying_order_server.API.v1
{
    [Route("api/v1/app-configuration")]
    [ApiController]
    public class AppConfigurationController : ControllerBase
    {
        private readonly ILogger<AppConfigurationController> _logger;
        private readonly IAppConfigurationRepository _appConfigurationRepository;
        private readonly IMapper _mapper;

        public AppConfigurationController(IAppConfigurationRepository orderRepo, IMapper mapper, ILogger<AppConfigurationController> logger)
        {
            _appConfigurationRepository = orderRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AppConfigurationDTO), Status200OK)]
        public async Task<AppConfigurationDTO> Get()
        {
            var data = await _appConfigurationRepository.GetLastAsync();
            var config = _mapper.Map<AppConfigurationDTO>(data);
            _logger.LogDebug(config.ToString());
            return config;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> Post([FromBody] AppConfigurationDTO createRequest)
        {
            if (!ModelState.IsValid) { throw new ApiProblemDetailsException(ModelState); }

            var config = _mapper.Map<AppConfigurationEntity>(createRequest);
            return new ApiResponse("Record successfully created.", await _appConfigurationRepository.CreateOrUpdateAsync(config), Status201Created);
        }
    }
}