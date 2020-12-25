using System;
using System.Windows;

namespace DictionaryEnglishRus.View
{
    /// <summary>
    /// Логика взаимодействия для HashTable.xaml
    /// </summary>
    public partial class HashTable : Window
    {
        public HashTable()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mw = new MainWindow();
            this.Close();
            mw.Show();
        }
    }
}
