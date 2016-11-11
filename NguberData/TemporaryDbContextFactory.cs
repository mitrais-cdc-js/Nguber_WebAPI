using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NguberData.Data;

namespace NguberData {
  public class TemporaryDbContextFactory : IDbContextFactory<ApplicationDbContext> {
    public ApplicationDbContext Create (DbContextFactoryOptions options) {
      var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
      builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NguberDB;AttachDbFileName=D:\\Lorensius4655\\Documents\\Projects-Test\\Nguber\\Database\\NguberDB.mdf;Trusted_Connection=True;MultipleActiveResultSets=true");
      return new ApplicationDbContext(builder.Options);
    }
  }
}
