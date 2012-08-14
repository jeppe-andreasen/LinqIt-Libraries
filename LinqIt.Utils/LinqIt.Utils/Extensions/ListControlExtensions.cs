using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace LinqIt.Utils.Extensions
{
    public static class ListControlExtensions
    {
        public static bool SelectByValue(this ListControl control, object value)
        {
            if (value == null)
                return control.Items.Count == 0;
            
            var item = control.Items.FindByValue(value.ToString());
            if (item == null)
                return false;

            control.SelectedIndex = control.Items.IndexOf(item);
            return true;
        }

        public static bool SelectByText(this ListControl control, string text)
        {
            if (text == null)
                return control.Items.Count == 0;

            var item = control.Items.FindByText(text);
            if (item == null)
                return false;

            control.SelectedIndex = control.Items.IndexOf(item);
            return true;
        }
        
        public static void AddDefaultItem(this ListControl control, string text = "")
        {
            control.Items.Add(new ListItem(text, ""));
        }
    }
}
