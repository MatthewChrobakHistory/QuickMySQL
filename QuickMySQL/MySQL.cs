using MySql.Data.MySqlClient;
using System;

namespace QuickMySQL.MySQL
{
    public class MySQL
    {
        private MySqlConnection _connection;
        private MySqlCommand _command;
        private MySqlDataReader _reader;

        public MySQL(string serverHost, string userID, string userPassword, string database) {
            string _connectionString = string.Format("Server={0};Database={1};Uid={2};Pwd={3}",
                serverHost,
                database,
                userID,
                userPassword
            );
            this._connection = new MySqlConnection(_connectionString);
            this._connection.StateChange += _connection_StateChange;

            try {
                _connection.Open();
            } catch (MySqlException e) {
                Console.WriteLine(e.Message);
                _connection.Close();
            }
        }

        private void _connection_StateChange(object sender, System.Data.StateChangeEventArgs e) {
            Console.WriteLine(e.OriginalState + " -> " + e.CurrentState);
        }

        public bool Query(string query) {
            this._command?.Dispose();
            this._reader?.Close();
            this._command = new MySqlCommand(query, this._connection);
            this._command.Prepare();
            try {
                this._reader = _command.ExecuteReader();
                return true;
            } catch (MySqlException e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool NonQuery(string query) {
            this._command?.Dispose();
            this._reader?.Close();
            this._command = new MySqlCommand(query, this._connection);
            this._command.Prepare();
            try {
                this._command.ExecuteNonQuery();
                return true;
            } catch (MySqlException e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public object GetValue(string key) {
            for (int i = 0; i < this._reader.FieldCount; i++) {
                if (this._reader.GetName(i).ToLower() == key.ToLower()) {
                    return this._reader.GetValue(i);
                }
            }

            Console.WriteLine("Key not found: " + key);
            return default(object);
        }

        public bool NextEntry() {
            if (this._reader == null || this._reader.IsClosed) {
                return false;
            }
            return this._reader.Read();
        }

        public void Close() {
            this._connection.Close();
        }

        public static void ShowQuickHelp() {
            Console.WriteLine("CREATE TABLE Table_Name(Column1 Type1, Column2 Type2);");
            Console.WriteLine("SELECT Column1,Column2 FROM Table_Name WHERE Condition;");
            Console.WriteLine("INSERT INTO Table_Name(Column1, Column2) VALUES(Value1, Value2);");
            Console.WriteLine("ALTER TABLE Table_Name ADD Column_Name Type;");
            Console.WriteLine("DELETE FROM Table_Name WHERE Condition;");
            Console.WriteLine("UPDATE Table_Name SET Column1=Value1, Column2=Value2 WHERE condition;");
        }
    }
}
