using System.Windows;

namespace MeerkeuzeUI
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();


        }

        private void BtnDocent_Click(object sender, RoutedEventArgs e)
        {

            DocentenWindow docentenWindow = new DocentenWindow();



            Application.Current.MainWindow = docentenWindow;


            docentenWindow.Show();
            this.Close();
        }

        private void BtnStudent_Click(object sender, RoutedEventArgs e)
        {

            StudentWindow studentWindow = new StudentWindow();
            Application.Current.MainWindow = studentWindow;
            studentWindow.Show();
            this.Close();

        }
    }
}