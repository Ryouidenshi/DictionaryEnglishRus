using System.Windows;


namespace DictionaryEnglishRus.View
{
    public partial class MainWindow : Window
    {
        public string answer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Answer(object sender, RoutedEventArgs e)
        {
            if (answer == null)
                ans.Visibility = Visibility.Hidden;
            if (answer == "first")
            {
                var trie = new Trie();
                trie.Show();
                this.Close();
            }
            else if (answer == "second")
            {
                var hashTable = new View.HashTable();
                hashTable.Show();
                this.Close();
            }
        }

        private void First(object sender, RoutedEventArgs e)
        {
            ans.Visibility = Visibility.Visible;
            answer = "first";
            second.Text = "Second method (HashTable)";
            first.Text = "KREVETKA";
        }

        private void Second(object sender, RoutedEventArgs e)
        {
            ans.Visibility = Visibility.Visible;
            answer = "second";
            first.Text = "First method (Trie)";
            second.Text = "DEREZHABL";
        }
    }
}
