using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mitten.Mobile.ViewModels;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Contains a collection of view controller types and their related view models.
    /// </summary>
    public class ViewControllerTypes
    {
        private readonly IDictionary<string, Type> viewControllers;

        /// <summary>
        /// Initializes a new instance of the ViewControllerTypes class.
        /// </summary>
        public ViewControllerTypes()
            : this(new Dictionary<string, Type>())
        {
        }

        private ViewControllerTypes(IDictionary<string, Type> viewControllers)
        {
            this.viewControllers = viewControllers;
        }

        /// <summary>
        /// Gets the view controller type associated with the specified view model.
        /// </summary>
        /// <returns>The view controller type.</returns>
        public Type GetViewControllerType<TViewModel>()
        {
            string viewModelName = this.GetViewModelName(typeof(TViewModel));
            if (!this.viewControllers.ContainsKey(viewModelName))
            {
                throw new ViewControllerNotFoundException(typeof(TViewModel));
            }

            return this.viewControllers[viewModelName];
        }

        /// <summary>
        /// Registers a view controller and model with the collection.
        /// </summary>
        public void Register<TViewModel, TViewController>()
            where TViewModel : ViewModel
            where TViewController : UIViewController<TViewModel>
        {
            string viewModelName = this.GetViewModelName(typeof(TViewModel));
            if (this.viewControllers.ContainsKey(viewModelName))
            {
                throw new ArgumentException("The ViewModel Type (" + viewModelName + ") is already registered.");
            }

            this.viewControllers.Add(typeof(TViewModel).FullName, typeof(TViewController));
        }

        /// <summary>
        /// Creates a new ViewControllerTypes loaded with view models and controllers in the specified assembly.
        /// </summary>
        /// <param name="assembly">An assembly to scan.</param>
        /// <returns>A new instance of the ViewControllerTypes class.</returns>
        public static ViewControllerTypes FromAssembly(Assembly assembly)
        {
            Dictionary<string, Type> controllers = new Dictionary<string, Type>();

            foreach (Type typeToCheck in assembly.GetTypes())
            {
                if (!typeToCheck.IsAbstract)
                {
                    Type baseType = typeToCheck;
                    while (baseType != null)
                    {
                        baseType = baseType.BaseType;

                        if (baseType != null &&
                            baseType.IsGenericType &&
                            baseType.GetGenericTypeDefinition() == typeof(UIViewController<>))
                        {
                            string viewModelName = baseType.GetGenericArguments().Single().FullName;
                            if (controllers.ContainsKey(viewModelName))
                            {
                                throw new ArgumentException("A view model with name (" + viewModelName + ") already exists.");
                            }

                            controllers.Add(viewModelName, typeToCheck);
                            break;
                        }
                    }
                }
            }

            return new ViewControllerTypes(controllers);
        }

        private string GetViewModelName(Type viewModelType)
        {
            return viewModelType.FullName;
        }
    }
}
