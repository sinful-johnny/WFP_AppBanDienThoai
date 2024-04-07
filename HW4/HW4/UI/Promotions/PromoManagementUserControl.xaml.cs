using HW4.BUS;
using HW4.UI.Promotions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW4
{
    /// <summary>
    /// Interaction logic for PromoManagementUserControl.xaml
    /// </summary>
    public partial class PromoManagementUserControl : UserControl
    {
        SqlConnection _con;
        int _rowPerPage = 32;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        DataTable _promos;
        int mode = Mode.Default;
        BindingList<Page> PageOptions = new BindingList<Page>();
        string _keyWord;
        public PromoManagementUserControl(SqlConnection connection)
        {
            InitializeComponent();
            _con = connection;
        }

        public enum PromotionManagementActions
        {
            AddPromo,
            EditPromo,
            DeletePromo,
            SeeAvailablePromo
        }
        public void HandleParentEvent(PromotionManagementActions action)
        {
            switch (action)
            {
                case PromotionManagementActions.AddPromo:
                    AddPromotion();
                    break;
                case PromotionManagementActions.EditPromo:
                    EditPromotion();
                    break;
                case PromotionManagementActions.DeletePromo:
                    DeletePromotion();
                    break;
                case PromotionManagementActions.SeeAvailablePromo:
                    SeeOpenPromos();
                    break;
            }
        }

        void LoadData()
        {
            try
            {
                int oldTotalPages = totalPages;
                if (mode == Mode.Default)
                {
                    (_promos, totalItems, totalPages) = PROMOTIONS_Control.GetAllPaging(_con, _currentPage, _rowPerPage);
                }else if(mode == Mode.OpenOnly) {
                    (_promos, totalItems, totalPages) = PROMOTIONS_Control.getOpenPromos(_con, _currentPage, _rowPerPage); 
                }else if(mode == Mode.KeyWord)
                {
                    (_promos, totalItems, totalPages) = BUS_Promotions.getPromosWithKeyword(_con, _currentPage, _rowPerPage,_keyWord);
                }
                PromoDataGrid.ItemsSource = _promos.DefaultView;
                if (oldTotalPages != totalPages)
                {
                    PageOptions.Clear();
                    for (int i = 1; i <= totalPages; i++)
                    {
                        PageOptions.Add(new Page()
                        {
                            _pageNo = i,
                            _totalPages = totalPages
                        });
                    }
                    PageSelectComboBox.ItemsSource = PageOptions;
                    PageSelectComboBox.SelectedIndex = 0;
                }

                if (_currentPage == 1)
                {
                    PrevButton.IsEnabled = false;
                }
                else
                {
                    PrevButton.IsEnabled = true;
                }

                if (_currentPage == totalPages)
                {
                    NextButton.IsEnabled = false;
                }
                else
                {
                    NextButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void PageSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSelectComboBox.SelectedItem != null && _currentPage != ((Page)PageSelectComboBox.SelectedItem)._pageNo)
            {
                _currentPage = ((Page)PageSelectComboBox.SelectedItem)._pageNo;
                LoadData();
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            PageSelectComboBox.SelectedItem = PageOptions.FirstOrDefault(x => x._pageNo == _currentPage - 1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PageSelectComboBox.SelectedItem = PageOptions.FirstOrDefault(x => x._pageNo == _currentPage + 1);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            
            PageSelectComboBox.SelectedIndex = 0;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }

        private void AddPromotion()
        {
            var screen = new PromoInfoDialog(_con);
            screen.ShowDialog();
        }

        private void EditPromotion()
        {
            if(PromoDataGrid.SelectedItems.Count != 0)
            {
                var selectRow = (DataRowView)PromoDataGrid.SelectedItems[0]!;
                var screen = new PromoInfoDialog(_con, selectRow);
                screen.ShowDialog();
                LoadData();
            }
            else
            {
                MessageBox.Show("Choose an item to edit!","Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeletePromotion()
        {
            if (PromoDataGrid.SelectedItems.Count != 0)
            {
                var selectedRow = (DataRowView)PromoDataGrid.SelectedItems[0]!;
                int id = (int)selectedRow.Row.ItemArray[0]!;
                var result = BUS_Promotions.deletePromotion(_con, id);
                LoadData();
                if (result == true)
                {
                    MessageBox.Show($"Promotion with ID: {id} has been deleted!", "Success", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show($"Cannot delete promotion with ID: {id}", "Failed", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Choose an item to edit!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeeOpenPromos()
        {
            if(mode != Mode.OpenOnly)
            {
                mode = Mode.OpenOnly;
            }
            else
            {
                mode = Mode.Default;
            }
            
            LoadData();
        }

        private void SeachButton_Click(object sender, RoutedEventArgs e)
        {
            _keyWord = SearchTextBox.Text;
            mode = Mode.KeyWord;
            LoadData();
        }
    }
    internal class Mode
    {
        static public int Default { get { return 0; } }
        static public int OpenOnly { get { return 1; } }
        static public int KeyWord { get { return 2; } }
    }
    internal class Page
    {
        public int _pageNo { get; set; }
        public int _totalPages { get; set; }

        public string Info { get { return $"{_pageNo}/{_totalPages}"; } }
    }
}
