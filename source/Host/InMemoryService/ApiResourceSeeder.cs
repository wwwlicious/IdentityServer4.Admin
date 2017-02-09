namespace IdentityAdmin.Host.InMemoryService
{
    using System;
    using System.Collections.Generic;

    public class ApiResourceSeeder
    {
        public static ICollection<InMemoryApiResource> Get(int random = 0)
        {
            var apiResources = new HashSet<InMemoryApiResource>
            {
                new InMemoryApiResource
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "They run the show"
                },
                new InMemoryApiResource
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "They pay the bills"
                }
            };

            for (var i = 0; i < random; i++)
            {
                apiResources.Add(new InMemoryApiResource
                {
                    Id = apiResources.Count + 1,
                    Name = GenName().ToLower(),
                    Description = GenName().ToLower()
                });                
            }
            return apiResources;
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