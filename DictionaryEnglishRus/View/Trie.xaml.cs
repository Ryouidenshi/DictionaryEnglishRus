﻿using System;
using System.Windows;
using System.Windows.Media;

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
            var beep = new MediaPlayer();
            beep.Open(new Uri("../../Music/beep.mp3", UriKind.RelativeOrAbsolute));
            beep.Play();
            var mw = new MainWindow();
            this.Close();
            mw.Show();
        }
    }
}
