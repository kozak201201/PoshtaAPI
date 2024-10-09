using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Poshta.Core.Interfaces.Repositories;
using Poshta.Core.Interfaces.Services;
using Poshta.Core.Models;

namespace Poshta.Application.Services
{
    public class PostOfficeService : IPostOfficeService
    {
        private readonly IPostOfficeTypeService postOfficeTypeService;
        private readonly IPostOfficesRepository postOfficesRepository;
        private readonly ILogger<PostOfficeService> logger;

        public PostOfficeService(
            IPostOfficeTypeService postOfficeTypeService,
            IPostOfficesRepository postOfficesRepository,
            ILogger<PostOfficeService> logger)
        {
            this.postOfficeTypeService = postOfficeTypeService;
            this.postOfficesRepository = postOfficesRepository;
            this.logger = logger;
        }

        public async Task<Result<PostOffice>> CreateAsync(
            int number,
            string city,
            string address,
            int maxShipmentsCount,
            double latitude,
            double longitude,
            Guid postOfficeTypeId)
        {
            logger.LogInformation($"Start create post office");

            var postOfficeTypeResult = await postOfficeTypeService.GetByIdAsync(postOfficeTypeId);

            if (postOfficeTypeResult.IsFailure)
            {
                logger.LogError(postOfficeTypeResult.Error);
                return Result.Failure<PostOffice>(postOfficeTypeResult.Error);
            }

            var postOfficeType = postOfficeTypeResult.Value;

            var postOfficeResult = PostOffice.Create(
                Guid.NewGuid(),
                number,
                city,
                address,
                maxShipmentsCount,
                latitude,
                longitude,
                postOfficeType);

            if (postOfficeResult.IsFailure)
            {
                logger.LogError(postOfficeResult.Error);
                return Result.Failure<PostOffice>(postOfficeResult.Error);
            }

            var postOffice = postOfficeResult.Value;

            await postOfficesRepository.CreateAsync(postOffice);

            logger.LogInformation($"Success create post office {postOffice}");
            return Result.Success(postOffice);
        }

        public async Task<Result<PostOffice>> GetPostOfficeByIdAsync(Guid postOfficeId)
        {
            logger.LogInformation($"Start get post office with id {postOfficeId}");
            var postOffice = await postOfficesRepository.GetPostOfficeByIdAsync(postOfficeId);

            if (postOffice == null)
            {
                logger.LogError($"post office with id: {postOfficeId} wasn't found");
                return Result.Failure<PostOffice>($"Post office with id: {postOfficeId} wasn't found");
            }

            logger.LogInformation($"Success get post office with id {postOffice}");
            return postOffice;
        }

        public async Task<IEnumerable<PostOffice>> GetPostOfficesAsync()
        {
            logger.LogInformation($"Start get post offices");
            IEnumerable<PostOffice> postOffices = await postOfficesRepository.GetPostOfficesAsync();

            logger.LogInformation($"Success get post offices");
            return postOffices;
        }

        public async Task<Result> DeleteAsync(Guid postOfficeId)
        {
            logger.LogInformation($"Start delete post office with id: {postOfficeId}");

            await postOfficesRepository.DeleteAsync(postOfficeId);

            logger.LogInformation($"Success delete post office with id: {postOfficeId}");
            return Result.Success();
        }
    }
}
