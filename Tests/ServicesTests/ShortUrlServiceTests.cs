using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.Services;
using Core.ServicesContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.ServicesTests
{
    public class ShortUrlServiceTests
    {
        // Mocks
        private readonly Mock<IShortUrlsRepo> _repoMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _ctxMock;
        private readonly Mock<IDistributedCache> _cacheMock;

        // No need to mock it
        private readonly IShortenerService _shortenerService;

        public ShortUrlServiceTests()
        {
            _repoMock = new Mock<IShortUrlsRepo>();
            _userManagerMock = GetMockUserManager();
            _ctxMock = new Mock<IHttpContextAccessor>();
            _cacheMock = new Mock<IDistributedCache>();
            _shortenerService = new ShortenerService();
        }

        [Fact]
        public async Task CreateShortUrl_invalidUserId_shouldThrowException()
        {
            // Arrange
            _userManagerMock.Setup(t => t.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            IShortUrlService service = new ShortUrlService
                (
                   _repoMock.Object,
                   _shortenerService,
                   _userManagerMock.Object,
                   _ctxMock.Object,
                   _cacheMock.Object
                );

            // Act / assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateShortUrl(-1, "test url"));
        }

        [Fact]
        public async Task CreateShortUrl_validInputs_shouldReturnGeneratedShortUrl()
        {
            // Arrange
            ApplicationUser user = new();
            _userManagerMock.Setup(t => t.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            _repoMock.Setup(t => t.AddOne(It.IsAny<ShortUrl>()));

            HttpContext mockHttp = new DefaultHttpContext();
            mockHttp.Request.Scheme = "https";
            mockHttp.Request.Host = new HostString(host: "host", port: 123);
            _ctxMock.Setup(t => t.HttpContext).Returns(mockHttp);

            _repoMock.Setup(t => t.UpdateOne(It.IsAny<ShortUrl>(), It.IsAny<ShortUrl>())).ReturnsAsync(new ShortUrl());

            IShortUrlService service = new ShortUrlService
                (
                   _repoMock.Object,
                   _shortenerService,
                   _userManagerMock.Object,
                   _ctxMock.Object,
                   _cacheMock.Object
                );

            // Act 
            ShortUrl res = await service.CreateShortUrl(1, "url");

            // Assert
            Assert.Equal("0", res.Code);
            Assert.Equal("https://host:123/0", res.Value);
            Assert.Equal("url", res.OriginalUrl);
            _repoMock.Verify(t => t.AddOne(It.IsAny<ShortUrl>()), Times.Once);
            _repoMock.Verify(t => t.UpdateOne(It.IsAny<ShortUrl>(), It.IsAny<ShortUrl>()), Times.Once);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var validator = new Mock<IUserValidator<ApplicationUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<IPasswordValidator<ApplicationUser>>();
            var pwdValidator = new Mock<IPasswordValidator<ApplicationUser>>();
            pwdValidators.Add(pwdValidator.Object);
            var normalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                pwdValidators,
                normalizer.Object,
                errors.Object,
                services.Object,
                logger.Object);
        }
    }
}
