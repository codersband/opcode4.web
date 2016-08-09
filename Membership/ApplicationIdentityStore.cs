using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace opcode4.web.Membership
{
    public class ApplicationIdentityStore : IUserStore<ApplicationUser, ulong>, IUserPasswordStore<ApplicationUser, ulong>, IRoleStore<IdentityUserRole, ulong>
    {
        #region User    
        static List<ApplicationUser> _users = new List<ApplicationUser> { new ApplicationUser { UserName = "anton@vayosoft.com" } };

        public Task CreateAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(IdentityUserRole role)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(ulong userId)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Task.Run(() => _users.FirstOrDefault(u => u.UserName == userName));
        }
        #endregion User

        #region Role
        public Task UpdateAsync(IdentityUserRole role)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(IdentityUserRole role)
        {
            throw new System.NotImplementedException();
        }

        Task<IdentityUserRole> IRoleStore<IdentityUserRole, ulong>.FindByIdAsync(ulong roleId)
        {
            throw new System.NotImplementedException();
        }

        Task<IdentityUserRole> IRoleStore<IdentityUserRole, ulong>.FindByNameAsync(string roleName)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public void Dispose()
        {

        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
    }
}