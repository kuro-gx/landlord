using System;
using SqlSugar;

[SugarTable("tb_user")]
public class User {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public string Username { get; set; }

    public string Mobile { get; set; }
    public string Password { get; set; }

    public string Avatar { get; set; }

    public byte Gender { get; set; }

    // 状态
    public byte State { get; set; }

    // 备注
    public string Remark { get; set; }

    [SugarColumn(ColumnName = "create_time")]
    public DateTime CreateTime { get; set; }

    [SugarColumn(ColumnName = "update_time")]
    public DateTime UpdateTime { get; set; }
}