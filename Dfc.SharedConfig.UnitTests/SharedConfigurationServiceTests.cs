using Dfc.SharedConfig.Contracts;
using Dfc.SharedConfig.Repositories;
using Dfc.SharedConfig.Services;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.SharedConfig.UnitTests
{
    public class SharedConfigurationServiceTests
    {
        private readonly ISharedConfigCacheProvider defaultMemoryStore;
        private readonly IConfigurationRepository defaultTableStore;

        private const string DummyServiceName = "DummyServiceName";
        private const string DummyKeyName = "DummyKeyName";

        public SharedConfigurationServiceTests()
        {
            this.defaultMemoryStore = A.Fake<ISharedConfigCacheProvider>();
            A.CallTo(() => defaultMemoryStore.GetConfig(A<string>.Ignored)).Returns(string.Empty);

            this.defaultTableStore = A.Fake<IConfigurationRepository>();
        }

        [Fact]
        public async Task GetConfigAsyncThrowsNotImplementedExceptionWhenDataIsEncrypted()
        {
            // Arrange
            var service = new SharedConfigurationService(defaultTableStore, defaultMemoryStore);

            // Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await service.GetConfigAsync<string>(DummyServiceName, DummyKeyName, true).ConfigureAwait(false)).ConfigureAwait(false);
            A.CallTo(() => defaultMemoryStore.GetConfig(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => defaultMemoryStore.SetConfig(A<string>.Ignored, A<object>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => defaultTableStore.GetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetConfigAsyncWhenFoundInMemoryCacheThenReturnsMemoryStoreValueParsedAsObject()
        {
            // Arrange
            var dummyResultObject = new SampleConfiguration
            {
                DataField1 = "value1",
                DataField2 = "value2",
                DataField3 = "value3",
            };

            var expectedStringValue = JsonConvert.SerializeObject(dummyResultObject);
            var memoryStore = A.Fake<ISharedConfigCacheProvider>();
            A.CallTo(() => memoryStore.GetConfig(A<string>.Ignored)).Returns(expectedStringValue);

            var service = new SharedConfigurationService(defaultTableStore, memoryStore);

            // Act
            var result = await service.GetConfigAsync<SampleConfiguration>(DummyServiceName, DummyKeyName).ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(JsonConvert.DeserializeObject<SampleConfiguration>(expectedStringValue));
            A.CallTo(() => memoryStore.GetConfig(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => memoryStore.SetConfig(A<string>.Ignored, A<object>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => defaultTableStore.GetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetConfigAsyncReturnsTableStoreValueParsedAsObject()
        {
            // Arrange
            var dummyResultObject = new SampleConfiguration
            {
                DataField1 = "value1",
                DataField2 = "value2",
                DataField3 = "value3",
            };

            var expectedStringValue = JsonConvert.SerializeObject(dummyResultObject);
            var tableStore = A.Fake<IConfigurationRepository>();
            A.CallTo(() => tableStore.GetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedStringValue);

            var service = new SharedConfigurationService(tableStore, defaultMemoryStore);

            // Act
            var result = await service.GetConfigAsync<SampleConfiguration>(DummyServiceName, DummyKeyName).ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(JsonConvert.DeserializeObject<SampleConfiguration>(expectedStringValue));
            A.CallTo(() => defaultMemoryStore.GetConfig(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => defaultMemoryStore.SetConfig(A<string>.Ignored, A<object>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => tableStore.GetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetConfigAsyncReturnsTableStoreValueParsedAsString()
        {
            // Arrange
            var expectedStringValue = JsonConvert.SerializeObject("SomeResultValue");
            var tableStore = A.Fake<IConfigurationRepository>();
            A.CallTo(() => tableStore.GetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored)).Returns(expectedStringValue);

            var service = new SharedConfigurationService(tableStore, defaultMemoryStore);

            // Act
            var result = await service.GetConfigAsync<string>(DummyServiceName, DummyKeyName).ConfigureAwait(false);

            // Assert
            Assert.Equal(JsonConvert.DeserializeObject<string>(expectedStringValue), result);
            A.CallTo(() => defaultMemoryStore.GetConfig(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => defaultMemoryStore.SetConfig(A<string>.Ignored, A<object>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => tableStore.GetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SetConfigAsyncThrowsNotImplementedExceptionWhenDataIsEncrypted()
        {
            // Arrange
            var service = new SharedConfigurationService(defaultTableStore, defaultMemoryStore);

            // Assert
            await Assert.ThrowsAsync<NotImplementedException>(async () => await service.SetConfigAsync<string>(DummyServiceName, DummyKeyName, "someData", true).ConfigureAwait(false)).ConfigureAwait(false);

            A.CallTo(() => defaultTableStore.SetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => defaultMemoryStore.SetConfig(A<string>.Ignored, A<object>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SetConfigAsyncReturnsTableStoreValueParsedAsString()
        {
            // Arrange
            var valueToInsert = JsonConvert.SerializeObject("SomeResultValue");
            var tableStore = A.Fake<IConfigurationRepository>();
            A.CallTo(() => tableStore.SetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(valueToInsert);

            var service = new SharedConfigurationService(tableStore, defaultMemoryStore);

            // Act
            await service.SetConfigAsync<string>(DummyServiceName, DummyKeyName, valueToInsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => tableStore.SetCloudConfigAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => defaultMemoryStore.SetConfig(A<string>.Ignored, A<object>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}