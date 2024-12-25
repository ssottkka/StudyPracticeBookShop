using BookShopYP02.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookShopYP02
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BookShopEntities1 db= new BookShopEntities1();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = db.Покупатели.FirstOrDefault(x => x.Логин == loginbox.Text && x.Пароль ==  passwordbox.Text);
            if (user != null)
            {
                User.Catalog catalog = new User.Catalog(user.Код);
                catalog.Show();
                this.Close();
            }
            var manager = db.Сотрудники.FirstOrDefault(x => x.Логин == loginbox.Text && x.Пароль == passwordbox.Text && x.КодДолжности == 1);
            if(manager != null)
            {
                Manager.ManagerWindow managerWindow = new Manager.ManagerWindow();
                managerWindow.Show();   
                this.Close();   

            }
        }

        private void btnViewProducts_Click(object sender, RoutedEventArgs e)
        {
            UnatorizationProductsView unatorizationProductsView = new UnatorizationProductsView();  
            unatorizationProductsView.Show();   
            this.Close();   
        }
    }
}
