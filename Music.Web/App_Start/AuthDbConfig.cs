﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Music.Core.Entities;
using Music.Infrastructure.Repository;

namespace Music.Web.App_Start
{
	public class AuthDbConfig
	{
		public static async Task RegisterAdmin()
		{
			using (var roles = new RoleRepository())
			{
				if (await roles.GetRoleByNameAsync("admin") == null)
				{
					await roles.CreateAsync(new IdentityRole("admin"));
				}

				if (await roles.GetRoleByNameAsync("editor") == null)
				{
					await roles.CreateAsync(new IdentityRole("editor"));
				}

				if (await roles.GetRoleByNameAsync("author") == null)
				{
					await roles.CreateAsync(new IdentityRole("author"));
				}

				if (await roles.GetRoleByNameAsync("user") == null)
				{
					await roles.CreateAsync(new IdentityRole("user"));
				}
			}

			using (var users = new UserRepository())
			{
				var user = await users.GetUserByNameAsync("admin");

				if (user == null)
				{
					var adminUser = new UserIdentity
					{
						UserName = "admin",
						Email = "admin@cms.com",
						DisplayName = "Administrator"
					};

					await users.CreateAsync(adminUser, "User@1234");
				}

				await users.AddUserToRoleAsync(user, "admin");
			}


		}
	}
}