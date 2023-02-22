using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Timers;
public class User
{
    public int id { get; set; }
    public string password { get; set; }
    public string name { get; set; }
    public string text { get; set; }

}

partial class Program
{
    static HttpListener httpobj;
    static void Main(string[] args)
    {
        //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
        httpobj = new HttpListener();
        //定义url及端口号，通常设置为配置文件
        httpobj.Prefixes.Add("http://+:8080/");
        //启动监听器
        httpobj.Start();
        //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
        //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
        httpobj.BeginGetContext(Result, null);
        Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
        Console.ReadKey();
    }


    private static void Result(IAsyncResult ar)
    {
        //当接收到请求后程序流会走到这里

        //继续异步监听
        httpobj.BeginGetContext(Result, null);
        var guid = Guid.NewGuid().ToString();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"接到新的请求:{guid},时间：{DateTime.Now.ToString()}");
        //获得context对象
        var context = httpobj.EndGetContext(ar);
        var request = context.Request;
        var response = context.Response;
        var requestUrl = request.RawUrl;

        Console.WriteLine(request.Url);
        Console.WriteLine(request.IsLocal);
        Console.WriteLine(request.InputStream);

        Console.WriteLine(request.HttpMethod);
        Console.WriteLine(request.RawUrl);
        Console.WriteLine(request.QueryString);
        Console.WriteLine(request.HasEntityBody);
        //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
        //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");//后台跨域参数设置，通常设置为配置文件
        //context.Response.AppendHeader("Access-Control-Allow-Method", "post");//后台跨域请求设置，通常设置为配置文件
        context.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
        context.Response.AddHeader("Content-type", "text/plain");//添加响应头信息
        context.Response.ContentEncoding = Encoding.UTF8;
        string returnObj = null;//定义返回客户端的信息


        //处理客户端发送的请求并返回处理信息
        returnObj = HandleRequest(request, response, requestUrl);


        var returnByteArr = Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
        try
        {
            using (var stream = response.OutputStream)
            {
                //把处理信息返回到客户端
                stream.Write(returnByteArr, 0, returnByteArr.Length);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"网络蹦了：{ex.ToString()}");
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"请求处理完成：{guid},时间：{DateTime.Now.ToString()}\r\n");
    }
    // 定时器
    private static void test(object source, ElapsedEventArgs e)
    {

        Console.WriteLine("OK, test event is fired at: " + DateTime.Now.ToString());

    }
    // 定时任务
    private static void test2(object source, ElapsedEventArgs e)
    {

        if (DateTime.Now.Hour == 14 && DateTime.Now.Minute == 14)  //如果当前时间是10点30分
            Console.WriteLine("OK, event fired at: " + DateTime.Now.ToString());

    }
    // 多线程1
    private static void DownLoadFile()
    {
        Console.WriteLine($"开始下载,线程ID:{Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(500);
        Console.WriteLine("下载完成!");
    }
    // 多线程2
    public static void TestThreadPool(object state)
    {
        string[] arry = state as string[];//传过来的参数值
        int workerThreads = 0;
        int CompletionPortThreads = 0;
        ThreadPool.GetMaxThreads(out workerThreads, out CompletionPortThreads);
        Console.WriteLine(DateTime.Now.ToString() + "---" + arry[0] + "--workerThreads=" + workerThreads + "--CompletionPortThreads" + CompletionPortThreads);
    }
    private static string HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string requestUrl)
    {
        string data = null;
        try
        {
            var byteList = new List<byte>();
            var byteArr = new byte[2048];
            int readLen = 0;
            int len = 0;
            //接收客户端传过来的数据并转成字符串类型
            do
            {
                readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                len += readLen;
                byteList.AddRange(byteArr);
            } while (readLen != 0);
            data = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);


            //获取得到数据data可以进行其他操作
        }
        catch (Exception ex)
        {
            response.StatusDescription = "404";
            response.StatusCode = 404;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"在接收数据时发生错误:{ex.ToString()}");
            return $"在接收数据时发生错误:{ex.ToString()}";//把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
        }
        response.StatusDescription = "200";//获取或设置返回给客户端的 HTTP 状态代码的文本说明。
        response.StatusCode = 200;// 获取或设置返回给客户端的 HTTP 状态代码。
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"接收数据完成:{data.Trim()},时间：{DateTime.Now.ToString()}");
        Console.WriteLine($"typeof:{data.Trim().GetType()},时间：{DateTime.Now.ToString()}");

        var newData = JsonSerializer.Deserialize<User>(data.Trim());
        Console.WriteLine(newData);


        String connetStr = "server=121.4.251.168;port=3306;user=root;password=123456; database=test;";
        MySqlConnection conn = new MySqlConnection(connetStr);

        try
        {
            conn.Open();//打开通道，建立连接，可能出现异常，使用try catch语句
            Console.WriteLine("已建立连接");
            string sql = "";

            switch (requestUrl)
            {
                case "/query":
                    // 查询
                    sql = $"select * from user  where id = '{newData.id}';";

                    break;
                case "/add":
                    // 新增
                    sql = $"insert into user(name,password) values('{newData.name}','{newData.password}')";
                    break;
                case "/update":
                    // 更新
                    sql = $"update user set name = '{newData.name}',password = '{newData.password}' where id = '{newData.id}';";
                    break;
                case "/delete":
                    // 删除
                    sql = $"delete from user where id = '{newData.id}';";
                    break;
                default: break;
            }
            //在这里使用代码对数据库进行增删查改








            MySqlCommand cmd = new MySqlCommand(sql, conn);

            int result = -1;
            var result2 = new User();

            switch (requestUrl)
            {
                case "/query":
                    MySqlDataReader? reader = null;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //Console.WriteLine(reader[0].ToString()+reader[1].ToString()+reader[2].ToString();
                        //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                        Console.WriteLine(reader.GetString("name") + reader.GetString("id") + reader.GetString("password"));
                        result2.name = reader.GetString("name");
                        result2.id = reader.GetInt16("id");
                        result2.password = reader.GetString("password");
                        result2.text = "操作成功";
                    }
               


                    return JsonSerializer.Serialize(result2);
                case "/add":
                    // 新增
                    result = cmd.ExecuteNonQuery();//3.执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行

                    break;
                case "/update":
                    // 更新
                    result = cmd.ExecuteNonQuery();//3.执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行
                    break;
                case "/delete":
                    // 删除
                    result = cmd.ExecuteNonQuery();//3.执行插入、删除、更改语句。执行成功返回受影响的数据的行数，返回1可做true判断。执行失败不返回任何数据，报错，下面代码都不执行

                    break;
                default: break;
            }
            Console.WriteLine(result);
            if (result != -1)
            {
                result2.text = "操作成功";
            }
            else
                result2.text = "操作失败";
            {
            }
            return JsonSerializer.Serialize(result2);







        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Catch");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 2000; //执行间隔时间,单位为毫秒; 这里实际间隔为10分钟  
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(test2);
            // Thread类开启线程

            Thread t = new Thread(() =>
            {
                Console.WriteLine("开始下载" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000);
                Console.WriteLine("下载完成");
            });
            t.Start();
            //线程睡眠
            t.Join(1000);
            //挂起线程 已弃用
            // t.Suspend();
            //继续执行线程 已弃用
            // t.Resume();
            //结束线程 已弃用
            /// t.Abort();

            // Task开启线程
            Task task = new Task(DownLoadFile);
            task.Start();
            // 通过线程池开启线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(TestThreadPool), new string[] { "test" });

            //Console.ReadKey();
            conn.Close();
        }


        return $"{data:'接收数据完成'}";
    }

}