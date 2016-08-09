using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace opcode4.web.Membership
{
    public class ApplicationUser : IUser<ulong>
    {
        public virtual ulong Id { get; set; }
        public virtual string UserName { get; set; }
        public string PasswordHash { get; set; }
        public virtual ICollection<IdentityUserRole> Roles { set; get; }
        public virtual ulong ProviderId { set; get; }
    }

    public class IdentityUserRole : IRole<ulong>
    {
        public virtual ulong Id { get; set; }
        public virtual string Name { get; set; }
    }
}
