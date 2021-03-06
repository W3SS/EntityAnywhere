﻿using Rhyous.WebFramework.Interfaces;
using System;
using System.Collections.Generic;

namespace Rhyous.WebFramework.Behaviors
{
    public class CustomRestDictionary : Dictionary<string, Func<Type, string, string>>, IDictionaryDefaultValueProvider<string, Func<Type, string, string>>
    {
        #region Singleton

        private static readonly Lazy<CustomRestDictionary> Lazy = new Lazy<CustomRestDictionary>(() => new CustomRestDictionary());

        public static CustomRestDictionary Instance { get { return Lazy.Value; } }

        internal CustomRestDictionary()
        {
            Add("GetByE1Ids", (entityType, template) => {
                var attribute = entityType.GetAttribute<MappingEntityAttribute>();
                string pluralEntityName = PluralizationDictionary.Instance.GetValueOrDefault(entityType.Name);
                var entity1Pluralized = string.IsNullOrWhiteSpace(attribute.Entity1UriTemplate) ? entityType.GetMappedEntity1Pluralized() : attribute.Entity1UriTemplate;
                return string.Format(RestDictionary.Instance[template], pluralEntityName, entity1Pluralized);
            });
            Add("GetByE2Ids", (entityType, template) => {
                var attribute = entityType.GetAttribute<MappingEntityAttribute>();
                string pluralEntityName = PluralizationDictionary.Instance.GetValueOrDefault(entityType.Name);
                var entity2Pluralized = string.IsNullOrWhiteSpace(attribute.Entity2UriTemplate) ? entityType.GetMappedEntity2Pluralized() : attribute.Entity2UriTemplate;
                return string.Format(RestDictionary.Instance[template], pluralEntityName, entity2Pluralized);
            });
        }
        #endregion

        public Func<Type, string, string> DefaultValue => (entityType, template) => {
            string pluralEntityName = PluralizationDictionary.Instance.GetValueOrDefault(entityType.Name);
            return string.Format(RestDictionary.Instance[template], pluralEntityName);

        };

        public Func<Type, string, string> DefaultValueProvider(string key)
        {
            return DefaultValue;
        }
    }
}
