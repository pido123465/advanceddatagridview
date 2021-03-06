﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace AdvancedDataGridViewSample
{
    public partial class FormMain : Form
    {
        private DataTable _dataTable = null;
        private DataSet _dataSet = null;

        private SortedDictionary<int, string> _filtersaved = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> _sortsaved = new SortedDictionary<int, string>();

        public FormMain()
        {
            InitializeComponent();
            
            //set filter and sort saved
            _filtersaved.Add(0, "");
            _sortsaved.Add(0, "");
            comboBox_filtersaved.DataSource = new BindingSource(_filtersaved, null);
            comboBox_filtersaved.DisplayMember = "Key";
            comboBox_filtersaved.ValueMember = "Value";
            comboBox_filtersaved.SelectedIndex = -1;
            comboBox_sortsaved.DataSource = new BindingSource(_sortsaved, null);
            comboBox_sortsaved.DisplayMember = "Key";
            comboBox_sortsaved.ValueMember = "Value";
            comboBox_sortsaved.SelectedIndex = -1;

            //initialize dataset
            _dataTable = new DataTable();
            _dataSet = new DataSet();

            //initialize bindingsource
            bindingSource_main.DataSource = _dataSet;

            //initialize datagridview
            advancedDataGridView_main.DataSource = bindingSource_main;

            //set bindingsource
            SetTestData();
        }

        private void SetTestData()
        {
            _dataTable = _dataSet.Tables.Add("TableTest");
            _dataTable.Columns.Add("int", typeof(int));
            _dataTable.Columns.Add("decimal", typeof(decimal));
            _dataTable.Columns.Add("double", typeof(double));
            _dataTable.Columns.Add("date", typeof(DateTime));
            _dataTable.Columns.Add("datetime", typeof(DateTime));
            _dataTable.Columns.Add("string", typeof(string));
            _dataTable.Columns.Add("boolean", typeof(bool));
            _dataTable.Columns.Add("guid", typeof(Guid));

            bindingSource_main.DataMember = _dataTable.TableName;

            advancedDataGridViewSearchToolBar_main.SetColumns(advancedDataGridView_main.Columns);
        }

        private void AddTestData()
        {
            for (int i = 0; i <= 100; i++)
            {
                object[] newrow = new object[] { 
                    i, 
                    (decimal)i*2/3,
                    i % 2 == 0 ? (double)i*2/3 : (double)i/2, 
                    DateTime.Today.AddHours(i*2).AddHours(i%2 == 0 ?i*10+1:0).AddMinutes(i%2 == 0 ?i*10+1:0).AddSeconds(i%2 == 0 ?i*10+1:0).AddMilliseconds(i%2 == 0 ?i*10+1:0).Date,
                    DateTime.Today.AddHours(i*2).AddHours(i%2 == 0 ?i*10+1:0).AddMinutes(i%2 == 0 ?i*10+1:0).AddSeconds(i%2 == 0 ?i*10+1:0).AddMilliseconds(i%2 == 0 ?i*10+1:0),
                    i*2 % 3 == 0 ? null : i.ToString()+" str", 
                    i % 2 == 0 ? true:false, 
                    Guid.NewGuid()
                };
                
                _dataTable.Rows.Add(newrow);
            }
        }
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            //add test data to bindsource
            AddTestData();

            //setup datagridview
            advancedDataGridView_main.DisableFilterAndSort(advancedDataGridView_main.Columns["int"]);
            advancedDataGridView_main.SetFilterDateAndTimeEnabled(advancedDataGridView_main.Columns["datetime"], true);
            advancedDataGridView_main.SetSortEnabled(advancedDataGridView_main.Columns["guid"], false);
            advancedDataGridView_main.SortDESC(advancedDataGridView_main.Columns["double"]);
        }

        private void advancedDataGridView_main_FilterStringChanged(object sender, EventArgs e)
        {
            bindingSource_main.Filter = advancedDataGridView_main.FilterString;
            textBox_filter.Text = bindingSource_main.Filter;
        }

        private void advancedDataGridView_main_SortStringChanged(object sender, EventArgs e)
        {
            bindingSource_main.Sort = advancedDataGridView_main.SortString;
            textBox_sort.Text = bindingSource_main.Sort;
        }

        private void bindingSource_main_ListChanged(object sender, ListChangedEventArgs e)
        {
            textBox_total.Text = bindingSource_main.List.Count.ToString();
        }

        private void button_savefilters_Click(object sender, EventArgs e)
        {
            _filtersaved.Add((comboBox_filtersaved.Items.Count-1) + 1, advancedDataGridView_main.FilterString);
            comboBox_filtersaved.DataSource = new BindingSource(_filtersaved, null);
            comboBox_filtersaved.SelectedIndex = comboBox_filtersaved.Items.Count-1;
            _sortsaved.Add((comboBox_sortsaved.Items.Count-1) + 1, advancedDataGridView_main.SortString);
            comboBox_sortsaved.DataSource = new BindingSource(_sortsaved, null);
            comboBox_sortsaved.SelectedIndex = comboBox_sortsaved.Items.Count-1;
        }

        private void button_setsavedfilter_Click(object sender, EventArgs e)
        {
            advancedDataGridView_main.LoadFilterAndSort(comboBox_filtersaved.SelectedValue.ToString(), comboBox_sortsaved.SelectedValue.ToString());
        }

        private void button_unloadfilters_Click(object sender, EventArgs e)
        {
            advancedDataGridView_main.CleanFilterAndSort();
            comboBox_filtersaved.SelectedIndex = -1;
            comboBox_sortsaved.SelectedIndex = -1;
        }

        private void advancedDataGridViewSearchToolBar_main_Search(object sender, Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = advancedDataGridView_main.CurrentCell.ColumnIndex + 1 >= advancedDataGridView_main.ColumnCount;
                bool endrow = advancedDataGridView_main.CurrentCell.RowIndex + 1 >= advancedDataGridView_main.RowCount;

                if (endcol && endrow)
                {
                    startColumn = advancedDataGridView_main.CurrentCell.ColumnIndex;
                    startRow = advancedDataGridView_main.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : advancedDataGridView_main.CurrentCell.ColumnIndex + 1;
                    startRow = advancedDataGridView_main.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = advancedDataGridView_main.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);

            if (c != null)
                advancedDataGridView_main.CurrentCell = c;
        }
        
    }
}
