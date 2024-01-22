using Moq;
using Palm.Models.Book;
using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Models;
using PalmVillas.Test.UnitTests.TestCaseSource;
using PalmVillas.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmVillas.Test.UnitTests
{
    [TestFixture]
    internal class BookingUtilityShould
    {
        private PalmContext db;
        private List<Booking> bookings;

        [SetUp]
        public void SetUp()
        {
            db = new PalmContext(Utilities.TestDbContextOptions());
            bookings = new List<Booking>()
                {
                    new Booking()
                    {
                        StartDate = DateTime.Now.AddDays(7).ToString(),
                        EndDate = DateTime.Now.AddDays(11).ToString(),
                    },
                     new Booking()
                    {
                        StartDate = DateTime.Now.AddDays(14).ToString(),
                        EndDate = DateTime.Now.AddDays(16).ToString(),
                    },
                      new Booking()
                    {
                        StartDate = DateTime.Now.AddDays(17).ToString(),
                        EndDate = DateTime.Now.AddDays(18).ToString(),
                    }
                };
        }

        [Test]
        [TestCaseSource(typeof(BookingUtilityNoConflictTestData), "TestCases")]
        public void IfConflictReturnTrue_NoConflict(Booking bookingToTest)
        {
            //Arrange
            var villaDbServiceMock = new Mock<VillaDbService>(null);
            villaDbServiceMock.Setup(x => x.GetFutureBookings(It.IsAny<int>())).Returns(bookings);
            var sut = new BookingUtility(villaDbServiceMock.Object);

            //Act
            var result = sut.IfConflictReturnTrue(bookingToTest);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(BookingUtilityHasConflictTestData), "TestCases")]
        public void IfConflictReturnTrue_HasConflict(Booking bookingToTest)
        {
            //Arrange
            var villaDbServiceMock = new Mock<VillaDbService>(null);
            villaDbServiceMock.Setup(x => x.GetFutureBookings(It.IsAny<int>())).Returns(bookings);
            var sut = new BookingUtility(villaDbServiceMock.Object);

            //Act
            var result = sut.IfConflictReturnTrue(bookingToTest);

            //Assert
            Assert.That(result, Is.True);
        }
    }
}
