using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PalmVillas.Domain;
using PalmVillas.Models.User;

using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PalmVillas.Migrations;
using PalmVillas.Models;
//using System.Data.Entity;

namespace PalmVillas.DbServices
{
    public interface IAccountDbService
    {

        void CreateUser(User newUser);
        User GetUserById(string userName);
        void DeleteUser(string id);
        User? GetUserByUserName(string userName);
        List<IdentityUserClaim<string>> GetClaimsByUserId(string userId);
        void AddOrUpdateClaim(string id, Claim avatarClaim);
        IQueryable<UserDetail> GetUsersAndRolesFromIncludes();
        IQueryable<UserDetail> GetUsersAndRolesFromJoin();
        bool SetUserRoles(PaginatedList<UserDetail> users);
    }

    public class AccountDbService : IAccountDbService
    {
        private readonly UserManager<User> _userManager;
        private readonly PalmContext db;
        private readonly ILogger<AccountDbService> _logger;
        public AccountDbService(PalmContext db, UserManager<User> userManager, ILogger<AccountDbService> logger)
        {
            this.db = db;
            _userManager = userManager;
            _logger = logger;
        }
        public User GetUserById(string id)
        {
            return db.Users.Find(id);
        }

        public List<IdentityUserClaim<string>> GetClaimsByUserId(string userId)
        {
            var claims = db.UserClaims.Where(x => x.UserId == userId).ToList();
            return claims;
        }

        public void AddOrUpdateClaim(string userId, Claim avatarClaim)
        {
            var claims = db.UserClaims.Where(x => x.UserId == userId && x.ClaimType == avatarClaim.Type).ToList();
            if (claims.IsNullOrEmpty())
            {
                db.UserClaims.Add(new IdentityUserClaim<string>()
                {
                    UserId = userId,
                    ClaimType = avatarClaim.Type,
                    ClaimValue = avatarClaim.Value
                });
                db.SaveChanges();
            }
            else
            {
                foreach (var claim in claims)
                {
                    claim.ClaimValue = avatarClaim.Value;
                }
            }

        }

        public void DeleteUser(string id)
        {
            var user = db.Users.Find(id);
            Guard.Against.Null(user);
            db.Users.Remove(user);
            db.SaveChanges();
        }
        public void CreateUser(User newUser)
        {
            db.Users.Add(newUser);
            db.SaveChanges();
        }

        public User? GetUserByUserName(string userName)
        {
            return db.Users.FirstOrDefault(x => x.Email == userName);
        }

        /// <summary>
        /// Gets Users and includes their roles using standard 'include' syntax
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserDetail> GetUsersAndRolesFromIncludes()
        {
            return db.Users.Include(x => x.UserRoles)
                .Select(x => new UserDetail()
                {
                    UserId = x.Id,
                    Name = x.Name,
                    Roles = x.UserRoles.Select(x => x.RoleId)
                });
        }

        /// <summary>
        /// gets users and roles using a more laborious join syntax
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserDetail> GetUsersAndRolesFromJoin()
        {
            //gets a join of users and associated roleIds
            var query = from user in db.Users
                        join role in db.UserRoles on user.Id equals role.UserId into ur
                        from userRoles in ur.DefaultIfEmpty()
                        select new
                        {
                            UserId = user.Id,
                            Name = user.Name,
                            RoleId = userRoles.RoleId
                        };

            //now group by the user and add a list of their roles to the result
            return query.GroupBy(x => x.UserId)
                .Select(x => new UserDetail()
                {
                    UserId = x.Key,
                    Name = x.First().Name,
                    Roles = x.Select(y => y.RoleId)
                });
        }

        public bool SetUserRoles(PaginatedList<UserDetail> Users)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {                   
                    var users = db.Users.Where(x => Users.Select(y => y.UserId).Contains(x.Id)).ToList();
                    var existingRoles =
                        from role in db.UserRoles
                        where Users.Select(x => x.UserId).Contains(role.UserId)
                        join systemRole in db.Roles on role.RoleId equals systemRole.Id
                        select new
                        {
                            UserId = role.UserId,
                            RoleId = role.RoleId,
                            RoleName = systemRole.Name
                        };

                    foreach (var user in Users)
                    {
                        var dbuser = users.First(x => x.Id == user.UserId);
                        var userRoleNamessToAdd = user.InRoles
                            .Where(x => x.Value && !(existingRoles.Select(y => y.RoleName).Contains(x.Key)
                            && existingRoles.Select(y => y.UserId).Contains(user.UserId)))
                            .Select(x => x.Key)
                            .ToList();
                        var result = _userManager.AddToRolesAsync(dbuser, userRoleNamessToAdd);

                        //this savechanges is in a transaction so will be reversed if something goes wrong later
                        db.SaveChanges();

                        var userRoleNamessToRemove = user.InRoles
                           .Where(x => !x.Value && (existingRoles.Select(y => y.RoleName).Contains(x.Key)
                           && existingRoles.Select(y => y.UserId).Contains(user.UserId)))
                           .Select(x => x.Key)
                           .ToList();

                        var removeResult = _userManager.RemoveFromRolesAsync(dbuser, userRoleNamessToRemove);
                        db.SaveChanges();

                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex.Message, ex);
                    db.SaveChanges();
                    return false;
                }
            }
        }
    }
}
