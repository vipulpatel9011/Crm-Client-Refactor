
namespace Aurea.CRM.Services.Delegates
{
    using System.Collections.Generic;
    using Aurea.CRM.Services.ModelControllers;

    /// <summary>
    /// Delegate interface for the page model controller
    /// </summary>
    public interface IPageModelControllerDelegate
    {
        /// <summary>
        /// Performed when the model controller view will disappear.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        void PageModelControllerViewWillDisappear(UPPageModelController pageModelController);

        /// <summary>
        /// Performed when the model controller view will appear.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        void PageModelControllerViewWillAppear(UPPageModelController pageModelController);

        /// <summary>
        /// Performed when the model controller set context value for key.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        void PageModelControllerSetContextValueForKey(UPPageModelController pageModelController, object value, string key);

        /// <summary>
        /// Provides context value for key.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        object PageModelControllerContextValueForKey(UPPageModelController pageModelController, string key);

        /// <summary>
        /// Provides contexts value dictionary for page model controller.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <returns></returns>
        Dictionary<string, object> ContextValueDictionaryForPageModelController(UPPageModelController pageModelController);
    }
}
