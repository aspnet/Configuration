using Xunit;

namespace Microsoft.Extensions.Configuration.Test
{
    public class ConfigurationBuilderTest
    {
        [Fact]
        public void DoesNotLoadProviderWhenAdded()
        {
            // Arrange
            var fakeProvider = new FakeProvider();
            var configurationBuilder = new ConfigurationBuilder();

            // Act
            configurationBuilder.Add(fakeProvider);

            // Assert
            Assert.False(fakeProvider.Loaded);
        }
    }
}
