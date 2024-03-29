﻿using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace SkillHaven.Application.Configurations
{
    public class Localizer : IStringLocalizer
    {
        private ResourceManager _resourceManager;

       

        public LocalizedString this[string name]
        {
            get
            {
                var value = _resourceManager.GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, string resourceFile]
        {
            get
            {
                if (_resourceManager == null)
                {
                    _resourceManager = GetAssemblyFromName(resourceFile);
                }
                var value = _resourceManager.GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {

                if (_resourceManager == null)
                {
                    _resourceManager = GetAssemblyFromName(arguments[0].ToString());
                }
                var value = _resourceManager.GetString(name, CultureInfo.CurrentCulture);
                if (arguments.Length > 1)
                {
                    try
                    {
                        value = string.Format(value, arguments?.Skip(1).ToArray());
                    }
                    catch(Exception e) 
                    {
                        value = string.Format(value, arguments? .ToArray());

                    }
                }
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        private ResourceManager GetAssemblyFromName(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resourceAssembly = assembly.GetManifestResourceNames();
            var targerResource = resourceAssembly.FirstOrDefault(x => x.Contains(resourceName) || x.Equals(resourceName+".resx"));

            if (targerResource != null)
            {
                return new ResourceManager(targerResource.Replace(".resources", string.Empty), assembly);
            }

            return null;
        }

    }

}
