using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Music.Core.Entities;

namespace Music.Infrastructure.Database
{
	public class ContextDb : IdentityDbContext<UserIdentity>
	{
		public ContextDb()
			: base("name=MusicUni")
		{

		}
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
