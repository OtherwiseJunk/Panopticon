using NUnit.Framework;
using Moq;
using Panopticon.Data.Interfaces;
using Panopticon.Shared.Models;
using Panopticon.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panopticon.Tests
{
    [TestFixture]
    public class OOCControllerTests
    {
        private Mock<IOocService> _mockService;
        private OOCController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IOocService>();
            _controller = new OOCController(_mockService.Object);
        }

        [Test]
        public void GetAllOOCItems_ReturnsItems_WhenItemsExist()
        {
            var items = new List<OOCItem> { new OOCItem(), new OOCItem() };
            _mockService.Setup(s => s.GetAllOocItems()).Returns(items);

            var result = _controller.GetAllOOCItems();
            
            Assert.That(result.Value, Is.EqualTo(items));
        }

        [Test]
        public void GetAllOOCItems_ReturnsNotFound_WhenNoItemsExist()
        {
            _mockService.Setup(s => s.GetAllOocItems()).Returns(new List<OOCItem>());

            var result = _controller.GetAllOOCItems();

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void GetOOCItemByID_ReturnsItem_WhenItemExists()
        {
            var item = new OOCItem { ItemID = 1 };
            _mockService.Setup(s => s.GetOocItem(1)).Returns(item);

            var result = _controller.GetOOCItemByID(1);
            
            Assert.That(result.Value, Is.EqualTo(item));
        }

        [Test]
        public void GetOOCItemByID_ReturnsNotFound_WhenItemDoesNotExist()
        {
            _mockService.Setup(s => s.GetOocItem(1)).Returns((OOCItem)null);

            var result = _controller.GetOOCItemByID(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateOOCItem_ReturnsNoContent_WhenItemIsCreated()
        {
            var item = new OOCItem();
            _mockService.Setup(s => s.CreateOocItem(item)).Returns(Task.CompletedTask);

            var result = await _controller.CreateOOCItem(item);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task CreateOOCItem_ReturnsUnprocessableEntity_WhenExceptionIsThrown()
        {
            var item = new OOCItem();
            _mockService.Setup(s => s.CreateOocItem(item)).Throws(new System.Exception());

            var result = await _controller.CreateOOCItem(item);

            Assert.That(result, Is.InstanceOf<UnprocessableEntityResult>());
        }

        [Test]
        public void DeleteOOCItemByID_ReturnsNoContent_WhenItemIsDeleted()
        {
            var item = new OOCItem { ItemID = 1 };
            _mockService.Setup(s => s.GetOocItem(1)).Returns(item);
            _mockService.Setup(s => s.DeleteOocItem(item)).Returns(Task.CompletedTask);

            var result = _controller.DeleteOOCItemByID(1);

            Assert.That(result.Result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void DeleteOOCItemByID_ReturnsNotFound_WhenItemDoesNotExist()
        {
            _mockService.Setup(s => s.GetOocItem(1)).Returns((OOCItem)null);

            var result = _controller.DeleteOOCItemByID(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }
    }
}