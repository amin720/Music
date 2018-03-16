﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Music.Core.Entities;
using Music.Infrastructure.Database;

namespace Music.Infrastructure.Repository
{
	public class CmsUserStore : UserStore<UserIdentity>
	{
		public CmsUserStore()
			: this(new ContextDb())
		{ }
		public CmsUserStore(ContextDb context)
			: base(context)
		{ }

	}

	public class CmsUserManager : UserManager<UserIdentity>
	{
		public CmsUserManager()
			: this(new CmsUserStore())
		{ }

		public CmsUserManager(UserStore<UserIdentity> userStore)
			: base(userStore)
		{ }
	}
}
