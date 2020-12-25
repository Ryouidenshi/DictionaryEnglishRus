using System;
using System.Windows;

namespace DictionaryEnglishRus.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Trie : Window
    {
        public Trie()
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
