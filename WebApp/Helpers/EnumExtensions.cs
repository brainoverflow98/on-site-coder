using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace WebApp.Helpers
{
    public static class EnumExtensions
    {        
        public static IEnumerable<SelectListItem> ToSelectList(this Type enumType, SelectListOptions options = null)
        {
            if (options == null)
                options = new SelectListOptions();

            var selectList = new List<SelectListItem>();

            var categories = enumType.GetCategories(options.DisabledGroups);

            if (options.Placeholder != null)
                selectList.Add(new SelectListItem(options.Placeholder, ""));

            var values = Enum.GetValues(enumType);
            foreach(var value in values)
            {
                bool selected = (options.SelectedValues.Any(v => v == value));
                bool disabled = (options.DisabledValues.Any(v => v == value));
                var group = categories.FirstOrDefault(c => c.Name == ((Enum)value).GetCategoryName());
                string listValues = options.IsStringValue ? ((Enum)value).ToString() : ((int)value).ToString();

                var item = new SelectListItem(((Enum)value).GetDisplayName(), listValues, selected, disabled);
                item.Group = group;

                selectList.Add(item);                
            }

            return selectList;
        }

        public static string GetDisplayName(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayNameAttribute[] attributes = fi.GetCustomAttributes(typeof(DisplayNameAttribute), false) as DisplayNameAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().DisplayName;
            }

            return value.ToString();
        }

        public static string GetCategoryName(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            CategoryAttribute[] attributes = fi.GetCustomAttributes(typeof(CategoryAttribute), false) as CategoryAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Category;
            }

            return "";
        }

        private static IEnumerable<SelectListGroup> GetCategories(this Type enumType, string[] DisabledGroups)
        {
            var categories = new List<SelectListGroup>();
            var fields = enumType.GetFields();

            foreach (var fi in fields)
            {
                CategoryAttribute[] attributes = fi.GetCustomAttributes(typeof(CategoryAttribute), false) as CategoryAttribute[];
                if (attributes != null && attributes.Any())
                {
                    string categoryName =  attributes.First().Category;
                    bool exists = categories.Any(c => c.Name == categoryName);
                    if (!exists)
                    {
                        bool disabled = DisabledGroups.Contains(categoryName);
                        categories.Add(new SelectListGroup { Name = categoryName, Disabled = disabled });
                    }                        
                }
            }           

            return categories;
        }              
    }

    public class SelectListOptions
    {
        public SelectListOptions()
        {
            SelectedValues = new object[] { };
            DisabledValues = new object[] { };
            DisabledGroups = new string[] { };
        }

        public object[] SelectedValues { get; set; }
        public object[] DisabledValues { get; set; }
        public string[] DisabledGroups { get; set; }
        public string Placeholder { get; set; }
        public bool IsStringValue { get; set; }
    }
}
