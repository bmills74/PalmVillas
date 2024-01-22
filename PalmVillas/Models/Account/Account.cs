//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;
//using Palm.Models.Account;
//using PalmVillas.Pages;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using PalmVillas.DbServices;

//namespace PalmVillas.Models.Account
//{
//    public class IAccountDbService 
//    {
//        private readonly IAccountDbService _accountDbService;
//        public IAccountDbService(IAccountDbService accountDbService)
//        {
//            _accountDbService = accountDbService;
//        }

//        public User Login(string username)
//        {
//            var user = _accountDbService.(username);
           
//            return user;
//        }

//        public User CreateUser(GoogleResponse response)
//        {
//            var newUser = new User()
//            {
//                Email = response.Email,
//                Name = response.Name,
//                UserId = response.Sub
//            };
//            _accountDbService.CreateUser(newUser);
//            return newUser;
//        }

       
//    }
//}
