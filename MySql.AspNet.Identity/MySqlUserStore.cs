using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;

namespace MySql.AspNet.Identity
{
    public class MySqlUserStore<TUser> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser, string>,
        IUserTwoFactorStore<TUser, string>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>
    where TUser : IdentityUser
    {
        private readonly string _connectionString;
        private readonly UserRepository<TUser> _userRepository;
        private readonly UserLoginRepository _userLoginRepository;
        private readonly UserClaimRepository<TUser> _userClaimRepository;
        private readonly UserRoleRepository<TUser> _userRoleRepository;
        public MySqlUserStore()
            : this("DefaultConnection")
        {

        }

        public MySqlUserStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _userRepository = new UserRepository<TUser>(_connectionString);
            _userLoginRepository = new UserLoginRepository(_connectionString);
            _userClaimRepository = new UserClaimRepository<TUser>(_connectionString);
            _userRoleRepository = new UserRoleRepository<TUser>(_connectionString);
        }



        public virtual void Dispose()
        {
            // connection is automatically disposed
        }


        public Task CreateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Id))
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");

            _userRepository.Insert(user);
            return Task.FromResult(true);
        }

        public Task DeleteAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Delete(user);

            return Task.FromResult(true);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            var user =_userRepository.GetById(userId);
            if (user != null)
            {
                user.Roles = _userRoleRepository.PopulateRoles(user.Id);
                user.Claims = _userClaimRepository.PopulateClaims(user.Id);
                user.Logins = _userLoginRepository.PopulateLogins(user.Id);
                return Task.FromResult(user);
            }

            return Task.FromResult(default(TUser));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (userName == null)
            {
                return Task.FromResult(default(TUser));
            }

            var user = _userRepository.GetByName(userName);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                user.Roles = _userRoleRepository.PopulateRoles(user.Id);
                user.Claims = _userClaimRepository.PopulateClaims(user.Id);
                user.Logins = _userLoginRepository.PopulateLogins(user.Id);
                return Task.FromResult(user);
            }

            return Task.FromResult(default(TUser));
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(user.Id))
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");


           _userRepository.Update(user);

            return Task.FromResult(true);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var id = new UserLoginInfo(login.LoginProvider, login.ProviderKey);
            user.Logins.Add(id);

            _userLoginRepository.Insert(user, login);
          

            return Task.FromResult<int>(0);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            var tUserLogin = user.Logins.SingleOrDefault(l =>
            {
                if (l.LoginProvider != login.LoginProvider)
                {
                    return false;
                }
                return l.ProviderKey == login.ProviderKey;
            });
            if (tUserLogin != null)
            {
              user.Logins.Remove(tUserLogin);

             _userLoginRepository.Delete(user, login);

            }
            return Task.FromResult<int>(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult<IList<UserLoginInfo>>(user.Logins.ToList());
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userId = _userLoginRepository.GetByUserLoginInfo(login);

            if (!string.IsNullOrEmpty(userId))
            {
                return FindByIdAsync(userId);
            }
            return Task.FromResult(default(TUser));
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
            {
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });

               _userClaimRepository.Insert(user,claim);

            }
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

           _userClaimRepository.Delete(user, claim);

            return Task.FromResult(0);
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Roles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase))
            {
                user.Roles.Add(roleName);
            }

           _userRoleRepository.Insert(user,roleName);

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Roles.RemoveAll(r => String.Equals(r, roleName, StringComparison.InvariantCultureIgnoreCase));


           _userRoleRepository.Delete(user, roleName);


            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult<IList<string>>(user.Roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Roles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult<bool>(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            if (email == null)
                throw new ArgumentNullException("email");

            var user = _userRepository.GetByEmail(email);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                user.Roles = _userRoleRepository.PopulateRoles(user.Id);
                user.Claims = _userClaimRepository.PopulateClaims(user.Id);
                user.Logins = _userLoginRepository.PopulateLogins(user.Id);
                return Task.FromResult(user);
            }
            return Task.FromResult(default(TUser));
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            DateTimeOffset dateTimeOffset;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.LockoutEndDate.HasValue)
            {
                DateTime? lockoutEndDateUtc = user.LockoutEndDate;
                dateTimeOffset = new DateTimeOffset(DateTime.SpecifyKind(lockoutEndDateUtc.Value, DateTimeKind.Utc));
            }
            else
            {
                dateTimeOffset = new DateTimeOffset();
            }
            return Task.FromResult(dateTimeOffset);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            DateTime? nullable;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (lockoutEnd == DateTimeOffset.MinValue)
            {
                nullable = null;
            }
            else
            {
                nullable = new DateTime?(lockoutEnd.UtcDateTime);
            }
            user.LockoutEndDate = nullable;
            return Task.FromResult<int>(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.TwoFactorAuthEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.TwoFactorAuthEnabled);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public IQueryable<TUser> Users
        {
            get{
                return _userRepository.GetAll();
            }
        }
    }
}
