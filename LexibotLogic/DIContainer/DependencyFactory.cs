using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LexibotData.Models;
using LexibotData.DataAccess;
using LexibotLogic.Reddit;
using LexibotLogic.Parsers;

namespace LexibotLogic.DIContainer
{
    public class DependencyFactory
    {
        private static IUnityContainer _container;

        /// <summary>
        /// Public reference to the unity container which will 
        /// allow the ability to register instrances or take 
        /// other actions on the container.
        /// </summary>
        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }
            private set
            {
                _container = value;
            }
        }

        /// <summary>
        /// Static constructor for DependencyFactory which will 
        /// initialize the unity container.
        /// </summary>
        static DependencyFactory()
        {
            _container = new UnityContainer();
            LoadDependencies();
        }

        /// <summary>
        /// Load classes and interfaces into the unity container.
        /// </summary>
        static void LoadDependencies()
        {

            _container.RegisterType<IEntityDbManager, EntityDbManager>(new ContainerControlledLifetimeManager())
                      .RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager())
                      .RegisterType<IRedditBot, RedditBot>(new ContainerControlledLifetimeManager())
                      .RegisterType<IWordParser, WordParser>(new TransientLifetimeManager());
        }

        /// <summary>
        /// Resolves the type parameter T to an instance of the appropriate type.
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        public static T Resolve<T>()
        {
            T ret = default(T);
 
            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>();
            }
 
            return ret;
        }
    }
}
