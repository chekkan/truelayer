using System;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Api.UnitTests
{
    public class CallbackControllerTests
    {
        [Fact]
        public void SutImplementsControllerBase()
        {
            var sut = new CallbackController();
            Assert.IsAssignableFrom<ControllerBase>(sut);
        }
    }
}