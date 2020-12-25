using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DictionaryEnglishRus.Model;
using System.Windows.Media;

namespace DictionaryEnglishRus.ViewModel
{
    public class ViewModelTrie : BindableBase
    {
        public ObservableCollection<OpHistoryItem> OpHistory { get; set; }
        public DelegateCommand Delete { get; set; }
        public string DeleteValue { get; set; }
        public DelegateCommand Translate { get; set; }
        public string TranslateValue { get; set; }
        public DelegateCommand Add { get; set; }
        public string AddValueF { get; set; }
        public string AddValueS { get; set; }
        public Trie<string> Tree { get; set; }


        public ViewModelTrie()
        {
            var tupleTreeAndTime = new DictionaryWords("../../Model/Words.txt").CreateTree();
            Tree = tupleTreeAndTime.Item1;
            OpHistory = new ObservableCollection<OpHistoryItem>();
            var massiveRussian = new MassiveChars("Russian").GetMassiveChars();
            var massiveEnglish = new MassiveChars("English").GetMassiveChars();
            OpHistory.Add(new OpHistoryItem("Время создания дерева из " + tupleTreeAndTime.Item3+ " слов - "+tupleTreeAndTime.Item2 + " ms."));

            Add = new DelegateCommand(() =>
            {
                var beep = new MediaPlayer();
                beep.Open(new Uri("../../Music/beep.mp3", UriKind.RelativeOrAbsolute));
                beep.Play();
                if (string.IsNullOrWhiteSpace(AddValueF) && !string.IsNullOrWhiteSpace(AddValueS))
                    OpHistory.Add(new OpHistoryItem("Вы не ввели слово на английском, которое хотели добавить!"));
                else if (!string.IsNullOrWhiteSpace(AddValueF) && string.IsNullOrWhiteSpace(AddValueS))
                    OpHistory.Add(new OpHistoryItem("Вы не ввели перевод слова!"));
                else if (string.IsNullOrWhiteSpace(AddValueF) && string.IsNullOrWhiteSpace(AddValueS))
                    OpHistory.Add(new OpHistoryItem("Вы ничего не ввели!"));
                else
                {
                    var spForAdd = new Stopwatch();
                    if (CheckerForEnglishAndRussian(AddValueF.ToCharArray(), AddValueS.ToCharArray()))
                    {
                        long time = 0;
                        if (!Tree.ContainsValue(AddValueF))
                        {
                            spForAdd.Start();
                            Tree.Add(AddValueF, AddValueS);
                            spForAdd.Stop();
                            OpHistory.Add(new OpHistoryItem("Слово \"" + AddValueF + "\" записано в словарь."));
                        }
                        else
                        {
                            spForAdd.Start();
                            Tree.Remove(AddValueF);
                            Tree.Add(AddValueF, AddValueS);
                            spForAdd.Stop();
                            OpHistory.Add(new OpHistoryItem("Слово \"" + AddValueF + "\" перезаписано."));
                        }
                        OpHistory.Add(new OpHistoryItem("Время добавления/переписывания составило - " + spForAdd.ElapsedTicks + " тиков."));
                        RaisePropertyChanged("OpHistory");
                    }
                    else OpHistory.Add(new OpHistoryItem("Первое слово должно быть Английским, а второе Русским!"));
                }
            });
            Delete = new DelegateCommand(() =>
            {
                var beep = new MediaPlayer();
                beep.Open(new Uri("../../Music/beep.mp3", UriKind.RelativeOrAbsolute));
                beep.Play();
                if (string.IsNullOrWhiteSpace(DeleteValue))
                    OpHistory.Add(new OpHistoryItem("Вы ничего не ввели!"));
                else
                {
                    if (CheckerForEnglish(DeleteValue.ToCharArray()))
                    {
                        if (Tree.ContainsValue(DeleteValue))
                        {
                            var spForDelete = new Stopwatch();
                            spForDelete.Start();
                            Tree.Remove(DeleteValue);
                            spForDelete.Stop();
                            OpHistory.Add(new OpHistoryItem("Слово \"" + DeleteValue + "\" удалено из словаря."));
                            OpHistory.Add(new OpHistoryItem("Время удаления - " + spForDelete.ElapsedTicks + " тиков"));
                        }
                        else
                        {
                            OpHistory.Add(new OpHistoryItem("Слово \"" + DeleteValue + "\" не найдено в словаре."));
                        }
                        RaisePropertyChanged("OpHistory");
                    }
                    else OpHistory.Add(new OpHistoryItem("Это не английское слово \"" + DeleteValue + "\"."));
                }
            });
            Translate = new DelegateCommand(() =>
            {
                var beep = new MediaPlayer();
                beep.Open(new Uri("../../Music/beep.mp3", UriKind.RelativeOrAbsolute));
                beep.Play();
                if (string.IsNullOrWhiteSpace(TranslateValue))
                    OpHistory.Add(new OpHistoryItem("Вы ничего не ввели!"));
                else
                {                   
                    if (CheckerForEnglish(TranslateValue.ToCharArray()))
                    {
                        if (Tree.ContainsValue(TranslateValue))
                        {
                            var spForTranslate = new Stopwatch();
                            spForTranslate.Start();
                            var russianTranslate = Tree.GetValue(TranslateValue);
                            spForTranslate.Stop();
                            OpHistory.Add(new OpHistoryItem("Перевод слова " + TranslateValue + " - " + russianTranslate));
                            OpHistory.Add(new OpHistoryItem("Время перевода - " + spForTranslate.ElapsedTicks + " тиков."));
                        }
                        else
                        {
                            OpHistory.Add(new OpHistoryItem("Слово \"" + TranslateValue + "\" не найдено в словаре."));
                        }
                        RaisePropertyChanged("OpHistory");
                    }
                    else OpHistory.Add(new OpHistoryItem("Это не английское слово \"" + TranslateValue + "\"."));
                }
            });
            Tree.GetValue("apple");
            Tree.Remove("warm");
            Tree.Remove("yourself");
            Tree.Add("Test", "Тест");
        }

        private bool CheckerForEnglish(char[] valueSplit)
        {
            var massiveEnglish = new MassiveChars("English").GetMassiveChars();
            var flag = true;
            for (int i = 0; i < valueSplit.Count(); i++)
                if (!massiveEnglish.Contains(valueSplit[i]) && valueSplit[i]!=' ')
                    flag = false;
            return flag;
        }

        private bool CheckerForEnglishAndRussian(char[] firstValueSplit, char[] secondValueSplit)
        {
            var massiveRussian = new MassiveChars("Russian").GetMassiveChars();
            var massiveEnglish = new MassiveChars("English").GetMassiveChars();
            var flag = true;
            for (int i = 0; i < firstValueSplit.Count(); i++)
                if (!massiveEnglish.Contains(firstValueSplit[i]) && firstValueSplit[i] != ' ')
                    flag = false;
            for (int i = 0; i < secondValueSplit.Count(); i++)
                if (!massiveRussian.Contains(secondValueSplit[i]) && secondValueSplit[i] != ' ')
                    flag = false;
            return flag;
        }
    }

    public class OpHistoryItem
    {
        public string Value { get; set; }

        public OpHistoryItem(string str)
        {
            Value = str;
        }
    }

    public class MassiveChars
    {
        public string TypeAlphabet { get; set; }

        public MassiveChars(string typeMassive)
        {
            TypeAlphabet = typeMassive;
        }

        public char[] GetMassiveChars()
        {
            switch (TypeAlphabet)
            {
                case "Russian":
                    char[] russinList = new char[32];
                    char ch;
                    int n = 0;
                    for (int i = 1072; i <= 1103; i++)
                    {
                        ch = System.Convert.ToChar(i);
                        russinList[n] = ch;
                        n++;
                    }
                    return russinList;
                case "English":
                    return Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c).ToArray();
                default: throw new Exception("Такого языка нет или ещё не добавлен");
            }
        }
    }

    public class DictionaryWords
    {
        public string PathToWords { get; set; }

        public DictionaryWords(string pathToWords)
        {
            PathToWords = pathToWords;
        }

        public Tuple<Trie<string>, long, int> CreateTree()
        {
            var tree = new Trie<string>();
            using (var streamReader = new StreamReader(PathToWords, Encoding.Default))
            {
                string line;
                int countWords = 0;
                var allWords = new List<string>();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while ((line = streamReader.ReadLine()) != null)
                {
                    allWords.Add(line);
                    countWords++;
                }

                for (int i = 0; i < countWords; i += 2)
                {
                    tree.Add(allWords[i], allWords[i + 1]);;
                }

                stopwatch.Stop();
                return Tuple.Create(tree, stopwatch.ElapsedMilliseconds,countWords/2);
            }
        }
    }
}