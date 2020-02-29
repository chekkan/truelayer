using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests
{
    public class CallbackControllerTests
    {
        [Fact]
        public void SutImplementsControllerBase()
        {
            var trueLayerClient = new Mock<ITrueLayerDataApiClient>().Object;
            var userService = new Mock<IUserService>().Object;
            var sut = new CallbackController(trueLayerClient, userService);
            Assert.IsAssignableFrom<ControllerBase>(sut);
        }
    }
}