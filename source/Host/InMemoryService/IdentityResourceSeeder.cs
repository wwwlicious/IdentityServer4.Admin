namespace IdentityAdmin.Host.InMemoryService
{
    using System;
    using System.Collections.Generic;

    public class IdentityResourceSeeder
    {
        public static ICollection<InMemoryIdentityResource> Get(int random = 0)
        {
            var identityResources = new HashSet<InMemoryIdentityResource>
            {
                new InMemoryIdentityResource
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "They run the show"
                },
                new InMemoryIdentityResource
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "They pay the bills"
                }
            };

            for (var i = 0; i < random; i++)
            {
                identityResources.Add(new InMemoryIdentityResource
                {
                    Id = identityResources.Count + 1,
                    Name = GenName().ToLower(),
                    Description = GenName().ToLower()
                });
            }
            return identityResources;
        }

        private static string GenName()
        {
            var firstChar = (char)((rnd.Next(26)) + 65);
            var username = firstChar.ToString();
            for (var j = 0; j < 6; j++)
            {
                username += Char.ToLower((char)(rnd.Next(26) + 65));
            }
            return username;
        }

        static Random rnd = new Random();
    }
}