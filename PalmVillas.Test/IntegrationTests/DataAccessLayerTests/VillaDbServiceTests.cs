using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.UserSecrets;
using NuGet.ContentModel;
using PalmVillas.DbServices;
using PalmVillas.Tests;

namespace PalmVillas.Test.IntegrationTests.DataAccessLayerTests
{
    [TestFixture]
    internal class VillaDbServiceTests
    {
        private PalmContext db;
        private IVillaDbService _villaDbService;

        [SetUp]
        public void Setup()
        {
            db = new PalmContext(Utilities.TestDbContextOptions());
            _villaDbService = new VillaDbService(db);
            Utilities.CreateTestData(db);
        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        [Test]
        public void AddVilla()
        {
            //Arrange
            var villa = new Domain.Villa()
            {
                Name = "test",
                Description = "test description",
                Price = 111,
                Rooms = 2
            };
            //Act
            var id = _villaDbService.AddVilla(villa);

            //Assert
            Assert.That(id, Is.GreaterThan(0));

            //Check it actually exists
            var returnedVilla = db.Villas.Find(id);
            Assert.That(returnedVilla, Is.Not.Null);
            Assert.That(returnedVilla.Price, Is.EqualTo(111));
        }

        [Test]
        public void GetVilla()
        {
            var villa = db.Villas.Find((long)1);
            Assert.That(villa, Is.Not.Null);
        }

        [Test]
        public void ListVillas()
        {
            var villa = db.Villas.ToList();
            Assert.That(villa, Is.Not.Null);
        }
    }
}
