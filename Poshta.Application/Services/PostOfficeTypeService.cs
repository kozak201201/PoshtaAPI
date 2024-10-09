using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.Application.Services
{
    public class PostOfficeTypeService : IPostOfficeTypeService
    {
        private IPostOfficeTypesRepository postOfficeTypesRepository;
        private ILogger<PostOfficeTypeService> logger;

        public PostOfficeTypeService(
            IPostOfficeTypesRepository postOfficeTypesRepository,
            ILogger<PostOfficeTypeService> logger)
        {
            this.postOfficeTypesRepository = postOfficeTypesRepository;
            this.logger = logger;
        }

        public async Task<Result<PostOfficeType>> CreateAsync(
            string name,
            float maxShipmentWeight,
            float maxShipmentLength,
            float maxShipmentWidth,
            float maxShipmentHeight)
        {
            logger.LogInformation("Start create post office type");

            var id = Guid.Empty;

            var postOfficeTypeResult = PostOfficeType.Create(
                id,
                name,
                maxShipmentWeight,
                maxShipmentLength,
                maxShipmentWidth,
                maxShipmentHeight);

            if (postOfficeTypeResult.IsFailure)
            {
                logger.LogError(postOfficeTypeResult.Error);
                return Result.Failure<PostOfficeType>(postOfficeTypeResult.Error);
            }

            var postOfficeType = postOfficeTypeResult.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType);

            logger.LogInformation($"Success create post office type. Id: {id}");
            return Result.Success(postOfficeType);
        }

        public async Task<Result<PostOfficeType>> GetByIdAsync(Guid postOfficeTypeId)
        {
            logger.LogInformation($"Start get post office type by id: {postOfficeTypeId}");

            var postOfficeType = await postOfficeTypesRepository.GetPostOfficeTypeByIdAsync(postOfficeTypeId);

            if (postOfficeType == null)
            {
                logger.LogError($"Post office type with id: {postOfficeTypeId} wasn't found");
                return Result.Failure<PostOfficeType>($"Post office type with id: {postOfficeTypeId} wasn't found");
            }

            logger.LogInformation($"Success get post office type by id: {postOfficeTypeId}");
            return Result.Success(postOfficeType);
        }

        public async Task<IEnumerable<PostOfficeType>> GetAllAsync()
        {
            logger.LogInformation($"Start get all post offices");

            var postOfficeTypes = await postOfficeTypesRepository.GetPostOfficeTypesAsync();

            logger.LogInformation($"Success get all post office types");
            return postOfficeTypes;
        }

        public async Task<Result<string>> DeleteByIdAsync(Guid postOfficeTypeId)
        {
            logger.LogInformation($"Start delete post office type with id: {postOfficeTypeId}");

            await postOfficeTypesRepository.DeleteAsync(postOfficeTypeId);

            logger.LogInformation($"Success delete post office type with id: {postOfficeTypeId}");
            return Result.Success($"Success delete post office type with id: {postOfficeTypeId}");
        }
    }
}
