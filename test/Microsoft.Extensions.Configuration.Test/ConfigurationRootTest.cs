using Xunit;

namespace Microsoft.Extensions.Configuration.Test
{
    public class ConfigurationRootTest
    {
        [Fact]
        public void LoadsProvidersDuringConstructor()
        {
            // Arrange
            var fakeProvider = new FakeProvider();
            var anotherFakeProvider = new FakeProvider();

            // Act
            var configurationRoot = new ConfigurationRoot(new[] { fakeProvider, anotherFakeProvider });

            // Assert
            Assert.True(fakeProvider.Loaded);
            Assert.True(anotherFakeProvider.Loaded);
        }
    }
}
