using Poshta.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poshta.IntegrationTests.Repositories.PostOfficeTypeMethods
{
    public class GetPostOfficeTypesAsyncTests : RepositoryTestsBase
    {
        [Fact]
        public async Task GetPostOfficeTypesAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await postOfficeTypesRepository.GetPostOfficeTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPostOfficeTypesAsync_PopulatedDatabase_ReturnsPostOfficeTypes()
        {
            // Arrange
            var postOfficeType1Result = PostOfficeType.Create(Guid.NewGuid(), "PostOfficeUpTo30Kg", 30f, 100f, 50f, 50f);
            var postOfficeType2Result = PostOfficeType.Create(Guid.NewGuid(), "PostOfficeUpTo15Kg", 15f, 100f, 50f, 50f);


            Assert.True(postOfficeType1Result.IsSuccess && postOfficeType2Result.IsSuccess);
            var postOfficeType1 = postOfficeType1Result.Value;
            var postOfficeType2 = postOfficeType2Result.Value;

            await postOfficeTypesRepository.CreateAsync(postOfficeType1);
            await postOfficeTypesRepository.CreateAsync(postOfficeType2);

            // Act
            var result = await postOfficeTypesRepository.GetPostOfficeTypesAsync();

            // Assert
            Assert.NotNull(result);
            var postOfficeTypesList = Assert.IsAssignableFrom<List<PostOfficeType>>(result);
            Assert.Equal(2, postOfficeTypesList.Count);
            Assert.Contains(postOfficeTypesList, p => p.Id == postOfficeType1.Id);
            Assert.Contains(postOfficeTypesList, p => p.Id == postOfficeType2.Id);
        }
    }
}
