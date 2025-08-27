using System.Threading;
using SqlSugar;

namespace Server {
    public class Program {
        static void Main(string[] args) {
            NetServer server = new NetServer();
            server.StartServer(NetDefine.IP, NetDefine.ServerPort);

            SqlSugarClient db = DBMgr.Instance.InitDB();

            LoginController loginController = new LoginController(new LoginService(db));
            // 注册指令集
            server.RegisterCommand(NetDefine.CMD_RegisterCode, loginController);
            server.RegisterCommand(NetDefine.CMD_LoginCode, loginController);
            server.RegisterCommand(NetDefine.CMD_UpdateUserInfoCode, loginController);

            while (true) {
                Thread.Sleep(1);
            }
        }
    }
}