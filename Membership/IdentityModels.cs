using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace opcode4.web.Membership
{
    public class ApplicationUser : IUser<long>
    {
        public virtual long Id { get; set; }
        public bool IsInitialized => Id > 0;
        public virtual string UserName { get; set; }
        public string PasswordHash { get; set; }
        public virtual ICollection<IdentityUserRole> Roles { set; get; }
        public virtual long ProviderId { set; get; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class IdentityUserRole : IRole<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
    }
}
