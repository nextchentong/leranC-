using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;

String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=test;";
MySqlConnection conn = new MySqlConnection(connetStr);

try
{
    conn.Open();//打开通道，建立连接，可能出现异常，使用try catch语句
    Console.WriteLine("已建立连接");
    //在这里使用代码对数据库进行增删查改
    // 查询
    string sql = "select * from user;";

    // 新增
    // string sql = "insert into user(name, id,password) values('啊宽','123','陈通22222')";

    // 删除
    // string sql = "delete from user where id = '123';";

    // 更新
    // string sql = "update user set name = '修改后',password = '修改后密码' where id = '2';";
    MySqlCommand cmd = new MySqlCommand(sql, conn);
    MySqlDataReader? reader = null;

    reader = cmd.ExecuteReader();

    // int result = cmd.ExecuteNonQuery();//3.执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
    // Console.WriteLine(result);
    while (reader.Read())
    {
        //Console.WriteLine(reader[0].ToString()+reader[1].ToString()+reader[2].ToString();
        //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
        Console.WriteLine(reader.GetString("name") + reader.GetString("id") + reader.GetString("password"));
    }







}
catch (MySqlException ex)
{
    Console.WriteLine("Catch");
    Console.WriteLine(ex.Message);
}
finally
{
    conn.Close();
}
