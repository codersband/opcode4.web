using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace opcode4.web.Membership
{
    public class ApplicationUserManager : UserManager<ApplicationUser, long>
    {
        public ApplicationUserManager(IApplicationIdentityStore store) : base(store)
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

        public async Task<IdentityResult> ChangePasswordAsync(long userId, string newPassword)
        {
            var user = await Store.FindByIdAsync(userId);
            if (user == null)
                return await new Task<IdentityResult>(() => IdentityResult.Failed());

            var store = Store as IUserPasswordStore<ApplicationUser,long>;
            return await base.UpdatePassword(store, user, newPassword);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(context.GetUserManager<IApplicationIdentityStore>());

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
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}   