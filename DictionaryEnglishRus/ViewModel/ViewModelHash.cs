using DictionaryEnglishRus.Model;
using DictionaryEnglishRus.View;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using static DictionaryEnglishRus.Model.HashTable;

namespace DictionaryEnglishRus.ViewModel
{
    class ViewModelHash : BindableBase
    {
        public MediaPlayer mp = new MediaPlayer();
        public DelegateCommand PlayMusic { get; set; }
        public ObservableCollection<OpHistoryItem> OpHistory { get; set; }
        public DelegateCommand Delete { get; set; }
        public string DeleteValue { get; set; }
        public DelegateCommand Translate { get; set; }
        public string TranslateValue { get; set; }
        public DelegateCommand Add { get; set; }
        public string AddValueF { get; set; }
        public string AddValueS { get; set; }
        public Model.HashTable HashTable { get; set; }

        public ViewModelHash()
        {
            HashTable = new MassiveWords().GetHashTableAndTime("../../Model/Words.txt").Item1;
            var time = new MassiveWords().GetHashTableAndTime("../../Model/Words.txt").Item2;
            OpHistory = new ObservableCollection<OpHistoryItem>();
            var massiveRussian = new MassiveChars("Russian").GetMassiveChars();
            var massiveEnglish = new MassiveChars("English").GetMassiveChars();
            OpHistory.Add(new OpHistoryItem("Время создания хэш таблицы из " + HashTable.Values.Count() + " слов - " + time + " ms."));
            mp.Open(new Uri("../../Model/music.mp3", UriKind.RelativeOrAbsolute));
            var flag = false;
            Add = new DelegateCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(AddValueF) && !string.IsNullOrWhiteSpace(AddValueS))
                    OpHistory.Add(new OpHistoryItem("Вы не ввели слово на английском, которое хотели добавить!"));
                else if (!string.IsNullOrWhiteSpace(AddValueF) && string.IsNullOrWhiteSpace(AddValueS))
                    OpHistory.Add(new OpHistoryItem("Вы не ввели перевод слова!"));
                else if (string.IsNullOrWhiteSpace(AddValueF) && string.IsNullOrWhiteSpace(AddValueS))
                    OpHistory.Add(new OpHistoryItem("Вы ничего не ввели!"));
                else
                {
                    if (CheckerForEnglishAndRussian(AddValueF.ToCharArray(), AddValueS.ToCharArray()))
                    {
                        var spForAdd = new Stopwatch();
                        if (!HashTable.Search(AddValueF))
                        {
                            spForAdd.Start();
                            HashTable.Insert(new Word(AddValueF, AddValueS));
                            spForAdd.Stop();
                            OpHistory.Add(new OpHistoryItem("Слово \"" + AddValueF + "\" записано в словарь."));
                        }
                        else
                        {
                            spForAdd.Start();
                            HashTable.Remove(AddValueF);
                            HashTable.Insert(new Word(AddValueF, AddValueS));
                            spForAdd.Stop();
                            OpHistory.Add(new OpHistoryItem("Слово \"" + AddValueF + "\" перезаписано."));
                        }
                        OpHistory.Add(new OpHistoryItem("Время добавление/переписывание составило - " + spForAdd.ElapsedTicks + " тиков."));
                        RaisePropertyChanged("OpHistory");
                    }
                    else OpHistory.Add(new OpHistoryItem("Первое слово должно быть Английским, а второе Русским!"));
                }
            });
            Delete = new DelegateCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(DeleteValue))
                    OpHistory.Add(new OpHistoryItem("Вы ничего не ввели!"));
                else
                {
                    if (CheckerForEnglish(DeleteValue.ToCharArray()))
                    {
                        if (HashTable.Search(DeleteValue))
                        {
                            var spForDelete = new Stopwatch();
                            spForDelete.Start();
                            HashTable.Remove(DeleteValue);
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
                if (string.IsNullOrWhiteSpace(TranslateValue))
                    OpHistory.Add(new OpHistoryItem("Вы ничего не ввели!"));
                else
                {
                    if (CheckerForEnglish(TranslateValue.ToCharArray()))
                    {

                        if (HashTable.Search(TranslateValue))
                        {
                            Word w;
                            var spForTranslate = new Stopwatch();
                            spForTranslate.Start();
                            var russianTranslate = HashTable.Search(TranslateValue, out w);
                            spForTranslate.Stop();
                            OpHistory.Add(new OpHistoryItem("Перевод слова " + TranslateValue + " - " + w.Value));
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
            PlayMusic = new DelegateCommand(() =>
            {
                if (!flag)
                {
                    mp.Play();
                    flag = true;
                }
                else
                {
                    mp.Pause();
                    flag = false;
                }
            });
            Word wo;
            HashTable.Search("apple", out wo);
            HashTable.Search("apple", out wo);
            HashTable.Remove("warm");
            HashTable.Remove("yourself");
            HashTable.Insert(new Word("Test", "Тест"));
        }

        private bool CheckerForEnglish(char[] valueSplit)
        {
            var massiveEnglish = new MassiveChars("English").GetMassiveChars();
            var flag = true;
            for (int i = 0; i < valueSplit.Count(); i++)
                if (!massiveEnglish.Contains(valueSplit[i]) && valueSplit[i] != ' ')
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


    public class MassiveWords
    {
        public Tuple<Model.HashTable, long> GetHashTableAndTime(string path)
        {
            using (var streamReader = new StreamReader(path, Encoding.Default))
            {
                string line;
                int countWords = 0;
                var allWords = new List<string>();
                Stopwatch stopwatch = new Stopwatch();
                while ((line = streamReader.ReadLine()) != null)
                {
                    allWords.Add(line);
                    countWords++;
                }
                var hashTable = new Model.HashTable(countWords);
                stopwatch.Start();
                for (int i = 0; i < countWords; i += 2)
                {
                    hashTable.Insert(new Word(allWords[i], allWords[i + 1]));
                }
                stopwatch.Stop();
                return Tuple.Create(hashTable, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
