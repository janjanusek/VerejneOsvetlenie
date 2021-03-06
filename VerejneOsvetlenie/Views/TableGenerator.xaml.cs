﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Db;
using VerejneOsvetlenieData.Data;
using VerejneOsvetlenieData.Data.Tables;

namespace VerejneOsvetlenie.Views
{
    /// <summary>
    /// Interaction logic for TableGenerator.xaml
    /// </summary>
    public partial class TableGenerator : UserControl
    {
        public PomenovanyVystup Model => DataContext as PomenovanyVystup;
        private IVystup _aktualnyVystup;
        public event EventHandler<Dictionary<string, object>> UserKlikolNaElement;
        public event EventHandler<object> UserKlikolNaElementMamId;

        public TableGenerator()
        {
            InitializeComponent();
            DataContextChanged += TableGenerator_DataContextChanged;
        }

        private void TableGenerator_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid.Columns.Clear();
            DataGrid.ItemsSource = null;
            if (_aktualnyVystup != null)
                _aktualnyVystup.VystupSpracovany -= ModelOnVystupSpracovany;
            if (Model == null && !(DataContext is IVystup))
            {
                _aktualnyVystup = null;
                return;
            }

            _aktualnyVystup = Model?.Vystup ?? DataContext as IVystup;
            _aktualnyVystup.VystupSpracovany += ModelOnVystupSpracovany;
            if (_aktualnyVystup.ParametrePreVystup == null || !_aktualnyVystup.ParametrePreVystup.Any())
                _aktualnyVystup.SpustiVystup();

            FilterInput.Children.Clear();
            if (_aktualnyVystup.ParametrePreVystup != null)
            {
                foreach (var parameter in _aktualnyVystup.ParametrePreVystup)
                {
                    var label = this.DajLabel(parameter);
                    var inputBox = this.DajInputBox(parameter);
                    FilterInput.Children.Add(label);
                    FilterInput.Children.Add(inputBox);
                }
            }
            else
                Filter.Visibility = Visibility.Hidden;
            Filter.Visibility = FilterInput.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ModelOnVystupSpracovany(object sender, EventArgs eventArgs)
        {
            if (_aktualnyVystup.Rows.Any() && _aktualnyVystup.Rows.ElementAt(0)[0].ToString().Contains("<?xml"))
            {
                PocetRiadkov.Text = 1 + "";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_aktualnyVystup.Rows.ElementAt(0)[0].ToString());
                using (XmlTextWriter writer = new XmlTextWriter("temp", null))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);
                }
                Process.Start("temp");

            }
            else
            {
                GenerujTabulku();
            }
        }

        private void GenerujTabulku()
        {
            for (int i = 0; i < _aktualnyVystup.Columns.Count; i++)
            {
                var stlpec = new DataGridTextColumn
                {
                    Header = _aktualnyVystup.Columns[i],
                    Binding = new Binding($"[{i}]")
                };
                DataGrid.Columns.Add(stlpec);
            }
            DataGrid.ItemsSource = _aktualnyVystup.Rows;
            while (DataGrid.Columns.Count - _aktualnyVystup.Columns.Count > 0)
                DataGrid.Columns.RemoveAt(DataGrid.Columns.Count - 1);
            PocetRiadkov.Text = _aktualnyVystup.Rows.Count().ToString();

            var scrollBar = FindVisualChild<ScrollViewer>(DataGrid);
            if (scrollBar != null)
                scrollBar.IsDeferredScrollingEnabled = true;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        protected virtual void OnUserKlikolNaElement(Dictionary<string, object> e)
        {
            UserKlikolNaElement?.Invoke(this, e);
        }

        private void UserKlikolNaRiadok(object sender, SelectionChangedEventArgs e)
        {
            var row = new Dictionary<string, object>();
            var values = ((DataGrid)sender).SelectedItem as List<object>;
            if (values == null)
                return;
            for (int i = 0; i < DataGrid.Columns.Count; i++)
            {
                row.Add(DataGrid.Columns[i].Header.ToString(), values[i]);
            }
            OnUserKlikolNaElement(row);
            var stlpec =
                DataGrid.Columns.FirstOrDefault(
                    c => c.Header.ToString().ToLower() == _aktualnyVystup.KlucovyStlpec?.ToLower());
            if (stlpec == null)
                return;
            var id = row.Values.ElementAt(DataGrid.Columns.IndexOf(stlpec));
            OnUserKlikolNaElementMamId(id);
        }

        private TextBox DajInputBox(ProcedureParameter paParameter, SqlClassAttribute paAttribut = null)
        {
            var box = new TextBox
            {
                Tag = paParameter,
                Margin = new Thickness(5, 0, 5, 0),
                FontSize = 20,
                MinWidth = 100,
                MaxLength = (paAttribut?.Length ?? 0) != 0 ? paAttribut.Length : 50
            };
            box.SetBinding(TextBox.TextProperty, new Binding()
            {
                Path = new PropertyPath(nameof(paParameter.HodnotaParametra)),
                Source = paParameter,
                Mode = BindingMode.TwoWay,
                ConverterCulture = CultureInfo.CurrentCulture,
                StringFormat = paAttribut?.SpecialFormat
            });
            return box;
        }

        private TextBlock DajLabel(ProcedureParameter paParameter, SqlClassAttribute paAttribut = null)
        {
            return new TextBlock
            {
                Tag = paParameter,
                Text = paAttribut?.ElementName ?? paParameter.NazovParametra,
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private void SpustiFilter(object sender, RoutedEventArgs e)
        {
            _aktualnyVystup.SpustiVystup();
        }

        protected virtual void OnUserKlikolNaElementMamId(object e)
        {
            UserKlikolNaElementMamId?.Invoke(this, e);
        }
    }
}
