using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Models;
using PalmVillas.Models.Book;
using PalmVillas.Pages.Book;
using PalmVillas.Tests;

namespace PalmVillas.Test.IntegrationTests.PageTests
{
    [TestFixture]
    internal class BookingSummaryPageTests
    {
        private PalmContext db;
        private DbServices.IAccountDbService accountDbService;
        private IBookingDbService bookingDbService;
        private IVillaDbService villaDbService;

        private BookingSummaryModel pageModel;
        private Mock<UserManager<User>> _userManager;
        private ILogger<AccountDbService> _logger;


        [SetUp]
        public void Setup()
        {
            _userManager = MockHelpers.MockUserManager<User>();
            var mockLogger = new Mock<ILogger<AccountDbService>>();
            _logger = mockLogger.Object;
            db = new PalmContext(Utilities.TestDbContextOptions());
            accountDbService = new AccountDbService(db,_userManager.Object,_logger);
            bookingDbService = new BookingDbService(db);
            villaDbService = new VillaDbService(db);

            pageModel = new BookingSummaryModel(bookingDbService, accountDbService, villaDbService, new BookingUtility(villaDbService))
            {
                //sets a user in the pagecontext
                PageContext = Utilities.GetPageContextWithUserPrincipal()
            };
            Utilities.CreateTestData(db);
            pageModel.Input = GetInput();
        }

        [Test]
        public void OnGetPopulatePageModel()
        {
            pageModel.OnGet();
            Assert.That(pageModel.Villa, Is.Not.Null);
            Assert.That(pageModel.Villa.Id, Is.EqualTo(1));
            Assert.That(pageModel.Villa.Price, Is.EqualTo(250));
            Assert.That(pageModel.Villa.TotalPrice, Is.EqualTo(pageModel.Villa.Price * pageModel.Villa.NumNights));
        }

        [Test]
        public void OnPostAssertBookingMade()
        {
            var bookingMock = new Mock<BookingUtility>(null);
            bookingMock.Setup(x => x.IfConflictReturnTrue(null)).Returns(false);
            pageModel = new BookingSummaryModel(bookingDbService, accountDbService, villaDbService, bookingMock.Object)
            {
                PageContext = Utilities.GetPageContextWithUserPrincipal()
            };
            pageModel.Input = GetInput();

            //Act
            var result = pageModel.OnPost();
            // Assert
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            //and confirm there was no booking conflict and it's going to the right view
            Assert.That(pageModel.BookingConflict, Is.False);
        }

        private BookingSummaryInput GetInput()
        {
            return new BookingSummaryInput()
            {

                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddDays(7).ToString(),
                VillaId = 1,
                UserName = "test@test.com"
            };
        }


    }
}
