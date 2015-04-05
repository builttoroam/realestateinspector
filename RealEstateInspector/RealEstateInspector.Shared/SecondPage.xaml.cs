using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Microsoft.Practices.ServiceLocation;
using RealEstateInspector.Core;
using RealEstateInspector.Core.ViewModels;
using RealEstateInspector.Shared.Entities;
using RealEstateInspector.Shared.Client;

namespace RealEstateInspector
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondPage : Page
    {
        public SecondPage()
        {
            this.InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            (DataContext as SecondViewModel).Save();
        }
    }

    public class FormSelector : DataTemplateSelector
    {
        public DataTemplate TextBoxTemplate { get; set; }

        public DataTemplate ComboBoxTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item)
        {
            var fp = item as FormProperty;
            if (fp != null)
            {
                switch (fp.Form.Input)
                {
                    case InputType.Text:
                        return TextBoxTemplate;
                        case InputType.Dropdown:
                        return ComboBoxTemplate;
                }
            }

            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return base.SelectTemplateCore(item, container);
        }
    }


        

}
