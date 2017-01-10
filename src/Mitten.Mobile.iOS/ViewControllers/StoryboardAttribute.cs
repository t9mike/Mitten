using System;
using UIKit;

namespace Mitten.Mobile.iOS.ViewControllers
{
    /// <summary>
    /// Used to identify which storyboard a UIViewController is assigned to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class StoryboardAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the StoryboardAttribute class.
        /// </summary>
        /// <param name="storyboardName">The name of the storyboard.</param>
        public StoryboardAttribute(string storyboardName)
        {
            this.StoryboardName = storyboardName;
        }

        /// <summary>
        /// Gets the name of the storyboard.
        /// </summary>
        public string StoryboardName { get; private set; }

        /// <summary>
        /// Gets the storyboard defined by this attribute.
        /// </summary>
        /// <returns>A storyboard.</returns>
        public UIStoryboard GetStoryboard()
        {
            return UIStoryboard.FromName(this.StoryboardName, null);
        }
    }
}

