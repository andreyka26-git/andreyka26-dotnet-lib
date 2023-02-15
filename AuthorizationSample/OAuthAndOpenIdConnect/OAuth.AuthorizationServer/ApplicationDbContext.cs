using Microsoft.EntityFrameworkCore;
using Polly;

namespace OAuth.AuthorizationServer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 

        }
    }
}
