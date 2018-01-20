namespace BudgetApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BudgetApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BudgetApp.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            // Create Roles
            #region
            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                roleManager.Create(new IdentityRole { Name = "Administrator" });
            }

            if (!context.Roles.Any(r => r.Name == "SuperUser"))
            {
                roleManager.Create(new IdentityRole { Name = "SuperUser" });
            }

            if (!context.Roles.Any(r => r.Name == "HeadOfHousehold"))
            {
                roleManager.Create(new IdentityRole { Name = "HeadOfHousehold" });
            }

            if (!context.Roles.Any(r => r.Name == "MemberOfHousehold"))
            {
                roleManager.Create(new IdentityRole { Name = "MemberOfHousehold" });
            }

            if (!context.Roles.Any(r => r.Name == "GuestAtFrontDoor"))
            {
                roleManager.Create(new IdentityRole { Name = "GuestAtFrontDoor" });
            }
            #endregion

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            //Create Users: Administrator & SuperUser
            #region
            if (!context.Users.Any(u => u.Email == "valschanewolf@bellsouth.net"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "valschanewolf@bellsouth.net",
                    Email = "valschanewolf@bellsouth.net",
                    FirstName = "Val",
                    LastName = "Schanewolf",
                    DisplayName = "ValSchanewolfAdministrator",
                }, "Abc&123");
            }
            var userId = userManager.FindByEmail("valschanewolf@bellsouth.net").Id;
            userManager.AddToRole(userId, "Administrator");

            if (!context.Users.Any(u => u.Email == "superuser@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "superuser@mailinator.com",
                    Email = "superuser@mailinator.com",
                    FirstName = "Super",
                    LastName = "User",
                    DisplayName = "SuperUser"
                }, "Abc&123");
            }
            userId = userManager.FindByEmail("superuser@mailinator.com").Id;
            userManager.AddToRole(userId, "Administrator");
            #endregion

            #region
            var GuestHousehold = new Household
            {
                HouseholdName = "Guest",
                Active = true,
                DateCreated = DateTime.Now
            };
            context.Households.Add(GuestHousehold);
            context.SaveChanges();

            if (!context.Users.Any(u => u.Email == "GuestAtFrontDoor@mailinator.com"))
            {
                var houseId = context.Households.FirstOrDefault(h => h.HouseholdName == "Guest").Id;
                userManager.Create(new ApplicationUser
                {
                    UserName = "GuestAtFrontDoor@mailinator.com",
                    Email = "GuestAtFrontDoor@mailinator.com",
                    FirstName = "GuestAtFrontDoor",
                    LastName = "AtFrontDoor",
                    DisplayName = "GuestAtFrontDoor",
                    HouseholdId = houseId,
                }, "Abc&123");
            }
            userId = userManager.FindByEmail("GuestAtFrontDoor@mailinator.com").Id;
            userManager.AddToRole(userId, "GuestAtFrontDoor");
			#endregion

			#region
			//context.BudgetItemTypes.AddOrUpdate(i => i.ItemCategory,
			//new BudgetItemType { ItemCategory = "Auto", ItemType = "Gas" },
			//new BudgetItemType { ItemCategory = "Auto", ItemType = "Service" },
			//new BudgetItemType { ItemCategory = "Auto", ItemType = "Payment" },
			//new BudgetItemType { ItemCategory = "Auto", ItemType = "Insurance" },

			//new BudgetItemType { ItemCategory = "Utilities", ItemType = "Cable" },
			//new BudgetItemType { ItemCategory = "Utilities", ItemType = "Internet" },
			//new BudgetItemType { ItemCategory = "Utilities", ItemType = "Mobile Phone" },
			//new BudgetItemType { ItemCategory = "Utilities", ItemType = "Electricity" },
			//new BudgetItemType { ItemCategory = "Utilities", ItemType = "Gas" },
			//new BudgetItemType { ItemCategory = "Utilities", ItemType = "Water" },

			//new BudgetItemType { ItemCategory = "Entertainment", ItemType = "Movies/DVDS" },
			//new BudgetItemType { ItemCategory = "Entertainment", ItemType = "Music/Concerts" },
			//new BudgetItemType { ItemCategory = "Entertainment", ItemType = "Newspaper/Magazines" },

			//new BudgetItemType { ItemCategory = "Food & Dining", ItemType = "Alcohol/Bars" },
			//new BudgetItemType { ItemCategory = "Food & Dining", ItemType = "Coffee Shops" },
			//new BudgetItemType { ItemCategory = "Food & Dining", ItemType = "Dining Out" },
			//new BudgetItemType { ItemCategory = "Food & Dining", ItemType = "Groceries" },

			//new BudgetItemType { ItemCategory = "Health & Fitness", ItemType = "Dentist" },
			//new BudgetItemType { ItemCategory = "Health & Fitness", ItemType = "Doctor" },
			//new BudgetItemType { ItemCategory = "Health & Fitness", ItemType = "Eyecare" },
			//new BudgetItemType { ItemCategory = "Health & Fitness", ItemType = "Gym" },
			//new BudgetItemType { ItemCategory = "Health & Fitness", ItemType = "Pharmacy" },
			//new BudgetItemType { ItemCategory = "Health & Fitness", ItemType = "Health Insurance" },

			//new BudgetItemType { ItemCategory = "Home", ItemType = "Furnishings" },
			//new BudgetItemType { ItemCategory = "Home", ItemType = "Home Improvement" },
			//new BudgetItemType { ItemCategory = "Home", ItemType = "Lawn & Garden" },
			//new BudgetItemType { ItemCategory = "Home", ItemType = "Mortgage/Rent" },
			//new BudgetItemType { ItemCategory = "Home", ItemType = "Home Supplies" },

			//new BudgetItemType { ItemCategory = "Income", ItemType = "Paycheck" },
			//new BudgetItemType { ItemCategory = "Income", ItemType = "Bonus" },
			//new BudgetItemType { ItemCategory = "Income", ItemType = "Returned Purchase" },

			//new BudgetItemType { ItemCategory = "Personal Care", ItemType = "Hair" },
			//new BudgetItemType { ItemCategory = "Personal Care", ItemType = "Spa" },
			//new BudgetItemType { ItemCategory = "Personal Care", ItemType = "Products" },

			//new BudgetItemType { ItemCategory = "Pets", ItemType = "Pet Food & Supplies" },
			//new BudgetItemType { ItemCategory = "Pets", ItemType = "Pet Grooming" },
			//new BudgetItemType { ItemCategory = "Pets", ItemType = "Veterinary" },

			//new BudgetItemType { ItemCategory = "Shopping", ItemType = "Clothing" },
			//new BudgetItemType { ItemCategory = "Shopping", ItemType = "Shoes" },

			//new BudgetItemType { ItemCategory = "Travel", ItemType = "Air Travel" },
			//new BudgetItemType { ItemCategory = "Travel", ItemType = "Hotel" },
			//new BudgetItemType { ItemCategory = "Travel", ItemType = "Rental Car/Taxi" },
			//new BudgetItemType { ItemCategory = "Travel", ItemType = "Vacation" }

			//);
			#endregion
		}


	}
}
