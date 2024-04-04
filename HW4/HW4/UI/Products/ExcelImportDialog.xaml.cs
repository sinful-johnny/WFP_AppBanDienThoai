using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HW4.BUS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace HW4.UI.Products
{
    /// <summary>
    /// Interaction logic for ExcelImportDialog.xaml
    /// </summary>
    public partial class ExcelImportDialog : Window
    {
        string _filename = "";
        SqlConnection _con;
        DataTable _data;
        public ExcelImportDialog(SqlConnection connection)
        {
            InitializeComponent();
            _con = connection;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                FilenameLabel.Content = screen.FileName;
                _filename = screen.FileName;
            }
            if (_filename.Length > 0)
            {
                _data = new DataTable();
                var document = SpreadsheetDocument.Open(_filename, false);
                var wbPart = document.WorkbookPart!;
                var tabs = wbPart.Workbook.Descendants<Sheet>()!;

                var tab = tabs.FirstOrDefault(s => s.Name == "PHONE");
                var wsPart = (WorksheetPart)(wbPart!.GetPartById(tab!.Id!));
                var cells = wsPart.Worksheet.Descendants<Cell>();

                int row = 1;
                Cell nameCell = cells.FirstOrDefault(
                                                        c => c?.CellReference == $"A{row}"
                                                    )!;
                Cell manufacturerCell = cells.FirstOrDefault(
                                                        c => c?.CellReference == $"B{row}"
                                                    )!;
                Cell thumbnailCell = cells.FirstOrDefault(
                                                        c => c?.CellReference == $"C{row}"
                                                    )!;
                Cell priceCell = cells.FirstOrDefault(
                                                        c => c?.CellReference == $"D{row}"
                                                    )!;

                var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault()!;

                _data.Columns.Add(stringTable.SharedStringTable.ElementAt(int.Parse(nameCell!.InnerText)).InnerText);
                _data.Columns.Add(stringTable.SharedStringTable.ElementAt(int.Parse(manufacturerCell!.InnerText)).InnerText);
                _data.Columns.Add(stringTable.SharedStringTable.ElementAt(int.Parse(thumbnailCell!.InnerText)).InnerText);
                _data.Columns.Add(stringTable.SharedStringTable.ElementAt(int.Parse(priceCell!.InnerText)).InnerText);

                row++;
                nameCell = cells.FirstOrDefault(c => c?.CellReference == $"A{row}")!;
                manufacturerCell = cells.FirstOrDefault(c => c?.CellReference == $"B{row}")!;
                thumbnailCell = cells.FirstOrDefault(c => c?.CellReference == $"C{row}")!;
                priceCell = cells.FirstOrDefault(c => c?.CellReference == $"D{row}")!;

                while (nameCell != null)
                {
                    string name = stringTable.SharedStringTable.ElementAt(int.Parse(nameCell!.InnerText)).InnerText;
                    string manufacturer = manufacturerCell!.InnerText;
                    string thumbnail = stringTable.SharedStringTable.ElementAt(int.Parse(thumbnailCell!.InnerText)).InnerText;
                    string price = priceCell!.InnerText;

                    _data.Rows.Add(name, manufacturer, thumbnail, price);

                    row++;
                    nameCell = cells.FirstOrDefault(c => c?.CellReference == $"A{row}")!;
                    manufacturerCell = cells.FirstOrDefault(c => c?.CellReference == $"B{row}")!;
                    thumbnailCell = cells.FirstOrDefault(c => c?.CellReference == $"C{row}")!;
                    priceCell = cells.FirstOrDefault(c => c?.CellReference == $"D{row}")!;
                }
                FileContentDataGrid.ItemsSource = _data.DefaultView;
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BUS_Phone.insertMany(_con, _data);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            DialogResult = true;
        }
    }
}
