using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.ContentModel;
using PalmVillas.DbServices;
using PalmVillas.Domain;
using PalmVillas.Models;
using PalmVillas.Models.User;
using PalmVillas.Tests;

namespace PalmVillas.Test.IntegrationTests.DataAccessLayerTests
{
    [TestFixture]
    internal class AccountDbServiceTests
    {
        private PalmContext db;
        private IAccountDbService accountDbService;
        private Mock<UserManager<User>> _userManager;
        private  ILogger<AccountDbService> _logger;
        private const string _userId = "123";

        [SetUp]
        public void Setup()
        {
            db = new PalmContext(Utilities.TestDbContextOptions());
            _userManager = MockHelpers.MockUserManager<User>();
            var mockLogger = new Mock<ILogger<AccountDbService>>();
            _logger = mockLogger.Object;
            accountDbService = new AccountDbService(db,_userManager.Object,_logger);

            Utilities.CreateTestData(db);
        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        [Test]
        public void DeleteUser()
        {
            //accountDbService.DeleteUser(userId);
            //var deletedUser = accountDbService.GetUserById(userId);
            //Assert.IsNull(deletedUser);

        }

        [Test]
        public void DeleteUserExceptionThrownWrongId()
        {
            Action deleteUser = () => accountDbService.DeleteUser("incorrect");
            Assert.That(deleteUser, Throws.TypeOf<ArgumentNullException>());

        }

        [Test]
        public void ReturnUserFromDb()
        {
            //    var user = accountDbService.GetUserById(userId);
            //    Assert.IsNotNull(user);
            //    Assert.That(user.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void ReturnUserFromDb_InvalidId()
        {
            var user = accountDbService.GetUserById("notvalid");
            Assert.That(user, Is.Null);
        }

        //[Test]
        //public void RolesSetOnUser()
        //{
        //    //arrange
        //    var user = accountDbService.GetUserById(_userId);
        //    PaginatedList<UserDetail> Users = new PaginatedList<UserDetail>()
        //    {
        //        new UserDetail
        //        {
        //            Name = user.Name,
        //            UserId = user.Id,
        //            Roles = new List<string>(){"Admin","Villa Manager"}
        //        }
        //    };
        //    //act
        //    var sut = accountDbService.SetUserRoles(Users);
        //    //assert
        //    var userAfter = db.Users
        //        .Where(x=> x.Id==_userId)
        //        .Include(x => x.UserRoles).FirstOrDefault();

        //    Assert.That(sut, Is.True);
        //    var roles = userAfter.UserRoles.Select(x=> x.RoleId).ToList();  
        //}
    }
}
