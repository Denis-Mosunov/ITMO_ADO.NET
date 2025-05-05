using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace FarmersMarketsApp
{
    public partial class FormMain : Form
    {
        private readonly string _connectionString;
        private readonly NpgsqlConnection _connection;
        private DataTable _table;
        private NpgsqlDataAdapter _adapter;
        private string _currentTable = "farmers";

        private ComboBox cmbTables;
        private DataGridView dgv;

        public FormMain(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();

            this.Text = "Таблицы базы данных";
            this.Width = 800;
            this.Height = 600;

            cmbTables = new ComboBox() { Dock = DockStyle.Top };
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged;

            dgv = new DataGridView() { Dock = DockStyle.Fill, Name = "dgv" };

            Button btnSave = new Button() { Text = "Сохранить изменения", Dock = DockStyle.Top };
            btnSave.Click += BtnSave_Click;

            Button btnRefresh = new Button() { Text = "Обновить данные", Dock = DockStyle.Top };
            btnRefresh.Click += BtnRefresh_Click;

            this.Controls.Add(dgv);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnSave);
            this.Controls.Add(cmbTables);

            LoadTables();
            LoadData();
        }

        private void LoadTables()
        {
            try
            {
                cmbTables.Items.Clear();
                using (var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'", _connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbTables.Items.Add(reader.GetString(0));
                    }
                }

                if (cmbTables.Items.Count > 0)
                {
                    cmbTables.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка таблиц: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentTable))
                    return;

                _adapter = new NpgsqlDataAdapter($"SELECT * FROM {_currentTable}", _connection);
                var builder = new NpgsqlCommandBuilder(_adapter);

                _adapter.InsertCommand = builder.GetInsertCommand(true);
                _adapter.UpdateCommand = builder.GetUpdateCommand(true);
                _adapter.DeleteCommand = builder.GetDeleteCommand(true);

                _table = new DataTable();
                _adapter.Fill(_table);

                dgv.DataSource = _table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _adapter.Update(_table);
                MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void CmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentTable = cmbTables.SelectedItem.ToString();
            LoadData();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
                _connection.Dispose();
            }
            base.OnFormClosed(e);
        }
    }
}
