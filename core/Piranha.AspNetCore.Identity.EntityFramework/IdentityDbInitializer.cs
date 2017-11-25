using Microsoft.EntityFrameworkCore;

namespace Piranha.AspNetCore.Identity.EntityFramework
{
    public static class IdentityDbInitializer
    {
        public static void Initialize(IdentityDb context)
        {
            context.Database.Migrate();
        }
    }
}