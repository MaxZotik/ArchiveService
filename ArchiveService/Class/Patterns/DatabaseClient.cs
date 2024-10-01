namespace ArchiveService.Class.Patterns
{
    class DatabaseClient
    {
        public string Server { get;  }
        public string ServerName { get; }
        public string DatabaseName { get; }
        public string Login { get; }
        public string Password { get; }
        public DatabaseClient(string server, string serverName, string databaseName, string login, string password)
        {
            Server = server;
            ServerName = serverName;
            DatabaseName = databaseName;
            Login = login;
            Password = password;
        }
    }
}
