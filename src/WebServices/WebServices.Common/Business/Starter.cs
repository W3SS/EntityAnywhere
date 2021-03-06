﻿namespace Rhyous.WebFramework.WebServices
{
    /// <summary>
    /// This starts EAF.
    /// </summary>
    public class Starter
    {
        /// <summary>
        /// This starts up the custom and common entity services. Custom services are loaded first.
        /// </summary>
        public static void Start()
        {
            var loader = new EntityLoader();
            var types = loader.PluginTypes;
            CustomWebServiceLoader.LoadEntities();
            EntityLoader.LoadEntities();
        }
    }
}
