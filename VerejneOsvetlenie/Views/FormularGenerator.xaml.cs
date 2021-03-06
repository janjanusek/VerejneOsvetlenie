﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
using VerejneOsvetlenie.Converters;
using VerejneOsvetlenieData.Data;
using VerejneOsvetlenieData.Data.Interfaces;
using Image = System.Windows.Controls.Image;

namespace VerejneOsvetlenie.Views
{
    /// <summary>
    /// Interaction logic for FormularGenerator.xaml
    /// </summary>
    public partial class FormularGenerator : UserControl
    {
        public SqlEntita ModelAkoEntita => Model as SqlEntita;
        public object Model => this.DataContext;
        public StavyFormulara AktualnyStav { get; private set; }
        private SqlEntita _aktualnaEntita;
        public Action<SqlEntita> ActionWithNewElement { get; set; }
        public bool Insert { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

        public FormularGenerator()
        {
            InitializeComponent();
            this.DataContextChanged += FormularGenerator_DataContextChanged;
            Insert = true;
            Update = true;
            Delete = true;
            Zmazat.Visibility = Visibility.Collapsed;
        }

        private void FormularGenerator_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Reset();
            GenerujFormular();
        }

        private void Reset()
        {
            HlavnyGrid.Background = new SolidColorBrush(Colors.Transparent);
            Formular.Children.Clear();
            Formular.RowDefinitions.Clear();
            AktualnyStav = StavyFormulara.Init;
            HlavnyGrid.IsEnabled = false;
            NastavTlacidla(Update, false, Insert);
        }

        public void GenerujFormular()
        {
            if (Model == null)
            {
                if (ModelAkoEntita == null)
                    NastavTlacidla(false, false, false);
                return;
            }
            if (ModelAkoEntita != null)
                GenerujPodlaSqlEntity();
            //else
            //    GenerujPodlaNeznamehoTypu();

        }

        private void GenerujPodlaNeznamehoTypu()
        {
            var props = DataContext.GetType().GetProperties();
            FormularTitulok.Text = DataContext.GetType().Name;
            foreach (var propertyInfo in props)
            {
                this.DajNovyRiadok();
                var label = this.DajLabel(propertyInfo);
                var box = this.DajInputBox(propertyInfo);

                label.SetValue(Grid.RowProperty, Formular.RowDefinitions.Count - 1);
                label.SetValue(Grid.ColumnProperty, 0);
                box.SetValue(Grid.RowProperty, Formular.RowDefinitions.Count - 1);
                box.SetValue(Grid.ColumnProperty, 1);
                Formular.Children.Add(label);
                Formular.Children.Add(box);
            }
        }

        private void GenerujPodlaSqlEntity(SqlEntita paRefencia = null)
        {
            var model = _aktualnaEntita = paRefencia ?? ModelAkoEntita;
            var atr = SqlClassAttribute.ExtractSqlClassAttribute(model);
            Zmazat.Visibility = this.Delete && model.DeleteEnabled ? Visibility.Visible : Visibility.Collapsed;
            if (atr.IgnoreEntity)
                return;
            FormularTitulok.Text = ModelAkoEntita != null ? DajAtributTabulky(ModelAkoEntita).ElementName : string.Empty;
            var props = model.GetType().GetProperties();
            foreach (var propertyInfo in props)
            {
                var atribut = this.DajAtributStlpca(propertyInfo);
                if (atribut.ShowElement == false)
                    continue;
                if (atribut.IsReference)
                {
                    var property = propertyInfo.GetValue(model) as SqlEntita;// ?? Activator.CreateInstance(propertyInfo.PropertyType) as SqlEntita;
                    if (property == null)
                        continue;
                    GenerujPodlaSqlEntity(property);
                    continue;
                }
                DajNovyRiadok();
                var label = DajLabel(propertyInfo, atribut);
                UIElement inputBoxOrImage = null;
                if (atribut?.IsBitmapImage == false)
                {
                    if (atribut?.IsDate == false)
                        inputBoxOrImage = DajInputBox(propertyInfo, model, atribut);
                    else
                        inputBoxOrImage =  DajDatePicker(propertyInfo, model);
                }
                else
                {
                    inputBoxOrImage = new Image();
                    var img = new BitmapImage
                    {
                        StreamSource = (MemoryStream)propertyInfo.GetValue(Model)
                    };
                    ((Image)inputBoxOrImage).Source = img;
                }
                label.SetValue(Grid.RowProperty, Formular.RowDefinitions.Count - 1);
                label.SetValue(Grid.ColumnProperty, 0);
                inputBoxOrImage.SetValue(Grid.RowProperty, Formular.RowDefinitions.Count - 1);
                inputBoxOrImage.SetValue(Grid.ColumnProperty, 1);
                Formular.Children.Add(label);
                Formular.Children.Add(inputBoxOrImage);
            }
        }

        private TextBox DajInputBox(PropertyInfo paPropertyInfo, SqlEntita paEntita = null, SqlClassAttribute paAttribut = null)
        {
            var box = new TextBox
            {
                FontSize = 20,
                IsReadOnly = !paPropertyInfo.CanWrite || paAttribut?.ReadOnly == true,
                Margin = new Thickness(5, 5, 0, 5),
                MaxWidth = 180,
                MaxLines = 10,
                TextWrapping = TextWrapping.Wrap,
                MaxLength = (paAttribut?.Length ?? 0) != 0 ? paAttribut.Length : 50
            };
            box.SetBinding(TextBox.TextProperty, new Binding()
            {
                Path = new PropertyPath(paPropertyInfo.Name),
                Source = paEntita ?? DataContext,
                Mode = paPropertyInfo.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ConverterCulture = CultureInfo.CurrentCulture,
                StringFormat = paAttribut?.SpecialFormat
            });
            return box;
        }

        public DatePicker DajDatePicker(PropertyInfo paPropertyInfo, SqlEntita paEntita = null)
        {
            var picker = new DatePicker
            {
                FontSize = 20,
                Margin = new Thickness(5, 5, 0, 5),
                MaxWidth = 180,
                Text = "vyberte dátum",
                SelectedDateFormat = DatePickerFormat.Short,
            };
            var binding = new Binding()
            {
                Path = new PropertyPath(paPropertyInfo.Name),
                Source = paEntita ?? DataContext,
                Mode = paPropertyInfo.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ConverterCulture = CultureInfo.CurrentCulture
            };
            if (paPropertyInfo.PropertyType == typeof(string))
                binding.Converter = new StringDateTimeConverter();

            picker.SetBinding(DatePicker.SelectedDateProperty, binding);

            return picker;
        }

        private TextBlock DajLabel(PropertyInfo paPropertyInfo, SqlClassAttribute paAttribut = null)
        {
            return new TextBlock
            {
                FontSize = 20,
                Text = paAttribut?.ElementName ?? paPropertyInfo.Name,
                Margin = new Thickness(0, 5, 5, 5)
            };
        }

        private SqlClassAttribute DajAtributTabulky(SqlEntita paEntita)
        {
            return SqlClassAttribute.ExtractSqlClassAttribute(paEntita);
        }

        private SqlClassAttribute DajAtributStlpca(PropertyInfo paPropertyInfo)
        {
            return SqlClassAttribute.ExtractSqlClassAttribute(paPropertyInfo);
        }

        private RowDefinition DajNovyRiadok()
        {
            var riadok = new RowDefinition
            {
                Height = GridLength.Auto
            };
            Formular.RowDefinitions.Add(riadok);
            return riadok;
        }

        private RowDefinition DajNovyNovyStlpec()
        {
            return new RowDefinition
            {
                Height = GridLength.Auto
            };
        }

        private void Upravit_Click(object sender, RoutedEventArgs e)
        {
            HlavnyGrid.IsEnabled = true;
            AktualnyStav = StavyFormulara.Update;
            NastavTlacidla(false, true, Insert);
        }

        private void Ulozit_Click(object sender, RoutedEventArgs e)
        {
            var result = false;
            if (AktualnyStav == StavyFormulara.Update)
                result = _aktualnaEntita.Update();
            else if (AktualnyStav == StavyFormulara.Insert)
                result = _aktualnaEntita.Insert();

            GenerujSpravu(result, _aktualnaEntita.ErrorMessage);
            if (result)
            {
                HlavnyGrid.IsEnabled = false;
                AktualnyStav = StavyFormulara.Init;
                NastavTlacidla(Update, false, Insert);
            }
        }

        private void NastavTlacidla(bool paUprav, bool paUloz, bool paNovy)
        {
            Novy.Visibility = paNovy ? Visibility.Visible : Visibility.Collapsed;
            Upravit.Visibility = paUprav ? Visibility.Visible : Visibility.Collapsed;
            Ulozit.Visibility = paUloz ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Novy_Click(object sender, RoutedEventArgs e)
        {
            var novaEntita = Activator.CreateInstance(ModelAkoEntita.GetType()) as SqlEntita;
            ActionWithNewElement?.Invoke(novaEntita);
            Reset();
            AktualnyStav = StavyFormulara.Insert;
            NastavTlacidla(false, true, Insert);
            GenerujPodlaSqlEntity(novaEntita);
            HlavnyGrid.IsEnabled = true;
        }

        public static void GenerujSpravu(bool paVysledokOperacie, string paErrorMessage, Window paNoveOkno = null)
        {
            var sprava = paVysledokOperacie ? "Operácia prebehla úspešne po obnovení záznamu uvidíte zmeny." : paErrorMessage;
            MessageBox.Show(paNoveOkno ?? SpravaZaznamovWindow.AktualneOkno, sprava,
            "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Zmazat_Click(object sender, RoutedEventArgs e)
        {
            var odpoved = MessageBox.Show(SpravaZaznamovWindow.AktualneOkno, "Ste si istý že chcete zmazať túto položku?",
            "Upozornenie", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (odpoved == MessageBoxResult.OK)
            {
                var result = _aktualnaEntita.Drop();
                GenerujSpravu(result, _aktualnaEntita.ErrorMessage);
                if (result)
                {
                    NastavTlacidla(false, false, false);
                    Zmazat.Visibility = Visibility.Collapsed;
                    HlavnyGrid.Background = new SolidColorBrush(Colors.LightSalmon);
                }
            }
        }

    }

    public enum StavyFormulara
    {
        Init, Update, Insert, Delete
    }
}
