using MaxJobTracker.Models;
using System.Collections.Generic;

namespace MaxJobTracker.Data
{
    public static class DummyUserStore
    {
        public static List<ApplicationUser> UserList = new List<ApplicationUser> {
                new ApplicationUser { Id = "1", Name = "User A", Email = "usera@gmail.com", Password = "123" },
                new ApplicationUser { Id = "2", Name = "User B", Email = "userb@gmail.com", Password = "123" },
                new ApplicationUser { Id = "3", Name = "User C", Email = "userc@gmail.com", Password = "123" }
            };
    }
}
