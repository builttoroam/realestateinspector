using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateInspector.Core.ViewModels;
using Xamarin.Forms;

namespace RealEstateInspector.XForms
{
    public partial class MainPage : ContentPage
    {
        public static event EventHandler AuthenticateRequested;
        public MainPage()
        {
            InitializeComponent();
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    //var vm = new MainViewModel();
        //    //BindingContext = vm;
        //   // await vm.LoadPropertyData();
        //}

        public void AuthenticateClick(object sender, EventArgs e)
        {
            if (AuthenticateRequested != null)
            {
                AuthenticateRequested(this, EventArgs.Empty);
            }
        }

        //public void OpenFile()
        //{
        //    string filePath = Path.Combine(
        //Environment.GetFolderPath(Environment.SpecialFolder.Personal),
        //"MyFile.txt");
        //    System.IO.File.WriteAllText(filePath, "Contents of text file");
        //    Console.WriteLine(System.IO.ReadAllText(filePath));
        //}
    }
}
