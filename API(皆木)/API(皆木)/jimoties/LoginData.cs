using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// リクエストボディのデータを受け取るためのクラス
public class EmployeeLoginRequest
{
    public string Employee_ID { get; set; }
    public string Employee_password { get; set; }
}

public class UserLoginRequest
{
    public string Email { get; set; }
    public string User_password { get; set; }
}

public class UserTableList
{
    public List<UserTableRow> List { get; set; } = new List<UserTableRow>();
}

public class UserTableRow
{
    public int User_ID { get; set; }
    public string User_name { get; set; }
    public string User_password { get; set; }
    public string Mail { get; set; }
    public string Address { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
}