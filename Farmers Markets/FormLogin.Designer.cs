using System;
using System.Windows.Forms;
using Npgsql;

namespace FarmersMarketsApp
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            this.Text = "Подключение к базе данных";
            this.Width = 400;
            this.Height = 300;

            Label lblUser = new Label() { Text = "Пользователь:", Top = 20, Left = 20, Width = 100 };
            TextBox txtUser = new TextBox() { Top = 20, Left = 130, Width = 200, Name = "txtUser" };

            Label lblPassword = new Label() { Text = "Пароль:", Top = 60, Left = 20, Width = 100 };
            TextBox txtPassword = new TextBox() { Top = 60, Left = 130, Width = 200, Name = "txtPassword", PasswordChar = '*' };

            Label lblDatabase = new Label() { Text = "База данных:", Top = 100, Left = 20, Width = 100 };
            TextBox txtDatabase = new TextBox() { Top = 100, Left = 130, Width = 200, Name = "txtDatabase", Text = "farmersmarkets" };

            Button btnConnect = new Button() { Text = "Подключиться", Top = 150, Left = 130, Width = 200 };
            btnConnect.Click += (s, e) =>
            {
                string username = txtUser.Text;
                string password = txtPassword.Text;
                string database = txtDatabase.Text;
                string connectionString = $"Host=localhost;Port=5432;Username={username};Password={password};Database={database}";

                try
                {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        MessageBox.Show("Подключение успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FormMain mainForm = new FormMain(connectionString);
                        mainForm.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblDatabase);
            this.Controls.Add(txtDatabase);
            this.Controls.Add(btnConnect);
        }
    }
}