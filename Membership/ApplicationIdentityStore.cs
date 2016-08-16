using Microsoft.AspNet.Identity;

namespace opcode4.web.Membership
{
    public interface IApplicationIdentityStore : IUserPasswordStore<ApplicationUser, long>
    {

    }
}