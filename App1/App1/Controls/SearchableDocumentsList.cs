using Xamarin.Forms.Internals;
//using EssentialUIKit.Models.Navigation;
using App1.Models;

namespace App1.Controls
{
    /// <summary>
    /// This class extends the behavior of the SearchableListView control to filter the ListViewItem based on text.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class SearchableDocumentsList : SearchableListView
    {
        #region Method

        /// <summary>
        /// Filtering the list view items based on the search text.
        /// </summary>
        /// <param name="obj">The list view item</param>
        /// <returns>Returns the filtered item</returns>
        public override bool FilterContacts(object obj)
        {
            if (base.FilterContacts(obj))
            {
                var taskInfo = obj as Document;
                if (taskInfo == null || string.IsNullOrEmpty(taskInfo.name) || string.IsNullOrEmpty(taskInfo.size) || string.IsNullOrEmpty(taskInfo.date))
                {
                    return false;
                }

                return taskInfo.name.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant()) ||
                    taskInfo.size.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant()) || taskInfo.date.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant());
            }

            return false;
        }

        #endregion
    }
}