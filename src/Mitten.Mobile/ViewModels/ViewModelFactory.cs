using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Mitten.Mobile.Application;

namespace Mitten.Mobile.ViewModels
{
    /// <summary>
    /// Factory for creating and initializing view models.
    /// </summary>
    public static class ViewModelFactory
    {
        private static readonly Dictionary<Type, ConstructorInfo> types = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// Creates a new instance of the specified view model.
        /// </summary>
        /// <param name="applicationHost">The global application host.</param>
        /// <param name="navigation">Used to handle navigation between screens.</param>
        /// <param name="parameter">An optional parameter used to initialize the view model.</param>
        public static TViewModel Create<TViewModel>(ApplicationHost applicationHost, INavigation navigation, object parameter = null)
            where TViewModel : ViewModel
        {
            TViewModel viewModel = (TViewModel)ViewModelFactory.Create(typeof(TViewModel));
            viewModel.Initialize(applicationHost, navigation, parameter);
            return viewModel;
        }

        /// <summary>
        /// Creates a new view model instance of the specified type. A view model is expected to have a single
        /// default internal constructor.
        /// </summary>
        /// <param name="type">The view model type.</param>
        /// <returns>A new view model instance.</returns>
        internal static object Create(Type type)
        {
            ConstructorInfo constructor = ViewModelFactory.GetConstructor(type);
            return constructor.Invoke(null);
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            if (ViewModelFactory.types.ContainsKey(type))
            {
                return ViewModelFactory.types[type];
            }

            TypeInfo typeInfo = type.GetTypeInfo();
            IEnumerable<ConstructorInfo> constructors = typeInfo.DeclaredConstructors.Where(constructor => !constructor.IsStatic);

            if (constructors.Count() != 1)
            {
                throw new InvalidOperationException("The view model (" + type.Name + ") is expected to only have a default constructor and it must be internal.");
            }

            ConstructorInfo constructorInfo = constructors.Single();
            if (constructorInfo.GetParameters().Any())
            {
                throw new InvalidOperationException("The view model (" + type.Name + ") is expected to only have a default constructor and it must be internal.");
            }

            if (constructorInfo.IsPublic)
            {
                throw new InvalidOperationException("The view model (" + type.Name + ") is expected to have a non-public constructor.");
            }

            ViewModelFactory.types.Add(type, constructorInfo);
            return constructorInfo;
        }
    }
}