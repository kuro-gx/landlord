using SqlSugar;

public class DBMgr : Singleton<DBMgr> {
    public SqlSugarClient InitDB() {
        ConnectionConfig config = new ConnectionConfig() {
            ConnectionString = "server=localhost;Database=landlord;User=root;Password=123456;",
            DbType = DbType.MySql,
            IsAutoCloseConnection = true, // 自动关闭连接
        };
        return new SqlSugarClient(config);
    }
}