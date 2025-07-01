using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Services;

namespace ApiTest
{
    [TestClass]
    public class ApiKeyServiceTests
    {
        private static LitterDbContext CreateContextWithOptions(string dbName)
        {
            var options = new DbContextOptionsBuilder<LitterDbContext>().UseInMemoryDatabase(databaseName: dbName).Options;
            return new LitterDbContext(options);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsFalse_ForEmptyGuid()
        {
            using var context = CreateContextWithOptions("EmptyGuidDb");
            var service = new ApiKeyService(context);

            bool result = service.IsValidApiKey(Guid.Empty);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsFalse_ForNonexistentKey()
        {
            using var context = CreateContextWithOptions("NonexistentDb");
            var service = new ApiKeyService(context);

            bool result = service.IsValidApiKey(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsFalse_WhenKeyIsInactive()
        {
            using var context = CreateContextWithOptions("InactiveDb");
            var key = Guid.NewGuid();
            context.ApiKeys.Add(new ApiKey { Key = key, IsActive = false, Type = "backend" });
            context.SaveChanges();

            var service = new ApiKeyService(context);
            bool result = service.IsValidApiKey(key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsFalse_WhenTypeIsNotBackend()
        {
            using var context = CreateContextWithOptions("WrongTypeDb");
            var key = Guid.NewGuid();
            context.ApiKeys.Add(new ApiKey { Key = key, IsActive = true, Type = "frontend" });
            context.SaveChanges();

            var service = new ApiKeyService(context);
            bool result = service.IsValidApiKey(key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsFalse_WhenExpired()
        {
            using var context = CreateContextWithOptions("ExpiredDb");
            var key = Guid.NewGuid();
            context.ApiKeys.Add(new ApiKey
            {
                Key = key,
                IsActive = true,
                Type = "backend",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
            });
            context.SaveChanges();

            var service = new ApiKeyService(context);
            bool result = service.IsValidApiKey(key);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsTrue_ForValidKeyWithoutExpiration()
        {
            using var context = CreateContextWithOptions("ValidNoExpiryDb");
            var key = Guid.NewGuid();
            context.ApiKeys.Add(new ApiKey
            {
                Key = key,
                IsActive = true,
                Type = "backend",
                ExpiresAt = null
            });
            context.SaveChanges();

            var service = new ApiKeyService(context);
            bool result = service.IsValidApiKey(key);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidApiKey_ReturnsTrue_ForValidKeyWithFutureExpiration()
        {
            using var context = CreateContextWithOptions("ValidWithExpiryDb");
            var key = Guid.NewGuid();
            context.ApiKeys.Add(new ApiKey
            {
                Key = key,
                IsActive = true,
                Type = "backend",
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            });
            context.SaveChanges();

            var service = new ApiKeyService(context);
            bool result = service.IsValidApiKey(key);

            Assert.IsTrue(result);
        }
    }
}
