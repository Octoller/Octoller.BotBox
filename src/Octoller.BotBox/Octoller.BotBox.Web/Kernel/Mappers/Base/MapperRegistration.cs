using AutoMapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Octoller.BotBox.Web.Kernel.Mappers.Base
{
    public static class MapperRegistration
    {
        public static MapperConfiguration GetMapperConfiguration()
        {
            var profiles = GetProfiles();

            return new MapperConfiguration(config =>
            {
                foreach (var profile in profiles.Select(profile => (Profile)Activator.CreateInstance(profile)))
                {
                    config.AddProfile(profile);
                }
            });
        }

        private static List<Type> GetProfiles()
        {
            return (from t in typeof(Startup).GetTypeInfo().Assembly.GetTypes()
                    where typeof(IAutomapper).IsAssignableTo(t) && !t.GetTypeInfo().IsAbstract
                    select t).ToList(); 
        }
    }
}
