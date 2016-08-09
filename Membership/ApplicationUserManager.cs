using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace opcode4.web.Membership
{
    public class ApplicationUserManager : UserManager<ApplicationUser, ulong>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, ulong> store) : base(store)
        {
            this.PasswordHasher = new CustomPasswordHasher();
        }

        public override async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            //PasswordVerificationResult result = this.PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            var user = await Store.FindByNameAsync(userName);
            if (user != null)
            { 
                return await CheckPasswordAsync(user, password) ? user : null;
            }
            return null;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(context.GetUserManager<IUserPasswordStore<ApplicationUser, ulong>>());

            //manager.UserValidator = new UserValidator<ApplicationUser, ulong>(manager)
            //{
            //    AllowOnlyAlphanumericUserNames = false,
            //    RequireUniqueEmail = true
            //};
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true,
            //};
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, ulong>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}   