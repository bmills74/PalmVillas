using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

using PalmVillas.DbServices;
using PalmVillas.Domain;


namespace PalmVillas.Tests
{
    public static class Utilities
    {


        #region snippet1
        public static DbContextOptions<PalmContext> TestDbContextOptions()
        {
            // Create a new service provider to create a new in-memory database.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance using an in-memory database and 
            // IServiceProvider that the context should resolve all of its 
            // services from.
            var builder = new DbContextOptionsBuilder<PalmContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
        #endregion

        public static PageContext GetPageContextWithUserPrincipal()
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Test User"),
                     new Claim(ClaimTypes.Email, "test@test.com"),
                      new Claim(ClaimTypes.UserData, "")
                };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(x => x.Identity).Returns(claimsIdentity);
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(mockPrincipal.Object);
            return new PageContext() { HttpContext = mockHttpContext.Object };
        }

        internal static void CreateTestData(PalmContext db)
        {
            IAccountDbService accountDbService = new AccountDbService(db, null, null);
            IBookingDbService bookingDbService = new BookingDbService(db);
            IVillaDbService villaDbService = new VillaDbService(db);
            var user = new User()
            {
                Id = "123",
                Name = "Test",
                Email = "test@email.com"
            };
            accountDbService.CreateUser(user);

            var villa = new Villa()
            {
                Id = 1,
                Name = "Test",
                Price = 250,
                Rooms = 2,
                Description = "Test description",
                Images = "[\"/images/villa1_image1.jpg\",\"/images/villa1_image2.jpg\",\"/images/villa1_image3.jpg\"]"
            };
            villaDbService.AddVilla(villa);

            var start = DateTime.Now.AddDays(1);
            var end = DateTime.Now.AddDays(7);
            var booking = new Booking()
            {
                UserId = user.Id,
                StartDate = start.ToString(),
                EndDate = end.ToString(),
                Price = (long)villa.Price * (long)(end - start).Days,
                VillaId = villa.Id,
            };            

            db.Bookings.Add(booking);

            var roles = new List<IdentityRole>() {
                new IdentityRole() {Id = "1",Name="Admin" },
            new IdentityRole(){Id="2",Name="Villa Manager"} };
            db.Roles.AddRange(roles);
            db.SaveChanges();
        }
    }
}
