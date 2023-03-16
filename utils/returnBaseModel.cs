
namespace sssMoonlet;
public class returnBaseModel
{
    /**
     * http状态码，http状态码
     */
    private long code;
    /**
     * 返回的数据，返回的数据
     */
    private Data data;
    /**
     * 提示消息，提示消息
     */
    private String message;

    public long getCode() { return code; }
    public void setCode(long value) { this.code = value; }

    public Data getData() { return data; }
    public void setData(Data value) { this.data = value; }

    public String getMessage() { return message; }
    public void setMessage(String value) { this.message = value; }
}

// Data.java


/**
 * 返回的数据，返回的数据
 */
public class Data
{
    /**
     * token，token
     */
    private static String token;

    public static String getToken() { return token; }
    public static void setToken(String value) { token = value; }
}

